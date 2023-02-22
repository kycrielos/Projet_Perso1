using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Personnage", menuName ="Personnage/Create new personnage")]
public class PersonnageBase : ScriptableObject
{
    [SerializeField] string personnageName;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PersonnageRace personnageRace1;
    [SerializeField] PersonnageRace personnageRace2;

    [SerializeField] PersonnagePowerType personnagePowerType;

    [SerializeField] int maxHp;
    [SerializeField] int actualHp;
    [SerializeField] int atk;
    [SerializeField] int speAtk;
    [SerializeField] int def;
    [SerializeField] int speDef;
    [SerializeField] int movementPoint;
    [SerializeField] int actionPoint;

    public int bonusPhysicalDamageFix;
    public int bonusSpecialDamageFix;

    public int bonusPhysicalResistanceFix;
    public int bonusSpecialResistanceFix;

    [SerializeField] List<LearnableMove> learnableMoves;

    public string PersonnageName
    {
        get { return personnageName; }
    }

    public string Description
    {
        get { return description; }
    }

    public PersonnageRace PersonnageRace1
    {
        get { return personnageRace1; }
    }

    public PersonnageRace PersonnageRace2
    {
        get { return personnageRace2; }
    }

    public PersonnagePowerType PersonnagePowerType
    {
        get { return personnagePowerType; }
    }

    public int MaxHp
    {
        get { return maxHp; }
        set { maxHp = value; }
    }
    public int ActualHp
    {
        get { return actualHp; }
        set { actualHp = value; }
    }

    public int Atk
    {
        get { return atk; }
        set { atk = value; }
    }

    public int SpeAtk
    {
        get { return speAtk; }
        set { speAtk = value; }
    }

    public int Def
    {
        get { return def; }
        set { def = value; }
    }

    public int SpeDef
    {
        get { return speDef; }
        set { speDef = value; }
    }

    public int MovementPoint
    {
        get { return movementPoint; }
        set { movementPoint = value; }
    }

    public int ActionPoint
    {
        get { return actionPoint; }
        set { actionPoint = value; }
    }

    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;

    public MoveBase Base
    {
        get { return moveBase; }
    }
}


public enum PersonnageRace
{
    Human,
    Elf,
    Dwarf,
    Gnome,
    Sylvan,
    Troll,
    Orc,
    Gobelin,
    Ogre,
    Dragon,
    None,
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
