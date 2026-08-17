public class MiniBoss : Monster
{
    protected override MonsterType Type => MonsterType.miniboss;

    protected override void ConfigureStats(int difficulty)
    {
        SetCombatStats(5 * difficulty, 5f * difficulty, 0f, 500f * difficulty);
        miniBossNumMax = 4;
        miniBossNum = miniBossNumMax;
    }

    protected override void OnDeath()
    {
        BossBattleController.GetOrCreate().ReportMiniBossDeath();
        Destroy(gameObject);
    }
}
