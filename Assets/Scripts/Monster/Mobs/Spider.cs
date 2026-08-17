public class Spider : Monster
{
    protected override MonsterType Type => MonsterType.spider;
    protected override bool UsesAI => true;
    protected override bool UsesGravity => false;

    protected override void ConfigureStats(int difficulty)
    {
        if (difficulty == 1) SetCombatStats(1, 1f, 5f, 50f);
        else if (difficulty == 2) SetCombatStats(5, 5f, 10f, 100f);
        else SetCombatStats(10, 10f, 20f, 200f);
    }

    protected override void OnDeath()
    {
        SpawnDeathMarkAndDestroy(100);
    }
}
