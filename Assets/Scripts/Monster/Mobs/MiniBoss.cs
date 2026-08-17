public class MiniBoss : Monster
{
    protected override MonsterType Type => MonsterType.miniboss;

    protected override void ConfigureStats(int difficulty)
    {
        SetCombatStats(5 * difficulty, 5f * difficulty, 0f, 10f);
        miniBossNumMax = 4;
        miniBossNum = miniBossNumMax;
    }

    protected override void OnDeath()
    {
        BossBattleController.GetOrCreate().ReportMiniBossDeath();
        Destroy(gameObject);
    }
}
