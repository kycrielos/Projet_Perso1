using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingGlyph : GlyphBase
{
    public bool isPercentHp;
    public float power;

    public override void TriggerEffect(PersonnageScript target)
    {
        if (isPercentHp)
        {
            target.Healed(target.personnage.MaxHp * (power / 100f));
        }
        else
        {
            target.Healed(power * (caster.actualSpeAtk + 100) / 100);
        }
    }
}
