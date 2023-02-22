using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "Buff/Create new Buff")]
public class BuffBase : ScriptableObject
{
    [SerializeField] string buffName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] int maxDuration;
    [SerializeField] int actualDuration;
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
    public int ActualDuration
    {
        get { return actualDuration; }
        set { actualDuration = value; }
    }
    public int MaxCumul
    {
        get { return maxCumul; }
    }
    public bool Clearable
    {
        get { return clearable; }
    }

    protected virtual void AttachEffects(PersonnageScript target)
    {
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
                buff.ActualDuration = maxDuration;
            }
        }
        buffDuplicates.Clear();
    }

    protected virtual void TriggerEffects(PersonnageScript target)
    {

    }

    protected virtual void ClearEffects(PersonnageScript target, int turnToClear)
    {
        actualDuration -= turnToClear;
        if (actualDuration <= 0)
        {
            target.attachedBuffs.Remove(this);
        }
    }
}
