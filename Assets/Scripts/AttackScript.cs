using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public SkillBase[] skillBase;

    PersonnageBase playerBaseStats;
    PersonnageScript playerScript;

    private SkillBase attack;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GetComponent<PersonnageScript>();
        playerBaseStats = playerScript.personnage;
    }

    public void Attack(PersonnageScript targetScript)
    {
        attack = GameManager.Instance.actualPlayerAttack;
        if (playerScript.actualActionPoint >= attack.Cost)
        {
            playerScript.actualActionPoint -= attack.Cost;
            if (attack.IsPhysical)
            {
                targetScript.physicalDamage = (attack.Power * (playerBaseStats.Atk + 100) / 100) + playerBaseStats.bonusPhysicalDamageFix;
            }
            else
            {
                targetScript.specialDamage = (attack.Power * (playerBaseStats.SpeAtk + 100) / 100) + playerBaseStats.bonusSpecialDamageFix;
                targetScript.Damaged();
                GameManager.Instance.actualPlayerState = GameManager.PlayerState.idle;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
