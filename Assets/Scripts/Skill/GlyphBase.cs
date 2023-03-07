using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GlyphBase : MonoBehaviour
{
    public int maxDuration;
    private int actualDuration;

    public int ActualDuration
    {
        get { return actualDuration; }
        set
        {
            actualDuration = value;
            if (actualDuration == maxDuration)
            {
                Destroy(gameObject);
            }
        }
    }

    public PersonnageScript caster;

    public abstract void TriggerEffect(PersonnageScript target);
}
