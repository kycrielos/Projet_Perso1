using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedingBuff : BuffBase
{
    public int power;
    public SkillBase.DamagingType damageType;

    public PersonnageScript casterScript;
    public float casterAtk;
    public float casterSpeAtk;

    public override void TriggerEffects(PersonnageScript target)
    {
        switch (damageType)
        {
            case SkillBase.DamagingType.physical:

                target.physicalDamage = (power * (casterAtk + 100) / 100) - power;
                target.Damaged();
                break;

            case SkillBase.DamagingType.special:
                target.specialDamage = (power * (casterSpeAtk + 100) / 100) - power;
                target.Damaged();
                break;
        }  
    }
}
