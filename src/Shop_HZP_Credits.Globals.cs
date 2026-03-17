public class ShopHZPCreditsGlobals
{
    public required Dictionary<int, PlayerRewardState> PlayerStates { get; set; }
    public required Dictionary<int, CancellationTokenSource> PlaytimeTasks { get; set; }

    public ShopHZPCreditsGlobals()
    {
        PlayerStates = new Dictionary<int, PlayerRewardState>();
        PlaytimeTasks = new Dictionary<int, CancellationTokenSource>();
    }

    public class PlayerRewardState
    {
        public float PlayTimeMinutes;
        public float DamageDealt;
        public float DamageTaken;
    }
}