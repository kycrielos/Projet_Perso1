using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class PersonnageScript : MonoBehaviour
{
    public PersonnageBase personnage;

    public float physicalDamage;
    public float specialDamage;

    public bool playerturn;

    public TMP_Text text;

    public SkillBase[] attackSet;
    public int[] attacksActualCooldown;
    public int actualAttackIndex;

    public float actualHp;
    public float actualAtk;
    public float actualSpeAtk;
    public float actualDef;
    public float actualSpeDef;
    public float actualActionPoint;
    public float actualMovementPoint;

    public float bonusPM;
    public float bonusPA;


    public List<BuffBase> attachedBuffs = new List<BuffBase>();
    public List<BuffBase> buffsToClear = new List<BuffBase>();
    // Start is called before the first frame update
    void Start()
    {
        attacksActualCooldown = new int[4];


        actualHp = personnage.MaxHp;
        actualAtk = personnage.Atk;
        actualSpeAtk = personnage.SpeAtk;
        actualDef = personnage.Def;
        actualSpeDef = personnage.SpeDef;
        actualActionPoint = personnage.ActionPoint;
        actualMovementPoint = personnage.MovementPoint; 

        GameManager.Instance.playerOrder.Add(gameObject);
        text.text = actualHp.ToString() + " HP";
        if (actualHp <= 0)
        {
            Killed();
        }
    }

    public virtual void StartTurn()
    {
        playerturn = true;
        actualActionPoint = personnage.ActionPoint + bonusPA;
        actualMovementPoint = personnage.MovementPoint + bonusPM;
        GridManager.Instance.UpdateGridState();
    }


    public virtual void EndTurn()
    {
        GameManager.Instance.ActualPlayerState = GameManager.PlayerState.idle;
        if (attachedBuffs != null)
        {
            foreach (BuffBase buff in attachedBuffs)
            {
                buffsToClear.Add(buff);
            }
            foreach (BuffBase buff in buffsToClear)
            {
                buff.ClearEffects(this, 1);
            }
            buffsToClear.Clear();
        }
        for (int i = 0; i < 4; i++)
        {
            if (attackSet[i] != null)
            {
                if (attackSet[i].Cooldown > 0)
                {
                    if (attacksActualCooldown[i] > 0)
                    {
                        attacksActualCooldown[i] -= 1;
                    }
                }
            }
        }
        
        playerturn = false;
    }

    public void Healed(float value)
    {
        actualHp += Mathf.Round(value);
        if (actualHp > personnage.MaxHp)
        {
            actualHp = personnage.MaxHp;
        }

        text.text = actualHp.ToString() + " HP";
    }

    public void Damaged()
    {
        if (physicalDamage > personnage.bonusPhysicalResistanceFix)
        {
            actualHp -= Mathf.Round((physicalDamage - personnage.bonusPhysicalResistanceFix) * (100f / (actualDef + 100f)));
        }

        if (specialDamage > personnage.bonusSpecialResistanceFix)
        {
            actualHp -= Mathf.Round((specialDamage - personnage.bonusSpecialResistanceFix) * (100f / (actualSpeDef + 100f)));
        }

        physicalDamage = 0;
        specialDamage = 0;

        text.text = actualHp.ToString() + " HP";

        if (actualHp <= 0)
        {
            actualHp = 0;
            Killed();
        }
    }

    public virtual void Killed()
    {
        if (playerturn)
        {
            GameManager.Instance.NextPlayerTurn();
        }
        GameManager.Instance.RemoveFromIndex(gameObject);
        Destroy(gameObject);
    }
}
