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
                        if (Input.GetMouseButtonDown(0) && PathCheck())
                        {
                            GameManager.Instance.actualPlayerState = GameManager.PlayerState.isMoving;
                            StartCoroutine(MovePlayer());
                        }
                    }
                    break;
                case GameManager.PlayerState.isTargeting:
                    if (GetClickedGameObject() != null)
                    {
                        if (targetCheck() && Input.GetMouseButtonDown(0))
                        {
                            clickedObjNode.isTarget = false;
                            GameManager.Instance.actualPlayerState = GameManager.PlayerState.idle;
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
            clickedObjNode.isTarget = false;
        }
        clickedObjNode = GridManager.Instance.NodeFromWorldPoint(GetClickedGameObject().transform.position);
        Node playerNode = GridManager.Instance.NodeFromWorldPoint(player.transform.position);

        if (clickedObjNode.groundstate == GroundState.nothing || clickedObjNode.groundstate == GroundState.wall)
        {
            return false;
        }

        clickedObjNode.isTarget = true;


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

    IEnumerator MovePlayer()
    {
        foreach (Node n in GridManager.Instance.path)
        {
            player.GetComponent<PersonnageScript>().actualMovementPoint -= 1;
            while (Vector3.Distance(player.transform.position, n.nodeObj.transform.position) > 0.05f)
            {
                float step = speed * Time.deltaTime;
                player.transform.position = Vector3.MoveTowards(player.transform.position, n.nodeObj.transform.position, step);
                yield return null;
            }
        }
        GameManager.Instance.actualPlayerState = GameManager.PlayerState.idle;
        GridManager.Instance.CheckWalkable();
    }


    bool PathCheck()
    {
        if (GridManager.Instance.grid[(int)(clickedObjectPosition.x + GridManager.Instance.gridSizexCoeff), (int)(clickedObjectPosition.z + GridManager.Instance.gridSizeyCoeff)].groundstate == GroundState.possible)
        {
            return true;
        }
        return false;
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

