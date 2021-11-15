using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonnageScript : MonoBehaviour
{
    public PersonnageBase personnage;

    public float physicalDamage;
    public float specialDamage;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damaged()
    {
        if (physicalDamage > personnage.bonusPhysicalResistanceFix)
        {
            personnage.ActualHp -= Mathf.RoundToInt((physicalDamage - personnage.bonusPhysicalResistanceFix) * (100 / (personnage.Def + 100)));
        }

        if (specialDamage > personnage.bonusSpecialResistanceFix)
        {
            personnage.ActualHp -= Mathf.RoundToInt((specialDamage - personnage.bonusSpecialResistanceFix) * (100 / (personnage.SpeDef + 100)));
        }

        physicalDamage = 0;
        specialDamage = 0;
    }
}
