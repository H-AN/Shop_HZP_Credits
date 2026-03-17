public class ShopHZPCreditsCFG
{
    public bool Enabled { get; set; } = true;

    public string InfoCommand { get; set; } = "sw_money";
    public GlobalRewards Global { get; set; } = new();
    public List<ClassReward>? ZombieClassRewards { get; set; } = new();
}

public class GlobalRewards
{
    public int InfectReward { get; set; } = 10;

    public int HumanWinReward { get; set; } = 20;

    public int ZombieWinReward { get; set; } = 5;

    public int KillZombieReward { get; set; } = 5;

    public int KillHumanReward { get; set; } = 5;

    public float PlaytimeIntervalMinutes { get; set; } = 60f;

    public int PlaytimeReward { get; set; } = 1;

    public float DamageThreshold { get; set; } = 5000f;

    public int DamageReward { get; set; } = 1;

    public float DamageTakenThreshold { get; set; } = 3000f;

    public int DamageTakenReward { get; set; } = 1;

}

public class ClassReward
{
    public bool Enabled { get; set; } = true;

    // 丧尸名字
    public string ClassName { get; set; } = "";

    // 此丧尸感染人类额外获取积分
    public int? InfectHumanBonus { get; set; }

    // 丧尸击杀人类额外获取积分
    public int? KillHumanBonus { get; set; }

    // 人类击杀这个丧尸额外获取积分
    public int? KilledByHumanBonus { get; set; }

    // 伤害这个僵尸时积累伤害进度倍率
    public float? DamageMultiplier { get; set; }

    // 此丧尸承受伤害积累进度倍率
    public float? DamageTakenMultiplier { get; set; }
}

