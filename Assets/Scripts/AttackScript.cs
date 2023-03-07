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
        attack = GameManager.Instance.actualPlayerAttack;

        if (target.GetComponent<PlayerScript>() != null)
        {
            PersonnageScript targetScript = target.GetComponent<PlayerScript>();


            switch (attack.DamageType)
            {   
                case SkillBase.DamagingType.physical:
                    
                    targetHp = targetScript.actualHp;
                    targetScript.physicalDamage = (attack.Power * (playerScript.actualAtk + 100) / 100) - attack.Power + playerScript.bonusPhysicalDamageFix;
                    targetScript.Damaged();

                    if (attack.SustainEffect)
                    {
                        playerScript.Healed((targetHp - targetScript.actualHp) / 2);
                    }
                    break;

                case SkillBase.DamagingType.special:

                    targetHp = targetScript.actualHp;
                    targetScript.specialDamage = (attack.Power * (playerScript.actualSpeAtk + 100) / 100) - attack.Power + playerScript.bonusSpecialDamageFix;
                    targetScript.Damaged();

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

        GameManager.Instance.ActualPlayerState = GameManager.PlayerState.idle;

        if (attack.Cooldown > 0)
        {
            playerScript.attacksActualCooldown[playerScript.actualAttackIndex] = attack.Cooldown;
        }

        GameManager.Instance.RaisePlayerAttackedEvent();
    }
}
