using UnityEngine;
using System.Collections;
public class ClickDetector : MonoBehaviour
{
    public PathFinding pathfinder;
    public GridScript grid;
    public GameObject player;

    AttackScript playerAttack;

    public float speed;

    Vector3 clickedObjectPosition;

    private void Start()
    {
        playerAttack = player.GetComponent<AttackScript>();
    }

    void Update()
    {
        switch (GameManager.Instance.actualPlayerState)
        {
            case GameManager.PlayerState.idle:
                if (GetClickedGameObject() != null)
                {
                    clickedObjectPosition = GetClickedGameObject().transform.position;
                    pathfinder.target = GetClickedGameObject().transform;
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
                    if (targetCheck())
                    {

                    }
                }
                break;
        }
    }

    bool targetCheck()
    {
        Node clickedObjNode = grid.NodeFromWorldPoint(GetClickedGameObject().transform.position);
        Node playerNode = grid.NodeFromWorldPoint(player.transform.position);
       
        int actualRange = Mathf.Abs(clickedObjNode.gridX - playerNode.gridX) + Mathf.Abs(clickedObjNode.gridY - playerNode.gridY);

        if (actualRange > playerAttack.chosenMove.Range || actualRange < playerAttack.chosenMove.MinimumRange)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    IEnumerator MovePlayer()
    {
        foreach (Node n in grid.path)
        {
            while (Vector3.Distance(player.transform.position, n.nodeObj.transform.position) > 0.05f)
            {
                float step = speed * Time.deltaTime;
                player.transform.position = Vector3.MoveTowards(player.transform.position, n.nodeObj.transform.position, step);
                yield return null;
            }
        }
        GameManager.Instance.actualPlayerState = GameManager.PlayerState.idle;
        grid.CheckWalkable();
    }


    bool PathCheck()
    {
        if (grid.grid[(int)(clickedObjectPosition.x + grid.gridSizexCoeff), (int)(clickedObjectPosition.z + grid.gridSizeyCoeff)].groundstate == GroundState.possible)
        {
            return true;
        }
        return false;
    }

    GameObject GetClickedGameObject() 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            return hit.transform.gameObject;
        }
        else
        {
            return null;
        }
    }
}
