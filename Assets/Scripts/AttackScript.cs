using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    PersonnageBase playerBaseStats;
    PersonnageScript playerScript;

    private SkillBase attack;

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

            playerScript.actualActionPoint -= attack.Cost;

            if (attack.IsPhysical)
            {
                targetScript.physicalDamage = (attack.Power * (playerScript.actualAtk + 100) / 100) + playerBaseStats.bonusPhysicalDamageFix;
            }
            else
            {
                targetScript.specialDamage = (attack.Power * (playerScript.actualSpeAtk + 100) / 100) + playerBaseStats.bonusSpecialDamageFix;
            }
            targetScript.Damaged();
        }

        attack.CastSpecialEffects(target);
        GameManager.Instance.ActualPlayerState = GameManager.PlayerState.idle;

        if (attack.Cooldown > 0)
        {
            playerScript.attacksActualCooldown[playerScript.actualAttackIndex] = attack.Cooldown;
        }

        GameManager.Instance.RaisePlayerAttackedEvent();
    }
}
