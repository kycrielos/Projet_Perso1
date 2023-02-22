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
    
    [SerializeField] bool lineOfSight;

    [Tooltip("0 = everything can be target, 1 = square with a target on it only, 2 = empty square only")]
    [SerializeField] int targetingType;
    [SerializeField] bool isPhysical;

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

    public bool LineOfSight
    {
        get { return lineOfSight; }
    }

    public int TargetingType
    {
        get { return targetingType; }
    }

    public bool IsPhysical
    {
        get { return isPhysical; }
    }

    protected virtual void CastSpecialEffects(PersonnageScript target)
    {

    }

    protected void Ralstats(int value, string designatedStats, PersonnageBase target)
    {
        switch (designatedStats)
        {
            case "atk":
                target.Atk -= value;
                break;
            case "speAtk":
                target.SpeAtk -= value;
                break;
            case "def":
                target.Def -= value;
                break;
            case "speDef":
                target.SpeDef -= value;
                break;
            case "movementPoint":
                target.MovementPoint -= value;
                break;
            case "actionPoint":
                target.ActionPoint -= value;
                break;
        }
    }

}
