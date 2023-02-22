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

    public List<BuffBase> attachedBuffs = new List<BuffBase>();
    // Start is called before the first frame update
    void Start()
    {
        actualActionPoint = personnage.ActionPoint;
        actualMovementPoint = personnage.MovementPoint;
        GameManager.Instance.playerOrder.Add(gameObject);
        text.text = personnage.ActualHp.ToString() + " HP";
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
        GameManager.Instance.NextPlayerTurn();
    }

    public void Damaged()
    {
        if (physicalDamage > personnage.bonusPhysicalResistanceFix)
        {
            personnage.ActualHp -= Mathf.RoundToInt((physicalDamage - personnage.bonusPhysicalResistanceFix) * (100 / (personnage.Def + 100)));
        }

        if (specialDamage > personnage.bonusSpecialResistanceFix)
        {
            personnage.ActualHp -= Mathf.RoundToInt((specialDamage - personnage.bonusSpecialResistanceFix) * (100 / (personnage.SpeDef + 100)));
        }

        physicalDamage = 0;
        specialDamage = 0;

        text.text = personnage.ActualHp.ToString() + " HP";
    }
}
