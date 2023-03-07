using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ClickDetector : MonoBehaviour
{
    public GameObject player;

    public PersonnageScript playerScript;

    AttackScript playerAttack;
    GameObject attackTarget;

    public float speed;

    public LayerMask mask;

    Node clickedObjNode;

    List<Node> area = new List<Node>();

    List<GameObject> areaTarget = new List<GameObject>();

    private void Start()
    {
        player = GameManager.Instance.ActualPlayer;
        playerAttack = player.GetComponent<AttackScript>();
        playerScript = player.GetComponent<PersonnageScript>();
    }

    void Update()
    {
        if (player != GameManager.Instance.ActualPlayer)
        {
            player = GameManager.Instance.ActualPlayer;
            playerAttack = player.GetComponent<AttackScript>();
            playerScript = player.GetComponent<PersonnageScript>();
        }
        switch (GameManager.Instance.ActualPlayerState)
        {
            //if the player is in idle it will check for the movement possibilities
            case GameManager.PlayerState.idle:
                if (GetClickedGameObject() != null) //safety (called after the case to minimise runtime)
                {
                    //setup the pathfinding
                    PathFinding.Instance.target = GetClickedGameObject().transform;

                    //Start the player movement coroutine if the player click on a valid node
                    if (Input.GetMouseButtonDown(0) && MovementManager.Instance.PathCheck(GetClickedGameObject().transform.position))
                    {
                        GameManager.Instance.ActualPlayerState = GameManager.PlayerState.isMoving;
                        StartCoroutine(MovementManager.Instance.MovePersonnage(player, speed));
                    }
                }
                break;

            //if the player is targeting something with a move it will check for the available target
            case GameManager.PlayerState.isTargeting:
                if (GetClickedGameObject() != null) //safety (called after the case to minimise runtime)
                {
                    //set the last target node to false
                    if (clickedObjNode != null)
                    {
                        clickedObjNode.IsTarget = false;
                    }

                    if (GameManager.Instance.actualPlayerAttack.AreaEffectType != SkillBase.AeraType.none)
                    {
                        if (area != null)
                        {
                            foreach (Node n in area)
                            {
                                n.AreaTarget= false;
                            }
                            area.Clear();
                            areaTarget.Clear();
                        }
                    }

                    //get the new target node
                    attackTarget = GetClickedGameObject();
                    clickedObjNode = GridManager.Instance.NodeFromWorldPoint(attackTarget.transform.position);
                    clickedObjNode.IsTarget = true;

                    if (GameManager.Instance.actualPlayerAttack.AreaEffectType != SkillBase.AeraType.none)
                    {
                        AreaCheck();
                    }

                    //Check if the player click on a valid node if not return the player to idle
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (TargetCheck())
                        {
                            if (GameManager.Instance.actualPlayerAttack.AreaEffectType != SkillBase.AeraType.none)
                            {
                                playerAttack.recastProtection = false;
                                foreach (GameObject obj in areaTarget)
                                {
                                    playerAttack.Attack(obj);
                                }
                                GameManager.Instance.ActualPlayerState = GameManager.PlayerState.idle;
                            }
                            else if (clickedObjNode.player != null)
                            {
                                playerAttack.Attack(clickedObjNode.player);
                            }
                            else
                            {
                                playerAttack.Attack(clickedObjNode.nodeObj);
                            }
                        }
                        else
                        {
                            clickedObjNode.IsTarget = false;
                            GameManager.Instance.ActualPlayerState = GameManager.PlayerState.idle;
                        }
                        GridManager.Instance.UpdateGridState();
                    }
                }
                break;
        }
    }

    bool TargetCheck()
    {
        //return false if the node can't be a target at all
        if (clickedObjNode.GroundState == GroundStateEnum.nothing || clickedObjNode.GroundState == GroundStateEnum.wall)
        {
            return false;
        }

        // return false if the  node can't be a target for the player actual attack
        switch (GameManager.Instance.actualPlayerAttack.TargetingType) //0 = everything can be target, 1 = square with a target on it only, 2 = empty square only
        {
            case 0:
                if (clickedObjNode.GroundState != GroundStateEnum.targetablePlayer && clickedObjNode.GroundState != GroundStateEnum.targetable)
                {
                    return false;
                }
                break;
            case 1:
                if (clickedObjNode.GroundState != GroundStateEnum.targetablePlayer)
                {
                    return false;
                }
                break;
            case 2:
                if (clickedObjNode.GroundState != GroundStateEnum.targetable)
                {
                    return false;
                }
                break;
        }

        Node playerNode = GridManager.Instance.NodeFromWorldPoint(player.transform.position); //get player Node

        //Attack maximum range calcul
        int actualRange = Mathf.Abs(clickedObjNode.gridX - playerNode.gridX) + Mathf.Abs(clickedObjNode.gridY - playerNode.gridY);
        if (actualRange <= GameManager.Instance.actualPlayerAttack.Range && actualRange >= GameManager.Instance.actualPlayerAttack.MinimumRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AreaCheck()
    {

        if (TargetCheck())
        {
            Node playerNode = GridManager.Instance.NodeFromWorldPoint(player.transform.position); //get player Node
            List<Node> neighbours = new List<Node>();

            area.Add(clickedObjNode);
            switch (GameManager.Instance.actualPlayerAttack.AreaEffectType)
            {
                case SkillBase.AeraType.cross:
                    for (int i = 0; i < GameManager.Instance.actualPlayerAttack.AreaSize; i++)
                    {
                        foreach (Node n in area)
                        {
                            foreach (Node n2 in GridManager.Instance.GetNeighbours(n))
                            {
                                if (!neighbours.Contains(n2) && (n2.gridX == clickedObjNode.gridX || n2.gridY == clickedObjNode.gridY))
                                {
                                    neighbours.Add(n2);
                                    n2.AreaTarget = true;
                                    if (n2.player != null && !(n2 == playerNode && !GameManager.Instance.actualPlayerAttack.AreaAffectPlayer))
                                    {
                                        areaTarget.Add(n2.player);
                                    }
                                }
                            }
                        }

                        foreach (Node n in neighbours)
                        {
                            if (!area.Contains(n))
                            {
                                area.Add(n);
                            }
                        }
                    }
                    break;
                case SkillBase.AeraType.star:
                    for (int i = 0; i < GameManager.Instance.actualPlayerAttack.AreaSize; i++)
                    {
                        foreach (Node n in area)
                        {
                            foreach (Node n2 in GridManager.Instance.GetNeighbours(n))
                            {
                                if (!neighbours.Contains(n2))
                                {
                                    neighbours.Add(n2);
                                    n2.AreaTarget = true;
                                    if (n2.player != null && !(n2 == playerNode && !GameManager.Instance.actualPlayerAttack.AreaAffectPlayer))
                                    {
                                        areaTarget.Add(n2.player);
                                    }
                                }
                            }
                        }

                        foreach (Node n in neighbours)
                        {
                            if (!area.Contains(n))
                            {
                                area.Add(n);
                            }
                        }
                    }
                    break;
                case SkillBase.AeraType.square:
                    for (int i = 0; i < GameManager.Instance.actualPlayerAttack.AreaSize * 2; i++)
                    {
                        foreach (Node n in area)
                        {
                            foreach (Node n2 in GridManager.Instance.GetNeighbours(n))
                            {
                                if (!neighbours.Contains(n2))
                                {
                                    if (Mathf.Abs(n2.gridX - clickedObjNode.gridX) <= GameManager.Instance.actualPlayerAttack.AreaSize && Mathf.Abs(n2.gridY - clickedObjNode.gridY) <= GameManager.Instance.actualPlayerAttack.AreaSize)
                                    {
                                        neighbours.Add(n2);
                                        n2.AreaTarget = true;
                                        if (n2.player != null && !(n2 == playerNode && !GameManager.Instance.actualPlayerAttack.AreaAffectPlayer))
                                        {
                                            areaTarget.Add(n2.player);
                                        }
                                    }
                                }
                            }
                        }

                        foreach (Node n in neighbours)
                        {
                            if (!area.Contains(n))
                            {
                                area.Add(n);
                            }
                        }
                    }
                    break;
                case SkillBase.AeraType.diagonal:
                    for (int i = 0; i < GameManager.Instance.actualPlayerAttack.AreaSize * 2; i++)
                    {
                        foreach (Node n in area)
                        {
                            foreach (Node n2 in GridManager.Instance.GetNeighbours(n))
                            {
                                if (!neighbours.Contains(n2))
                                {
                                    neighbours.Add(n2);
                                    if (Mathf.Abs(n2.gridX - clickedObjNode.gridX) == Mathf.Abs(n2.gridY - clickedObjNode.gridY))
                                    {
                                        n2.AreaTarget = true;
                                        if (n2.player != null && !(n2 == playerNode && !GameManager.Instance.actualPlayerAttack.AreaAffectPlayer))
                                        {
                                            areaTarget.Add(n2.player);
                                        }
                                    }
                                }
                            }
                        }

                        foreach (Node n in neighbours)
                        {
                            if (!area.Contains(n))
                            {
                                area.Add(n);
                            }
                        }
                    }
                    break;
            }
        }
    }


    //Use a Raycast from the mouse to get the target node
    GameObject GetClickedGameObject() 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            return hit.transform.gameObject;
        }
        else
        {
            return null;
        }
    }
}

