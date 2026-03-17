using System.Numerics;
using HanZombiePlagueS2;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Players;

namespace ShopCore;

public class ShopHZPCreditsService
{
    private readonly ILogger<ShopHZPCreditsService> _logger;
    private readonly ISwiftlyCore _core;
    private readonly ShopHZPCreditsGlobals _globals;
    private readonly IOptionsMonitor<ShopHZPCreditsCFG> _cfg;

    public ShopHZPCreditsService(ISwiftlyCore core, ILogger<ShopHZPCreditsService> logger,
        ShopHZPCreditsGlobals globals, IOptionsMonitor<ShopHZPCreditsCFG> CFG)
    {
        _core = core;
        _logger = logger;
        _globals = globals;
        _cfg = CFG;
    }

    public void StartPlaytimeTimer(int playerId)
    {
        var player = _core.PlayerManager.GetPlayer(playerId);
        if (player == null || !player.IsValid || player.IsFakeClient)
            return;

        //_core.Logger.LogInformation($"StartPlaytimeTimer 执行成功 playerId={playerId}");

        var config = _cfg.CurrentValue;
        var intervalSeconds = config.Global.PlaytimeIntervalMinutes * 60f;

        CancellationTokenSource cts = new CancellationTokenSource();

        _globals.PlaytimeTasks[playerId] = cts;

        _core.Scheduler.DelayBySeconds(intervalSeconds, () =>
        {
            var currentPlayer = _core.PlayerManager.GetPlayer(playerId);
            if (currentPlayer == null || !currentPlayer.IsValid)
            {
                _globals.PlaytimeTasks.Remove(playerId);
                cts.Dispose();
                return;
            }

            GivePlayerCredits(currentPlayer, config.Global.PlaytimeReward, $"{T(currentPlayer, "ReasonOnline", config.Global.PlaytimeIntervalMinutes)}");

            if (!cts.IsCancellationRequested)
            {
                StartPlaytimeTimer(playerId);  
            }
        }); 
    }

    public void OnPlayerDisconnect(int playerId)
    {
        if (_globals.PlaytimeTasks.TryGetValue(playerId, out var cts))
        {
            try
            {
                cts.Cancel();
                cts.Dispose();  
            }
            catch (Exception ex)
            {
                _core.Logger.LogWarning($"取消 timer 失败 playerId={playerId}, {ex.Message}");
            }
            _globals.PlaytimeTasks.Remove(playerId);
        }
        _globals.PlayerStates.Remove(playerId);
    }

    public ClassReward? GetZombieClassReward(IPlayer player)
    {
        var _zpAPI = Shop_HZP_Credits._zpApi;
        if (_zpAPI == null)
            return null;

        var className = _zpAPI.HZP_GetZombieClassname(player);
        if (string.IsNullOrEmpty(className))
            return null;

        var config = _cfg.CurrentValue;

        // 处理 ZombieClassRewards 可能为 null 的情况
        var rewards = config.ZombieClassRewards;
        if (rewards == null || rewards.Count == 0)
            return null;

        return rewards.FirstOrDefault(x =>
            x.Enabled &&
            !string.IsNullOrEmpty(x.ClassName) &&
            x.ClassName.Equals(className, StringComparison.OrdinalIgnoreCase));
    }
    public void GivePlayerCredits(IPlayer player, int credits, string reason)
    {
        if(player.IsFakeClient)
            return;

        if(credits <= 0)
            return;

        var _shopApi = Shop_HZP_Credits._shopApi;
        if (_shopApi == null) 
            return;

        _shopApi.AddCredits(player, credits);

        _core.PlayerManager.SendMessage(MessageType.Chat, $"{T(player, "Globals", credits, reason)}");
    }

    public string T(IPlayer? player, string key, params object[] args)
    {
        if (player == null || !player.IsValid)
            return string.Format(key, args);

        var localizer = _core.Translation.GetPlayerLocalizer(player);
        return localizer[key, args];
    }

    public void RegisterZPEvents(IHanZombiePlagueAPI _zpApi)
    {
        var config = _cfg.CurrentValue;
        if (!config.Enabled)
            return;
        
        _zpApi!.HZP_OnPlayerInfect += (attacker, player, grenade, zombieName) =>
        {
            if (attacker == null || !attacker.IsValid || player == null || !player.IsValid)
                return;

            var classReward = GetZombieClassReward(attacker);
            int reward = config.Global.InfectReward;

            int extra = 0;
            string className = "";

            if (classReward != null)
            {
                extra = classReward.InfectHumanBonus ?? 0;
                className = classReward.ClassName;
                reward += extra;
            }

            GivePlayerCredits(attacker, reward, $"{T(attacker, "ReasonInfect")}");

            if (extra > 0)
            {
                attacker.SendMessage(MessageType.Chat, $"{T(attacker, "ExtraInfectBonus", extra, className)}");
            }
        };

        _zpApi.HZP_OnHumanWin += (result) =>
        {
            var allplayers = _core.PlayerManager.GetAllPlayers();

            foreach (var player in allplayers)
            {
                if (player == null || !player.IsValid)
                    continue;

                if (result && !_zpApi!.HZP_IsZombie(player.PlayerID))
                {
                    GivePlayerCredits(player, config.Global.HumanWinReward, $"{T(player, "ReasonHumanWin")}");
                }
                else if (!result && _zpApi!.HZP_IsZombie(player.PlayerID))
                {
                    GivePlayerCredits(player, config.Global.ZombieWinReward, $"{T(player, "ReasonZombieWin")}");
                }
            }
        };
    }



}