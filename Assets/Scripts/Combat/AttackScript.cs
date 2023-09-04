using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    PersonnageBase playerBaseStats;
    PersonnageScript playerScript;

    private SkillBase attack;

    public bool recastProtection;

    private float targetHp;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GetComponent<PersonnageScript>();
        playerBaseStats = playerScript.personnage;
    }

    public void Attack(GameObject target)
    {
        attack = CombatManager.Instance.actualPlayerAttack;

        if (target.GetComponent<PersonnageScript>() != null)
        {
            PersonnageScript targetScript = target.GetComponent<PersonnageScript>();


            switch (attack.DamageType)
            {   
                case SkillBase.DamagingType.physical:
                    
                    targetHp = targetScript.actualHp;
                    targetScript.Damaged(AttackDamageCalcul(target, attack));

                    if (attack.SustainEffect)
                    {
                        playerScript.Healed((targetHp - targetScript.actualHp) / 2);
                    }
                    break;

                case SkillBase.DamagingType.special:

                    targetHp = targetScript.actualHp;
                    targetScript.Damaged(AttackDamageCalcul(target, attack));

                    if (attack.SustainEffect)
                    {
                        playerScript.Healed((targetHp - targetScript.actualHp) / 2);
                    }
                    break;

                case SkillBase.DamagingType.heal:

                    if (attack.HealIsPourcentHp)
                    {
                        targetScript.Healed(targetScript.personnage.MaxHp * (attack.Power / 100f));
                    }
                    else
                    {
                        targetScript.Healed((attack.Power * (playerScript.actualSpeAtk + 100) / 100));
                    }
                    break;
            }

            if (!recastProtection || attack.AreaEffectType == SkillBase.AeraType.none)
            {
                playerScript.actualActionPoint -= attack.Cost;
                recastProtection = true;
            }
            attack.CastSpecialEffects(target);
        }
        else if (attack.TargetingType != 1)
        {
            playerScript.actualActionPoint -= attack.Cost;
            attack.CastSpecialEffects(target);
        }

        if (!CombatManager.Instance.isAI)
        {
            CombatManager.Instance.ActualPlayerState = CombatManager.PlayerState.idle;
        }

        if (attack.Cooldown > 0)
        {
            playerScript.attacksActualCooldown[playerScript.actualAttackIndex] = attack.Cooldown;
        }

        CombatManager.Instance.RaisePlayerAttackedEvent();
    }

    public float AttackDamageCalcul(GameObject target, SkillBase chosenAttack)
    {
        float damage;

        if (target.GetComponent<PersonnageScript>() != null)
        {
            PersonnageScript targetScript = target.GetComponent<PersonnageScript>();


            switch (chosenAttack.DamageType)
            {
                case SkillBase.DamagingType.physical:
                    damage = (chosenAttack.Power * (playerScript.actualAtk + 100) / 100) - chosenAttack.Power + playerScript.bonusPhysicalDamageFix;
                    return Mathf.Round((damage - targetScript.bonusPhysicalResistanceFix) * (100f / (targetScript.actualDef + 100f)));

                case SkillBase.DamagingType.special:
                    damage = (chosenAttack.Power * (playerScript.actualSpeAtk + 100) / 100) - chosenAttack.Power + playerScript.bonusSpecialDamageFix;
                    return Mathf.Round((damage - targetScript.bonusSpecialResistanceFix) * (100f / (targetScript.actualSpeDef + 100f)));
            }
        }
        return 0; //default value;
    }
}
