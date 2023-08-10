using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : Singleton<MovementManager>
{
    //Move the player throught the path node per node
    public IEnumerator MovePersonnage(GameObject personnage, float speed)
    {
        foreach (Node n in GridManager.Instance.path)
        {
            personnage.GetComponent<PersonnageScript>().actualMovementPoint -= 1;
            while (Vector3.Distance(personnage.transform.position, n.nodeObj.transform.position) > 0.05f)
            {
                float step = speed * Time.deltaTime;
                personnage.transform.position = Vector3.MoveTowards(personnage.transform.position, n.nodeObj.transform.position, step);
                yield return null;
            }
        }
        CombatManager.Instance.ActualPlayerState = CombatManager.PlayerState.idle;
        GridManager.Instance.UpdateGridState();
    }

    //Check if the target node is valid for movement
    public bool PathCheck(Vector3 objectPosition)
    {
        if (GridManager.Instance.grid[(int)(objectPosition.x + GridManager.Instance.gridSizexCoeff), (int)(objectPosition.z + GridManager.Instance.gridSizeyCoeff)].GroundState == GroundStateEnum.possible)
        {
            return true;
        }
        return false;
    }
}
