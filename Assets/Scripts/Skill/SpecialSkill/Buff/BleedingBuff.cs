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
        target.Damaged(BuffDamageCalcul(target));
    }

    public float BuffDamageCalcul(PersonnageScript target)
    {
        float damage;

        if (target.GetComponent<PlayerScript>() != null)
        {
            switch (damageType)
            {
                case SkillBase.DamagingType.physical:
                    damage = (power * (casterScript.actualAtk + 100) / 100) - power;
                    return Mathf.Round(damage * (100f / (target.actualDef + 100f)));

                case SkillBase.DamagingType.special:
                    damage = (power * (casterScript.actualSpeAtk + 100) / 100) - power;
                    return Mathf.Round(damage * (100f / (target.actualSpeDef + 100f)));
            }
        }
        return 0; //default value;
    }
}
