using UnityEngine;
using System.Collections;
public class ClickDetector : MonoBehaviour
{
    public GameObject player;

    public PersonnageScript playerScript;

    AttackScript playerAttack;
    GameObject attackTarget;

    public float speed;

    public LayerMask mask;

    Node clickedObjNode;

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
        switch (GameManager.Instance.actualPlayerState)
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
                        GameManager.Instance.actualPlayerState = GameManager.PlayerState.isMoving;
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

                    //get the new target node
                    attackTarget = GetClickedGameObject();
                    clickedObjNode = GridManager.Instance.NodeFromWorldPoint(attackTarget.transform.position);
                    clickedObjNode.IsTarget = true;

                    //Check if the player click on a valid node if not return the player to idle
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (targetCheck())
                        {
                            playerAttack.Attack(clickedObjNode.player.GetComponent<PersonnageScript>());
                        }
                        else
                        {
                            clickedObjNode.IsTarget = false;
                            GameManager.Instance.actualPlayerState = GameManager.PlayerState.idle;
                        }
                        GridManager.Instance.UpdateGridState();
                    }
                }
                break;
        }
    }

    bool targetCheck()
    {
        //return false if the node can't be a target at all
        if (clickedObjNode.GroundState == GroundStateEnum.nothing || clickedObjNode.GroundState == GroundStateEnum.wall)
        {
            return false;
        }

        // return false if the  node can't be a target for the player actual attack
        switch (GameManager.Instance.actualPlayerAttack.TargetingType) //0 = everything can be target, 1 = square with a target on it only, 2 = empty square only
        {
            case 1:
                if (clickedObjNode.GroundState != GroundStateEnum.player)
                {
                    return false;
                }
                break;
            case 2:
                if (clickedObjNode.GroundState == GroundStateEnum.player)
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

