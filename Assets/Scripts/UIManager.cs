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
        GameManager.StartTurnEvent += TurnStart;

        StartCoroutine(LateStart(0.01f));
    }
    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        TurnStart();
    }
    public void EndTurn()
    {
        GameManager.Instance.NextPlayerTurn();
    }

    public void UIAttackClick(int buttonNumber)
    {
        if (GameManager.Instance.ActualPlayerState == GameManager.PlayerState.idle)
        {
            GameManager.Instance.actualPlayerAttack = attackSet[buttonNumber];
            GameManager.Instance.ActualPlayerState = GameManager.PlayerState.isTargeting;
        }
    }

    public void TurnStart()
    {
        attackSet = GameManager.Instance.ActualPlayerScript.attackSet;
        for (int i = 0; i < 4; i++)
        {
            if (attackSet[i] != null)
            {
                attackSetButton[i].GetComponentInChildren<TMP_Text>().text = attackSet[i].name;
            }
            else
            {
                attackSetButton[i].GetComponentInChildren<TMP_Text>().text = null;
            }
        }
    }

    private void Update()
    {
        paText.text = "Pa : " + GameManager.Instance.ActualPlayerScript.actualActionPoint.ToString();
        pmText.text = "Pm : " + GameManager.Instance.ActualPlayerScript.actualMovementPoint.ToString();
    }

    ~UIManager()
    {
        GameManager.StartTurnEvent -= TurnStart;
    }
}
