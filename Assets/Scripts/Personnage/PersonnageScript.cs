using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class PersonnageScript : MonoBehaviour
{
    public PersonnageBase personnage;

    public bool isAI;

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
    public float bonusPhysicalDamageFix;
    public float bonusSpecialDamageFix;

    public float bonusPM;
    public float bonusPA;


    public List<BuffBase> attachedBuffs = new List<BuffBase>();
    public List<BuffBase> buffsToClear = new List<BuffBase>();
    public List<GlyphBase> possessedGlyph = new List<GlyphBase>();

    bool destroyed;
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
        bonusPhysicalDamageFix = personnage.bonusPhysicalDamageFix;
        bonusSpecialDamageFix = personnage.bonusSpecialDamageFix;

        CombatManager.Instance.playerOrder.Add(gameObject);
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
        if (attachedBuffs.Count != 0)
        {
            foreach (BuffBase buff in attachedBuffs)
            {
                buff.TriggerEffects(this);
                if (destroyed)
                {
                    break;
                }
            }
        }
        if (possessedGlyph.Count != 0)
        {
            foreach (GlyphBase glyph in possessedGlyph)
            {
                glyph.ActualDuration += 1;
            }
        }
    }


    public virtual void EndTurn()
    {
        CombatManager.Instance.ActualPlayerState = CombatManager.PlayerState.idle;
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
        Node playerNode = GridManager.Instance.NodeFromWorldPoint(transform.position); //get player Node
        if (playerNode.glyhpScript != null)
        {
            playerNode.glyhpScript.TriggerEffect(this);
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
            CombatManager.Instance.NextPlayerTurn();
        }
        foreach (BuffBase buffToClear in attachedBuffs)
        {
            Destroy(buffToClear.gameObject, 0.1f);
        }
        foreach (GlyphBase glyph in possessedGlyph)
        {
            Destroy(glyph.gameObject, 0.1f);
        }
        attachedBuffs.Clear();
        possessedGlyph.Clear();
        destroyed = true;
        CombatManager.Instance.RemoveFromIndex(gameObject);
        Destroy(gameObject);
    }
}
