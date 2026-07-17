using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    private int pAttackSkillNum;
    public int[] pAttackSkillNums;
    public List<int> pAttackSkills;
    
    [SerializeField] private UIGameScore uiGameScore;

    // Start is called before the first frame update
    void Start()
    {
        pAttackSkillNums = new int[3];
        pAttackSkills = new List<int>();

        for (int i = 1; i <= 10; i++)
        {
            pAttackSkills.Add(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttackSkill()
    {
        for (int i = 0; i < pAttackSkills.Count; i++)
        {
            int randomSkill = Random.Range(i, pAttackSkills.Count);

            pAttackSkillNum = pAttackSkills[i];
            pAttackSkills[i] = pAttackSkills[randomSkill];
            pAttackSkills[randomSkill] = pAttackSkillNum;
        }

        for (int i = 0; i < 3; i++)
        {
            pAttackSkillNums[i] = pAttackSkills[i];
            Debug.Log($"현재 스킬 {i + 1} : {pAttackSkillNums[i]}");
        }

        uiGameScore?.SkillButtons();
    }

    public void UseSkill(int skillNumber)
    {
        switch (skillNumber)
        {
            case 1:
                Debug.Log("스킬 1");
                break;
            case 2:
                Debug.Log("스킬 2");
                break;
            case 3:
                Debug.Log("스킬 3");
                break;
            case 4:
                Debug.Log("스킬 4");
                break;
            case 5:
                Debug.Log("스킬 5");
                break;
            case 6:
                Debug.Log("스킬 6");
                break;
            case 7:
                Debug.Log("스킬 7");
                break;
            case 8:
                Debug.Log("스킬 8");
                break;
            case 9:
                Debug.Log("스킬 9");
                break;
            case 10:
                Debug.Log("스킬 10");
                break;
        }
    }
}
