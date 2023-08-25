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
        base.StartTurn();
        CombatManager.Instance.ActualPlayerState = CombatManager.PlayerState.isAI;
        AI.StartAI();
    }
}
