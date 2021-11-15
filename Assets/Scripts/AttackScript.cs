using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public MoveBase[] moveBase;

    public MoveBase chosenMove;

    PersonnageBase playerStats;

    // Start is called before the first frame update
    void Start()
    {
        playerStats = GetComponent<PersonnageBase>();
    }

    public void Attack(PersonnageScript targetScript)
    {
        if (chosenMove.IsPhysical)
        {
            targetScript.physicalDamage = (chosenMove.Ratio * ((playerStats.Atk + 100)/ 100) * chosenMove.Power) + playerStats.bonusPhysicalDamageFix;
        }
        else
        {
            targetScript.specialDamage = (chosenMove.Ratio * ((playerStats.SpeAtk + 100) / 100) * chosenMove.Power) + playerStats.bonusSpecialDamageFix;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
