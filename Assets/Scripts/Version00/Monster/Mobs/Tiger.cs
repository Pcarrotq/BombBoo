public class Tiger : Monster
{
    protected override MonsterType Type => MonsterType.tiger;
    protected override bool UsesAI => true;
    protected override float ChaseMoveSpeed => 4f;
    protected override float IdleMoveSpeed => 0.5f;

    protected override void ConfigureStats(int difficulty)
    {
        SetCombatStats(5 * difficulty, 5f * difficulty, 10f * difficulty, 200f * difficulty);
    }

    protected override void OnDeath()
    {
        SpawnDeathMarkAndDestroy(200, 100);
    }
}
