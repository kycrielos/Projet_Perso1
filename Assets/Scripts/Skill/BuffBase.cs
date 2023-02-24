using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffBase : MonoBehaviour
{
    [SerializeField] string buffName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] int maxDuration;
    public int actualDuration;
    [SerializeField] int maxCumul;
    [SerializeField] bool clearable;

    public string BuffName
    {
        get { return buffName; }
    }

    public int MaxDuration
    {
        get { return maxDuration; }
    }
    public int MaxCumul
    {
        get { return maxCumul; }
    }
    public bool Clearable
    {
        get { return clearable; }
    }

    public virtual void AttachEffects(PersonnageScript target)
    {
        actualDuration = MaxDuration;
        List<BuffBase> buffDuplicates = new List<BuffBase>();
        foreach (BuffBase buff in target.attachedBuffs)
        {
            if (buff.BuffName == BuffName)
            {
                buffDuplicates.Add(buff);
            }
        }
        if (buffDuplicates.Count < MaxCumul)
        {
            target.attachedBuffs.Add(this);
        }
        else
        {
            foreach(BuffBase buff in buffDuplicates)
            {
                buff.actualDuration = MaxDuration;
                Destroy(gameObject);
            }
        }
        buffDuplicates.Clear();
    }

    public abstract void TriggerEffects(PersonnageScript target);

    public virtual void ClearEffects(PersonnageScript target, int turnToClear)
    {
        actualDuration -= turnToClear;
        if (actualDuration <= 0)
        {
            target.attachedBuffs.Remove(this);
            Destroy(gameObject);
        }
    }
}
