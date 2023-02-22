using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PersonnageScript : MonoBehaviour
{
    public PersonnageBase personnage;

    public int actualActionPoint;
    public int actualMovementPoint;

    public float physicalDamage;
    public float specialDamage;

    public bool playerturn;

    public TMP_Text text;

    public SkillBase[] attackSet;

    public List<BuffBase> attachedBuffs = new List<BuffBase>();
    // Start is called before the first frame update
    void Start()
    {
        actualActionPoint = personnage.ActionPoint;
        actualMovementPoint = personnage.MovementPoint;
        GameManager.Instance.playerOrder.Add(gameObject);
        text.text = personnage.ActualHp.ToString() + " HP";
        if (personnage.ActualHp <= 0)
        {
            Killed();
        }
    }

    public void StartTurn()
    {
        playerturn = true;
        GridManager.Instance.UpdateGridState();
    }

    public void EndTurn()
    {
        actualActionPoint = personnage.ActionPoint;
        actualMovementPoint = personnage.MovementPoint;
        playerturn = false;
    }

    public void Damaged()
    {
        if (physicalDamage > personnage.bonusPhysicalResistanceFix)
        {
            personnage.ActualHp -= Mathf.RoundToInt((physicalDamage - personnage.bonusPhysicalResistanceFix) * (100f / (personnage.Def + 100f)));
        }

        if (specialDamage > personnage.bonusSpecialResistanceFix)
        {
            personnage.ActualHp -= Mathf.RoundToInt((specialDamage - personnage.bonusSpecialResistanceFix) * (100f / (personnage.SpeDef + 100f)));
        }

        physicalDamage = 0;
        specialDamage = 0;

        text.text = personnage.ActualHp.ToString() + " HP";

        if (personnage.ActualHp <= 0)
        {
            Killed();
        }
    }

    public void Killed()
    {
        if (playerturn)
        {
            GameManager.Instance.NextPlayerTurn();
        }
        GameManager.Instance.playerOrder.Remove(gameObject);
        Destroy(gameObject);
    }
}
