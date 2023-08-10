using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TMP_Text paText;
    public TMP_Text pmText;

    public SkillBase[] attackSet = new SkillBase[4];
    public Button[] attackSetButton;

    private void Start()
    {
        CombatManager.StartTurnEvent += TurnStart;
        CombatManager.PlayerAttackedEvent += UpdateAttackSetButtonState;

        //StartCoroutine(LateStart(0.03f));
    }
    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        TurnStart();
    }
    public void EndTurn()
    {
        CombatManager.Instance.NextPlayerTurn();
    }

    public void UIAttackClick(int buttonNumber)
    {
        if (CombatManager.Instance.ActualPlayerState == CombatManager.PlayerState.idle)
        {
            CombatManager.Instance.actualPlayerAttack = attackSet[buttonNumber];
            CombatManager.Instance.ActualPlayerScript.actualAttackIndex = buttonNumber;
            CombatManager.Instance.ActualPlayerState = CombatManager.PlayerState.isTargeting;
        }
    }

    public void TurnStart()
    {
        attackSet = CombatManager.Instance.ActualPlayerScript.attackSet;
        for (int i = 0; i < 4; i++)
        {
            if (attackSet[i] != null)
            {
                attackSetButton[i].GetComponentInChildren<TMP_Text>().text = attackSet[i].SkillName;
            }
            else
            {
                attackSetButton[i].GetComponentInChildren<TMP_Text>().text = null;
            }
        }
        UpdateAttackSetButtonState();
    }

    private void LateUpdate()
    {
        paText.text = "Pa : " + CombatManager.Instance.ActualPlayerScript.actualActionPoint.ToString();
        pmText.text = "Pm : " + CombatManager.Instance.ActualPlayerScript.actualMovementPoint.ToString();
    }

    private void UpdateAttackSetButtonState()
    {
        for (int i = 0; i < 4; i++)
        {
            if (attackSet[i] != null)
            {
                if (attackSet[i].Cost > CombatManager.Instance.ActualPlayerScript.actualActionPoint)
                {
                    attackSetButton[i].enabled = false;
                    attackSetButton[i].GetComponent<Image>().color = Color.gray;
                }
                else if (attackSet[i].Cooldown > 0)
                {
                    if (CombatManager.Instance.ActualPlayerScript.attacksActualCooldown[i] > 0)
                    {
                        attackSetButton[i].enabled = false;
                        attackSetButton[i].GetComponent<Image>().color = Color.gray;
                    }
                    else
                    {
                        attackSetButton[i].enabled = true;
                        attackSetButton[i].GetComponent<Image>().color = Color.white;
                    }
                }
                else
                {
                    attackSetButton[i].enabled = true;
                    attackSetButton[i].GetComponent<Image>().color = Color.white;
                }
            }
            else
            {
                attackSetButton[i].enabled = false;
                attackSetButton[i].GetComponent<Image>().color = Color.gray;
            }
        }
    }

    ~UIManager()
    {
        CombatManager.StartTurnEvent -= TurnStart;
    }
}
