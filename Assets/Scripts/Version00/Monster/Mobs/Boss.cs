using UnityEngine;

public class Boss : Monster
{
    private const float ReleaseRange = 3f;

    private bool hasSplit;

    protected override MonsterType Type => MonsterType.boss;
    protected override bool StartsSealed => true;

    protected override void Start()
    {
        base.Start();
        BossBattleController.GetOrCreate();
    }

    protected override void ConfigureStats(int difficulty)
    {
        SetCombatStats(10 * difficulty, 10f * difficulty, 0f, 40f);
    }

    protected override void OnDeath()
    {
        Time.timeScale = 0f;
    }

    protected override void TickMonster()
    {
        if (!IsReleased)
        {
            if (!IsReleaseAttempt(ReleaseRange)) return;

            if (!BossBattleController.GetOrCreate().AreAllSealsDefeated())
            {
                Player.ShowWarning(
                    "모든 Seal을 봉인 해제하고 처치해야 합니다.",
                    "Release and defeat all Seals first.");
                return;
            }

            TryRelease(ReleaseRange);
            return;
        }

        if (IsReleased && !hasSplit && mCurHP <= mMaxHP * 0.25f)
        {
            hasSplit = true;
            BossBattleController.GetOrCreate().SplitBoss(this);
        }
    }

}
