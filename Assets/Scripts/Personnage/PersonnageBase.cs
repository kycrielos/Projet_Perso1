using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Personnage", menuName ="Personnage/Create new personnage")]
public class PersonnageBase : ScriptableObject
{
    [SerializeField] string personnageName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PersonnagePowerType personnagePowerType;

    [SerializeField] float maxHp;
    [SerializeField] float atk;
    [SerializeField] float speAtk;
    [SerializeField] float def;
    [SerializeField] float speDef;
    [SerializeField] float movementPoint;
    [SerializeField] float actionPoint;
    [SerializeField] float initiative;

    public float bonusPhysicalDamageFix;
    public float bonusSpecialDamageFix;

    public float bonusPhysicalResistanceFix;
    public float bonusSpecialResistanceFix;

    [SerializeField] List<LearnableMove> learnableMoves;

    public string PersonnageName
    {
        get { return personnageName; }
    }

    public string Description
    {
        get { return description; }
    }

    public PersonnagePowerType PersonnagePowerType
    {
        get { return personnagePowerType; }
    }

    public float MaxHp
    {
        get { return maxHp; }
    }

    public float Atk
    {
        get { return atk; }
    }

    public float SpeAtk
    {
        get { return speAtk; }
    }

    public float Def
    {
        get { return def; }
    }

    public float SpeDef
    {
        get { return speDef; }
    }

    public float Initiative
    {
        get { return initiative; }
    }

    public float MovementPoint
    {
        get { return movementPoint; }
    }

    public float ActionPoint
    {
        get { return actionPoint; }
    }

    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] SkillBase skillBase;

    public SkillBase Base
    {
        get { return skillBase; }
    }
}

public enum PersonnagePowerType
{
    Light,
    Shadow,
    Nature,
    Cosmic,
    Magic,
    Technology,
    Normal,
}
