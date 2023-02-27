using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsChange : BuffBase
{
    [SerializeField] Stats[] StatsToChange;
    [SerializeField] int[] Value;
    [SerializeField] bool[] IsPercentage;
    [SerializeField] bool[] IsStatReduction;
        

    public override void AttachEffects(PersonnageScript target)
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

            TriggerEffects(target);
        }
        else
        {
            foreach (BuffBase buff in buffDuplicates)
            {
                buff.actualDuration = MaxDuration;
                Destroy(gameObject);
            }
        }
        buffDuplicates.Clear();
    }

    public override void TriggerEffects(PersonnageScript target)
    {

        for (int i = 0; i < StatsToChange.Length; i++)
        {
            switch (StatsToChange[i])
            {
                case Stats.PA:
                    target.actualActionPoint += Value[i];
                    target.bonusPA += Value[i];
                    break;
                case Stats.PM:
                    target.actualMovementPoint += Value[i];
                    target.bonusPM += Value[i];
                    break;
                case Stats.Atk:
                    target.actualAtk = Calculator(target.actualAtk, Value[i], IsPercentage[i], IsStatReduction[i]);
                    break;
                case Stats.SpeAtk:
                    target.actualSpeAtk = Calculator(target.actualSpeAtk, Value[i], IsPercentage[i], IsStatReduction[i]);
                    break;
                case Stats.Def:
                    target.actualDef = Calculator(target.actualDef, Value[i], IsPercentage[i], IsStatReduction[i]);
                    break;
                case Stats.SpeDef:
                    target.actualSpeDef = Calculator(target.actualSpeDef, Value[i], IsPercentage[i], IsStatReduction[i]);
                    break;
            }
        }
    }

    public override void ClearEffects(PersonnageScript target, int turnToClear)
    {
        actualDuration -= turnToClear;
        if (actualDuration <= 0)
        {
            for (int i = 0; i < StatsToChange.Length; i++)
            {
                switch (StatsToChange[i])
                {
                    case Stats.PA:
                        target.actualActionPoint -= Value[i];
                        target.bonusPA -= Value[i];
                        break;
                    case Stats.PM:
                        target.actualMovementPoint -= Value[i];
                        target.bonusPM -= Value[i];
                        break;
                    case Stats.Atk:
                        target.actualAtk = RevertCalculator(target.actualAtk, Value[i], IsPercentage[i], IsStatReduction[i]);
                        break;
                    case Stats.SpeAtk:
                        target.actualSpeAtk = RevertCalculator(target.actualSpeAtk, Value[i], IsPercentage[i], IsStatReduction[i]);
                        break;
                    case Stats.Def:
                        target.actualDef = RevertCalculator(target.actualDef, Value[i], IsPercentage[i], IsStatReduction[i]);
                        break;
                    case Stats.SpeDef:
                        target.actualSpeDef = RevertCalculator(target.actualSpeDef, Value[i], IsPercentage[i], IsStatReduction[i]);
                        break;
                }
            }
            target.attachedBuffs.Remove(this);
            Destroy(gameObject);
        }

    }

    float Calculator(float baseValue, int value, bool isPercentage, bool isReduction)
    {
        if (isPercentage)
        {
            if (isReduction)
            {
                return baseValue * (1f - value / 100f);
            }
            else
            {
                return baseValue  * (1 + value / 100);
            }
        }
        else
        {
            if (isReduction)
            {
                return baseValue - value;
            }
            else
            {
                return baseValue + value;
            }
        }
    }

    float RevertCalculator(float baseValue, int value, bool isPercentage, bool isReduction)
    {
        if (isPercentage)
        {
            if (isReduction)
            {
                return baseValue / (1f - value / 100f);
            }
            else
            {
                return baseValue / (1f + value / 100f);
            }
        }
        else
        {
            if (isReduction)
            {
                return baseValue + value;
            }
            else
            {
                return baseValue - value;
            }
        }
    }

    public enum Stats
    {
        PA,
        PM,
        Atk,
        SpeAtk,
        Def,
        SpeDef,
    }
}
