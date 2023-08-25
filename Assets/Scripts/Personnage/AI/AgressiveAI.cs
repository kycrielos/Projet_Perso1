using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgressiveAI : BaseAI
{
    public PersonnageScript target;
    public SkillBase[] attackSet = new SkillBase[4];

    AttackScript attackScript;
    PersonnageScript mobScript;

    private void Start()
    {
        attackScript = GetComponent<AttackScript>();
        mobScript = GetComponent<PersonnageScript>();
    }

    public override void StartAI()
    {
        attackSet = mobScript.attackSet;
        for (int i = 0; i < attackSet.Length; i++)
        {
            if (AttackCanBeUsed(i) && (attackSet[i].DamageType == SkillBase.DamagingType.heal || attackSet[i].DamageType == SkillBase.DamagingType.nothing))
            {
                CombatManager.Instance.actualPlayerAttack = attackSet[i];
                CombatManager.Instance.ActualPlayerScript.actualAttackIndex = i;
                attackScript.Attack(gameObject);
            }
        }

        CombatManager.Instance.NextPlayerTurn();
    }

    bool AttackCanBeUsed(int attackIndex) 
    {
        if (mobScript.attacksActualCooldown[attackIndex] <= 0 && attackSet[attackIndex].Cost < CombatManager.Instance.ActualPlayerScript.actualActionPoint)
        {
            return true;
        }
        return false;
    }
}
