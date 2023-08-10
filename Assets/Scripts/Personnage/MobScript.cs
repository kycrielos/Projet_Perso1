using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobScript : PersonnageScript
{
    public BaseAI AI;

    private void Start()
    {
        isAI = true;
    }

    public override void StartTurn() 
    {
        base.StartTurn();
        AI.StartAI();
    }
}
