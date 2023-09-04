using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgressiveAI : BaseAI
{
    public PersonnageScript target;
    public SkillBase[] attackSet = new SkillBase[4];

    AttackScript attackScript;
    PersonnageScript mobScript;

    bool allPossibleActionExecuted;

    const int maxExecutionNumber = 300;
    int actualExecutionNumber;

    const int movementSpeed = 5;
    private void Start()
    {
        attackScript = GetComponent<AttackScript>();
        mobScript = GetComponent<PersonnageScript>();
    }

    public override void StartAI()
    {
        allPossibleActionExecuted = false;
        attackSet = mobScript.attackSet;
        for (int i = 0; i < attackSet.Length; i++)
        {
            if (AttackCanBeUsed(i) && (attackSet[i].DamageType == SkillBase.DamagingType.heal || attackSet[i].DamageType == SkillBase.DamagingType.nothing))
            {
                CombatManager.Instance.actualPlayerAttack = attackSet[i];
                CombatManager.Instance.ActualPlayerScript.actualAttackIndex = i;
                attackScript.Attack(gameObject);
            }
        }

        StartCoroutine(ExecuteActions());
    }

    IEnumerator ExecuteActions() 
    {
        while (!allPossibleActionExecuted)
        {
            allPossibleActionExecuted = true;
            actualExecutionNumber += 1;
            foreach (SkillBase attack in attackSet)
            {
                if ((attack.DamageType == SkillBase.DamagingType.physical || attack.DamageType == SkillBase.DamagingType.special) && attack.Cost <= mobScript.actualActionPoint)
                {
                    allPossibleActionExecuted = false;
                }
            }
            StartCoroutine(MoveAndAttack());
            yield return new WaitUntil(() => CombatManager.Instance.ActualPlayerState == CombatManager.PlayerState.idleAI);

            if (actualExecutionNumber > maxExecutionNumber)
            {
                Debug.LogWarning("AI Max Execution Number Reached");
                break;
            }
        }

        CombatManager.Instance.NextPlayerTurn();
    }

    IEnumerator MoveAndAttack()
    {
        yield return new WaitForEndOfFrame();
        int actualMinPathCount = 1000;
        GameObject actualTarget = null;

        float highestDamage = 0;
        SkillBase highestDamageAttack = null;
        GameObject temporaryTarget = null;

        foreach (GameObject player in CombatManager.Instance.playerOrder)
        {
            if (player.GetComponent<PlayerScript>() != null)
            {
                PathFinding.Instance.FindPath(transform.position, player.transform.position);
                foreach (SkillBase attack in attackSet)
                {
                    if ((attack.DamageType == SkillBase.DamagingType.physical || attack.DamageType == SkillBase.DamagingType.special) && attack.Cost <= mobScript.actualActionPoint)
                    {
                        if (player.GetComponent<PlayerScript>().actualHp - attackScript.AttackDamageCalcul(player, attack) <= 0 && CheckRange(player, attack))
                        {
                            if (PathFinding.Instance.finalPath.Count > 0)
                            {
                                StartCoroutine(MovementManager.Instance.MovePersonnage(gameObject, movementSpeed));
                                yield return new WaitUntil(() => CombatManager.Instance.ActualPlayerState == CombatManager.PlayerState.isOnActionAI);
                            }
                            CombatManager.Instance.actualPlayerAttack = attack;
                            attackScript.Attack(player);
                            CombatManager.Instance.ActualPlayerState = CombatManager.PlayerState.idleAI;
                            yield break;
                        }
                    }
                }

                if (actualMinPathCount > PathFinding.Instance.finalPath.Count)
                {
                    actualMinPathCount = PathFinding.Instance.finalPath.Count;
                    actualTarget = player;
                }
                else if (actualMinPathCount == PathFinding.Instance.finalPath.Count)
                {
                    foreach (SkillBase attack in attackSet)
                    {
                        if ((attack.DamageType == SkillBase.DamagingType.physical || attack.DamageType == SkillBase.DamagingType.special) && attack.Cost <= mobScript.actualActionPoint)
                        {
                            if (attackScript.AttackDamageCalcul(player, attack) > attackScript.AttackDamageCalcul(actualTarget, attack) && attackScript.AttackDamageCalcul(player, attack) > highestDamage) 
                            {
                                highestDamage = attackScript.AttackDamageCalcul(player, attack);
                                temporaryTarget = player;
                            }
                            else if (attackScript.AttackDamageCalcul(actualTarget, attack) >= attackScript.AttackDamageCalcul(player, attack) && attackScript.AttackDamageCalcul(actualTarget, attack) > highestDamage)
                            {
                                highestDamage = attackScript.AttackDamageCalcul(actualTarget, attack);
                                temporaryTarget = actualTarget;
                            }
                        }
                    }
                    if (temporaryTarget != null)
                    {
                        actualTarget = temporaryTarget;
                    }
                }
            }
        }

        if (mobScript.actualMovementPoint > 0)
        {
            PathFinding.Instance.FindPath(transform.position, actualTarget.transform.position);
            if (PathFinding.Instance.finalPath.Count > 0)
            {
                StartCoroutine(MovementManager.Instance.MovePersonnage(gameObject, movementSpeed));
                yield return new WaitUntil(() => CombatManager.Instance.ActualPlayerState == CombatManager.PlayerState.isOnActionAI);
            }
        }

        highestDamage = 0;
        foreach (SkillBase attack in attackSet)
        {
            if ((attack.DamageType == SkillBase.DamagingType.physical || attack.DamageType == SkillBase.DamagingType.special) && attack.Cost <= mobScript.actualActionPoint)
            {
                if (attackScript.AttackDamageCalcul(actualTarget, attack) > highestDamage && CheckRange(actualTarget, attack))
                {
                    highestDamage = attackScript.AttackDamageCalcul(actualTarget, attack);
                    highestDamageAttack = attack;
                }
            }
        }

        if (highestDamageAttack != null)
        {
            CombatManager.Instance.actualPlayerAttack = highestDamageAttack;
            attackScript.Attack(actualTarget);
        }
        else if (mobScript.actualMovementPoint <= 0)
        {
            allPossibleActionExecuted = true;
        }

        CombatManager.Instance.ActualPlayerState = CombatManager.PlayerState.idleAI;
    }

    

    bool CheckRange(GameObject targetObj, SkillBase attack) 
    {
        foreach (Node n in GridManager.Instance.grid)
        {
            Node playerNode = GridManager.Instance.NodeFromWorldPoint(targetObj.transform.position); //get player Node
            int actualRange = Mathf.Abs(n.gridX - playerNode.gridX) + Mathf.Abs(n.gridY - playerNode.gridY);
            if (GridManager.Instance.CheckRange(actualRange, attack) && GridManager.Instance.CheckLine(n, playerNode, attack) && NoPhysicSphere(n))
            {
                if (GridManager.Instance.CheckLineOfSight(n, attack))
                {
                    PathFinding.Instance.FindPath(transform.position, n.worldPosition);
                    if (PathFinding.Instance.finalPath.Count <= mobScript.actualMovementPoint) 
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    bool NoPhysicSphere(Node n) 
    {
        if(Physics.CheckSphere(n.nodeObj.transform.position, GridManager.Instance.nodeRadius, GridManager.Instance.unwalkableMask) || Physics.CheckSphere(n.nodeObj.transform.position, GridManager.Instance.nodeRadius, GridManager.Instance.playerMask)) 
        {
            return true;
        }
        return false;
    }


    bool AttackCanBeUsed(int attackIndex) 
    {
        if (mobScript.attacksActualCooldown[attackIndex] <= 0 && attackSet[attackIndex].Cost < CombatManager.Instance.ActualPlayerScript.actualActionPoint)
        {
            return true;
        }
        return false;
    }
}
