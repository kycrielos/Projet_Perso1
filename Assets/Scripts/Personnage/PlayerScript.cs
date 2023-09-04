using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : PersonnageScript
{
    public override void StartTurn()
    {
        CombatManager.Instance.isAI = false;
        CombatManager.Instance.ActualPlayerState = CombatManager.PlayerState.idle;
        base.StartTurn();
    }
    public override void EndTurn()
    {
        CombatManager.Instance.ActualPlayerState = CombatManager.PlayerState.idle;
        base.EndTurn();
    }
}
