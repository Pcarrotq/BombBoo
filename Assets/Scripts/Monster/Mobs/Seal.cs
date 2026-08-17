using UnityEngine;

public class SealMonster : Monster
{
    private const float ReleaseRange = 3f;

    protected override MonsterType Type => MonsterType.seal;
    protected override bool UsesAI => true;
    protected override bool StartsSealed => true;

    protected override void ConfigureStats(int difficulty)
    {
        if (difficulty == 1) SetCombatStats(4, 4f, 6f, 20f);
        else if (difficulty == 2) SetCombatStats(9, 9f, 11f, 20f);
        else SetCombatStats(15, 15f, 25f, 20f);
    }

    protected override void TickMonster()
    {
        TryRelease(ReleaseRange);
    }

    protected override void OnDeath()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        Rigidbody body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
    }
}
