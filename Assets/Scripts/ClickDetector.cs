using UnityEngine;
using System.Collections;
public class ClickDetector : MonoBehaviour
{
    public GameObject player;

    public PersonnageScript playerScript;

    AttackScript playerAttack;

    public float speed;

    Vector3 clickedObjectPosition;

    public LayerMask mask;

    Node clickedObjNode;

    private void Start()
    {
        playerAttack = player.GetComponent<AttackScript>();
        playerScript = player.GetComponent<PersonnageScript>();
    }

    void Update()
    {
        if (playerScript.playerturn)
        {
            switch (GameManager.Instance.actualPlayerState)
            {
                case GameManager.PlayerState.idle:
                    if (GetClickedGameObject() != null)
                    {
                        clickedObjectPosition = GetClickedGameObject().transform.position;
                        PathFinding.Instance.target = GetClickedGameObject().transform;
                        if (Input.GetMouseButtonDown(0) && MovementManager.Instance.PathCheck(clickedObjectPosition))
                        {
                            GameManager.Instance.actualPlayerState = GameManager.PlayerState.isMoving;
                            StartCoroutine(MovementManager.Instance.MovePersonnage(player, speed));
                        }
                    }
                    break;
                case GameManager.PlayerState.isTargeting:
                    if (GetClickedGameObject() != null)
                    {
                        if (targetCheck() && Input.GetMouseButtonDown(0))
                        {
                            if (clickedObjNode.GroundState == GroundStateEnum.player)
                            {

                            }
                            else
                            {
                                clickedObjNode.IsTarget = false;
                                GameManager.Instance.actualPlayerState = GameManager.PlayerState.idle;
                            }
                        }
                    }
                    break;
            }
        }
    }

    bool targetCheck()
    {
        if (clickedObjNode != null)
        {
            clickedObjNode.IsTarget = false;
        }
        clickedObjNode = GridManager.Instance.NodeFromWorldPoint(GetClickedGameObject().transform.position);

        if (clickedObjNode.GroundState == GroundStateEnum.nothing || clickedObjNode.GroundState == GroundStateEnum.wall)
        {
            return false;
        }

        Node playerNode = GridManager.Instance.NodeFromWorldPoint(player.transform.position);

        clickedObjNode.IsTarget = true;


        int actualRange = Mathf.Abs(clickedObjNode.gridX - playerNode.gridX) + Mathf.Abs(clickedObjNode.gridY - playerNode.gridY);

        if (actualRange <= playerAttack.chosenMove.Range && actualRange >= playerAttack.chosenMove.MinimumRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


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

    public void UIMoveClick(MoveBase move)
    {
        if (GameManager.Instance.actualPlayerState == GameManager.PlayerState.idle)
        {
            playerAttack.chosenMove = move;
            GameManager.Instance.actualPlayerState = GameManager.PlayerState.isTargeting;
        }
    }
}

