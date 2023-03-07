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


            for (int i = 0; i < StatsToChange.Length; i++)
            {
                switch (StatsToChange[i])
                {
                    case Stats.PA:
                        if (IsStatReduction[i])
                        {
                            target.actualActionPoint -= Value[i];
                            target.bonusPA -= Value[i];
                        }
                        else
                        {
                            target.actualActionPoint += Value[i];
                            target.bonusPA += Value[i];
                        }
                        break;
                    case Stats.PM:
                        if (IsStatReduction[i])
                        {
                            target.actualMovementPoint -= Value[i];
                            target.bonusPM -= Value[i];
                        }
                        else
                        {
                            target.actualMovementPoint += Value[i];
                            target.bonusPM += Value[i];
                        }
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
                    case Stats.bonusPhysicalDamageFix:
                        target.bonusPhysicalDamageFix = Calculator(target.bonusPhysicalDamageFix, Value[i], IsPercentage[i], IsStatReduction[i]);
                        break;
                    case Stats.bonusSpecialDamageFix:
                        target.bonusSpecialDamageFix = Calculator(target.bonusSpecialDamageFix, Value[i], IsPercentage[i], IsStatReduction[i]);
                        break;
                }
            }
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
                        if (IsStatReduction[i])
                        {
                            target.actualActionPoint += Value[i];
                            target.bonusPA += Value[i];
                        }
                        else
                        {
                            target.actualActionPoint -= Value[i];
                            target.bonusPA -= Value[i];
                        }
                        break;
                    case Stats.PM:
                        if (IsStatReduction[i])
                        {
                            target.actualMovementPoint += Value[i];
                            target.bonusPM += Value[i];
                        }
                        else
                        {
                            target.actualMovementPoint -= Value[i];
                            target.bonusPM -= Value[i];
                        }
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
                    case Stats.bonusPhysicalDamageFix:
                        target.bonusPhysicalDamageFix = RevertCalculator(target.bonusPhysicalDamageFix, Value[i], IsPercentage[i], IsStatReduction[i]);
                        break;
                    case Stats.bonusSpecialDamageFix:
                        target.bonusSpecialDamageFix = RevertCalculator(target.bonusSpecialDamageFix, Value[i], IsPercentage[i], IsStatReduction[i]);
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
                return baseValue  * (1f + value / 100f);
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
        bonusPhysicalDamageFix,
        bonusSpecialDamageFix,
    }
}
