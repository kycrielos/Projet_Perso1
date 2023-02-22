using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public MoveBase[] moveBase;

    public MoveBase chosenMove;

    PersonnageBase playerBaseStats;
    PersonnageScript playerScript;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = GetComponent<PersonnageScript>();
    }

    public void Attack(PersonnageScript targetScript)
    {
        if (playerScript.actualActionPoint >= chosenMove.Cost)
        {
            playerScript.actualActionPoint -= chosenMove.Cost;
            if (chosenMove.IsPhysical)
            {
                targetScript.physicalDamage = (chosenMove.Ratio * ((playerBaseStats.Atk + 100) / 100) * chosenMove.Power) + playerBaseStats.bonusPhysicalDamageFix;
            }
            else
            {
                targetScript.specialDamage = (chosenMove.Ratio * ((playerBaseStats.SpeAtk + 100) / 100) * chosenMove.Power) + playerBaseStats.bonusSpecialDamageFix;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
