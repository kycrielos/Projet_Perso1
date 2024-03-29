using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobScript : PersonnageScript
{
    public BaseAI AI;

    public override void Start()
    {
        base.Start();
        isAI = true;
    }

    public override void StartTurn()
    {
        CombatManager.Instance.isAI = true;
        CombatManager.Instance.ActualPlayerState = CombatManager.PlayerState.idleAI;
        base.StartTurn();
        AI.StartAI();
    }
}
