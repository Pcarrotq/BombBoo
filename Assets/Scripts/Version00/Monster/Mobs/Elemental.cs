public class Elemental : Monster
{
    protected override MonsterType Type => MonsterType.elemental;
    protected override bool UsesAI => true;

    protected override void ConfigureStats(int difficulty)
    {
        SetCombatStats(difficulty, difficulty, 4f * difficulty, 25f * difficulty);
    }

    protected override void OnDeath()
    {
        SpawnDeathMarkAndDestroy(25, 10);
    }
}
