using HanZombiePlagueS2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ShopCore.Contract;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Players;
using SwiftlyS2.Shared.Plugins;


namespace ShopCore;

[PluginMetadata(
    Id = "Shop_HZP_Credits",
    Name = "Shop HZP Credits",
    Author = "H-AN",
    Version = "1.0.0",
    Description = "ShopCore module with Give Credits Compatibility with HZP"
)]


public partial class Shop_HZP_Credits(ISwiftlyCore core) : BasePlugin(core)
{
    private ServiceProvider? ServiceProvider { get; set; }
    private ShopHZPCreditsEvents _Events = null!;
    private ShopHZPCreditsService _Service = null!;
    private IOptionsMonitor<ShopHZPCreditsCFG> _CFG = null!;

    private const string ShopCoreInterfaceKey = "ShopCore.API.v2";
    private const string HanZombiePlagueKey = "HanZombiePlague";

    public static IShopCoreApiV2? _shopApi { get; private set; }
    public static IHanZombiePlagueAPI? _zpApi { get; private set; }

    public override void UseSharedInterface(IInterfaceManager interfaceManager)
    {
        if (!interfaceManager.HasSharedInterface(HanZombiePlagueKey))
        {
            throw new Exception($"[Shop_HZP_Credits] 缺少依赖 {HanZombiePlagueKey} / Missing dependency: {HanZombiePlagueKey}");
        }
        _zpApi = interfaceManager.GetSharedInterface<IHanZombiePlagueAPI>(HanZombiePlagueKey);

        if (_zpApi == null)
        {
            throw new Exception($"[Shop_HZP_Credits] 读取 {HanZombiePlagueKey} API 失败 / Failed to load {HanZombiePlagueKey} API");
        }

        if (!interfaceManager.HasSharedInterface(ShopCoreInterfaceKey))
        {
            throw new Exception($"[Shop_HZP_Credits] 缺少依赖 {ShopCoreInterfaceKey} / Missing dependency: {ShopCoreInterfaceKey}");
        }

        _shopApi = interfaceManager.GetSharedInterface<IShopCoreApiV2>(ShopCoreInterfaceKey);

        if (_shopApi == null)
        {
            throw new Exception($"[Shop_HZP_Credits] 读取 {ShopCoreInterfaceKey} API 失败 / Failed to load {ShopCoreInterfaceKey} API");
        }

        _Service.RegisterZPEvents(_zpApi);
    }

    public override void Load(bool hotReload)
    {
        Core.Configuration.InitializeJsonWithModel<ShopHZPCreditsCFG>("ShopHZPCreditsCFG.jsonc", "ShopHZPCreditsCFG").Configure(builder =>
        {
            builder.AddJsonFile("ShopHZPCreditsCFG.jsonc", false, true);
        });
        var collection = new ServiceCollection();
        collection.AddSwiftly(Core);

        collection
            .AddOptionsWithValidateOnStart<ShopHZPCreditsCFG>()
            .BindConfiguration("ShopHZPCreditsCFG");

        collection.AddSingleton<ShopHZPCreditsGlobals>();
        collection.AddSingleton<ShopHZPCreditsEvents>();
        collection.AddSingleton<ShopHZPCreditsService>();
        ServiceProvider = collection.BuildServiceProvider();

        _Events = ServiceProvider.GetRequiredService<ShopHZPCreditsEvents>();
        _Service = ServiceProvider.GetRequiredService<ShopHZPCreditsService>();
        _CFG = ServiceProvider.GetRequiredService<IOptionsMonitor<ShopHZPCreditsCFG>>();

        _Events.HookEvents();

    }
    public override void Unload()
    {
        ServiceProvider!.Dispose();
    }

}