using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Skill/Create new skill")]
public class SkillBase : ScriptableObject
{
    [SerializeField] string skillName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PersonnagePowerType powerType;
    [SerializeField] int power;
    [SerializeField] int range;
    [SerializeField] int minimumRange;
    [SerializeField] int cost;
    [SerializeField] int cooldown;

    [SerializeField] bool lineOfSight;
    [SerializeField] bool inLineOnly;

    [Tooltip("0 = everything can be target, 1 = square with a target on it only, 2 = empty square only")]
    [SerializeField] int targetingType;
    [SerializeField] bool isPhysical;


    [SerializeField] SpecialEffect effect;

    public string SkillName
    {
        get { return skillName; }
    }

    public string Description
    {
        get { return description; }
    }

    public PersonnagePowerType PowerType
    {
        get { return powerType; }
    }

    public int Power
    {
        get { return power; }
    }

    public int Range
    {
        get { return range; }
    }
    public int MinimumRange
    {
        get { return minimumRange; }
    }

    public int Cost
    {
        get { return cost; }
    }
    public int Cooldown
    {
        get { return cooldown; }
    }

    public bool LineOfSight
    {
        get { return lineOfSight; }
    }
    public bool InLineOnly
    {
        get { return inLineOnly; }
    }

    public int TargetingType
    {
        get { return targetingType; }
    }

    public bool IsPhysical
    {
        get { return isPhysical; }
    }

    public void CastSpecialEffects(GameObject target)
    {
        if (effect != null)
        {
            effect.Init(target);
        }
    }

}
