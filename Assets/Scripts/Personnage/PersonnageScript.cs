using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PersonnageScript : MonoBehaviour
{
    public PersonnageBase personnage;

    public float physicalDamage;
    public float specialDamage;

    public bool playerturn;

    public TMP_Text text;

    public SkillBase[] attackSet;

    public float actualAtk;
    public float actualSpeAtk;
    public float actualDef;
    public float actualSpeDef;
    public float actualActionPoint;
    public float actualMovementPoint;


    public List<BuffBase> attachedBuffs = new List<BuffBase>();
    private List<BuffBase> buffsToClear = new List<BuffBase>();
    // Start is called before the first frame update
    void Start()
    {
        actualAtk = personnage.Atk;
        actualSpeAtk = personnage.SpeAtk;
        actualDef = personnage.Def;
        actualSpeDef = personnage.SpeDef;
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
        GameManager.Instance.ActualPlayerState = GameManager.PlayerState.idle;
        if (attachedBuffs != null)
        {
            foreach(BuffBase buff in attachedBuffs)
            {
                buffsToClear.Add(buff);
            }
            foreach (BuffBase buff in buffsToClear)
            {
                buff.ClearEffects(this, 1);
            }
            buffsToClear.Clear();
        }
        playerturn = false;
    }

    public void Healed(int value)
    {
        personnage.ActualHp += value;
        if (personnage.ActualHp > personnage.MaxHp)
        {
            personnage.ActualHp = personnage.MaxHp;
        }

        text.text = personnage.ActualHp.ToString() + " HP";
    }
    public void Damaged()
    {
        if (physicalDamage > personnage.bonusPhysicalResistanceFix)
        {
            personnage.ActualHp -= Mathf.RoundToInt((physicalDamage - personnage.bonusPhysicalResistanceFix) * (100f / (actualDef + 100f)));
        }

        if (specialDamage > personnage.bonusSpecialResistanceFix)
        {
            personnage.ActualHp -= Mathf.RoundToInt((specialDamage - personnage.bonusSpecialResistanceFix) * (100f / (actualSpeDef + 100f)));
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
        GameManager.Instance.RemoveFromIndex(gameObject);
        Destroy(gameObject);
    }
}
