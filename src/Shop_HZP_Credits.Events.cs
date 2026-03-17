
using System.Numerics;
using HanZombiePlagueS2;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Commands;
using SwiftlyS2.Shared.GameEventDefinitions;
using SwiftlyS2.Shared.Helpers;
using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.Players;

namespace ShopCore;

public class ShopHZPCreditsEvents
{
    private readonly ILogger<ShopHZPCreditsEvents> _logger;
    private readonly ISwiftlyCore _core;
    private readonly ShopHZPCreditsService _service;
    private readonly ShopHZPCreditsGlobals _globals;
    private readonly IOptionsMonitor<ShopHZPCreditsCFG> _cfg;  

    public ShopHZPCreditsEvents(ISwiftlyCore core, ILogger<ShopHZPCreditsEvents> logger,
        ShopHZPCreditsService service, IOptionsMonitor<ShopHZPCreditsCFG> CFG,
        ShopHZPCreditsGlobals globals)
    {
        _core = core;
        _logger = logger;
        _service = service;
        _cfg = CFG;
        _globals = globals;
    }

    public void HookEvents()
    {
        _core.GameEvent.HookPre<EventPlayerDeath>(OnPlayerDeath);
        _core.GameEvent.HookPre<EventPlayerHurt>(OnPlayerHurt);
        _core.Event.OnClientPutInServer += Event_OnClientPutInServer;
        _core.Event.OnClientDisconnected += Event_OnClientDisconnected;

        var config = _cfg.CurrentValue;
        string command = !string.IsNullOrEmpty(config.InfoCommand) ? config.InfoCommand : "sw_money";

        _core.Command.RegisterCommand(command, ShowInfoCredits, true);
    }

    public void ShowInfoCredits(ICommandContext context)
    {
        var player = context.Sender;
        if (player == null || !player.IsValid) 
            return;

        var _shopAPI = Shop_HZP_Credits._shopApi;
        var _zpAPI = Shop_HZP_Credits._zpApi;
        if (_shopAPI == null || _zpAPI == null)
            return;

       
        if (!_globals.PlayerStates.TryGetValue(player.PlayerID, out var state) || state == null)
            return;

        var config = _cfg.CurrentValue;
        if (!config.Enabled)
            return;

        var classReward = _service.GetZombieClassReward(player);

        float damageDealt = state.DamageDealt;
        float damageTaken = state.DamageTaken;
        var credits = _shopAPI.GetCredits(player);

        bool isZombie = false;
        try 
        {
            isZombie = _zpAPI.HZP_IsZombie(player.PlayerID); 
        } 
        catch 
        { 
            isZombie = false; 
        }

        
        string info = $"<span><font color='#4CAF50'>{_service.T(player, "HudCredits", (int)credits)}</font></span><br>";

        if (isZombie)
        {
            info += $"<span><font color='#FF8C00'>{_service.T(player, "HudDamageTakenThreshold", damageTaken, config.Global.DamageTakenThreshold)}</font></span><br>";
            info += $"<span><font color='#E22D2D'>{_service.T(player, "HudInfectReward", config.Global.InfectReward)}</font></span><br>";
            info += $"<span><font color='#FF8C00'>{_service.T(player, "HudKillHumanReward", config.Global.KillHumanReward)}</font></span><br>";
            info += $"<span><font color='#E22D2D'>{_service.T(player, "HudDamageTakenReward", config.Global.DamageTakenReward)}</font></span>";
        }
        else
        {
            info += $"<span><font color='#FF8C00'>{_service.T(player, "HudDamageThreshold", damageDealt, config.Global.DamageThreshold)}</font></span><br>";
            info += $"<span><font color='#FF69B4'>{_service.T(player, "HudKillZombieReward", config.Global.KillZombieReward)}</font></span><br>";
            info += $"<span><font color='#4CAF50'>{_service.T(player, "HudDamageReward", config.Global.DamageReward)}</font></span><br>";
            info += $"<span><font color='#FFD700'>{_service.T(player, "HudTips")}</font></span>";
        }
        player.SendMessage(MessageType.CenterHTML, info);
    }
   

    private void Event_OnClientPutInServer(SwiftlyS2.Shared.Events.IOnClientPutInServerEvent @event)
    {
        var id = @event.PlayerId;
        if (!_globals.PlayerStates.TryGetValue(id, out var state) || state == null)
        {
            state = new ShopHZPCreditsGlobals.PlayerRewardState();
            _globals.PlayerStates[id] = state;
        }

        var config = _cfg.CurrentValue;
        if (!config.Enabled)
            return;

        _core.Scheduler.DelayBySeconds(30.0f, () => 
        {
            _service.StartPlaytimeTimer(id);
        });
    }

    private void Event_OnClientDisconnected(SwiftlyS2.Shared.Events.IOnClientDisconnectedEvent @event)
    {
        var id = @event.PlayerId;
        _service.OnPlayerDisconnect(id);
    }

