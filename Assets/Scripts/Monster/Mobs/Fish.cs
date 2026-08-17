public class Fish : Monster
{
    protected override MonsterType Type => MonsterType.fish;
    protected override bool UsesAI => true;
    protected override bool UsesGravity => false;

    protected override void ConfigureStats(int difficulty)
    {
        SetCombatStats(difficulty, difficulty, 5f * difficulty, 50f * difficulty);
    }

    protected override void OnDeath()
    {
        SpawnDeathMarkAndDestroy(100);
    }
}