    private HookResult OnPlayerHurt(EventPlayerHurt @event)
    {
        var attacker = @event.AttackerPlayer;
        if (attacker == null || !attacker.IsValid)
            return HookResult.Continue;

        var player = @event.UserIdPlayer;
        if (player == null || !player.IsValid)
            return HookResult.Continue;

        var Pawn = player.PlayerPawn;
        if (Pawn == null || !Pawn.IsValid)
            return HookResult.Continue;

        var Controller = player.Controller;
        if (Controller == null || !Controller.IsValid)
            return HookResult.Continue;

        var _zpAPI = Shop_HZP_Credits._zpApi;
        if (_zpAPI == null)
            return HookResult.Continue;

        var config = _cfg.CurrentValue;
        if (!config.Enabled)
            return HookResult.Continue;

        bool attackerZombie = _zpAPI.HZP_IsZombie(attacker.PlayerID);
        bool victimZombie = _zpAPI.HZP_IsZombie(player.PlayerID);
        short rawDamage = @event.DmgHealth;
        if (!attackerZombie && victimZombie)
        {
            if (attacker == player)
                return HookResult.Continue;

            float damage = rawDamage;

            var classReward = _service.GetZombieClassReward(player);

            if (!attacker.IsFakeClient)
            {
                int attackerID = attacker.PlayerID;

                if (_globals.PlayerStates.TryGetValue(attackerID, out var attackerState))
                {

                    float multiplier = classReward?.DamageMultiplier ?? 1f;
                    float progress = damage * multiplier;
                    attackerState.DamageDealt += progress;
                    if (attackerState.DamageDealt >= config.Global.DamageThreshold)
                    {
                        attackerState.DamageDealt = 0;
                        _service.GivePlayerCredits(attacker, config.Global.DamageReward, $"{_service.T(attacker, "ReasonDamageThreshold")}");
                    }
                }

            }
            if (!player.IsFakeClient)
            {
                int victimId = player.PlayerID;

                if (_globals.PlayerStates.TryGetValue(victimId, out var victimState))
                {
                    float multiplier = classReward?.DamageTakenMultiplier ?? 1f;
                    float progress = damage * multiplier;
                    victimState.DamageTaken += progress;
                    if (victimState.DamageTaken >= config.Global.DamageTakenThreshold)
                    {
                        victimState.DamageTaken = 0;
                        _service.GivePlayerCredits(player, config.Global.DamageTakenReward, $"{_service.T(player, "ReasonDamageTakenThreshold")}");
                    }
                }
            }
        }

        return HookResult.Continue;
    }

    private HookResult OnPlayerDeath(EventPlayerDeath @event)
    {
        var attacker = @event.AttackerPlayer;
        if (attacker == null || !attacker.IsValid)
            return HookResult.Continue;

        var player = @event.UserIdPlayer;
        if (player == null || !player.IsValid)
            return HookResult.Continue;

        var Pawn = player.PlayerPawn;
        if (Pawn == null || !Pawn.IsValid)
            return HookResult.Continue;

        var Controller = player.Controller;
        if (Controller == null || !Controller.IsValid)
            return HookResult.Continue;

        var _zpAPI = Shop_HZP_Credits._zpApi;
        if (_zpAPI == null)
            return HookResult.Continue;

        var config = _cfg.CurrentValue;
        if(!config.Enabled)
            return HookResult.Continue;

        bool attackerZombie = _zpAPI.HZP_IsZombie(attacker.PlayerID);
        bool victimZombie = _zpAPI.HZP_IsZombie(player.PlayerID);
        if (attackerZombie && !victimZombie)
        {
            var classReward = _service.GetZombieClassReward(attacker);
            int reward = config.Global.KillHumanReward;

            int extra = 0;
            string className = "";

            if (classReward != null)
            {
                extra = classReward.KillHumanBonus ?? 0;
                className = classReward.ClassName;
                reward += extra;
            }
            _service.GivePlayerCredits(attacker, reward, $"{_service.T(attacker, "ReasonKillHuman")}");
            if (extra > 0)
            {
                attacker.SendMessage(MessageType.Chat,$"{_service.T(attacker, "ExtraKillHumanBonus", extra, className)}");
            }
        }
        else if (!attackerZombie && victimZombie)
        {
            var classReward = _service.GetZombieClassReward(player);
            int reward = config.Global.KillZombieReward;

            int extra = 0;
            string className = "";

            if (classReward != null)
            {
                extra = classReward.KilledByHumanBonus ?? 0;
                className = classReward.ClassName;
                reward += extra;
            }
                

            _service.GivePlayerCredits(attacker, reward, $"{_service.T(attacker, "ReasonKillZombie")}");

            if (extra > 0)
            {
                attacker.SendMessage(MessageType.Chat, $"{_service.T(attacker, "ExtraKillZombieBonus", extra, className)}");
            }
        }


        return HookResult.Continue;
    }

}