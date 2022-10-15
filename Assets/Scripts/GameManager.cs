using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static readonly Lazy<T> LazyInstance = new Lazy<T>(CreateSingleton);

    public static T Instance => LazyInstance.Value;

    private static T CreateSingleton()
    {
        var ownerObject = new GameObject($"{typeof(T).Name} (singleton)");
        var instance = ownerObject.AddComponent<T>();
        DontDestroyOnLoad(ownerObject);
        return instance;
    }
}

public class GameManager : Singleton<GameManager>
{
    public int playingPersonnage;
    public PlayerState actualPlayerState;

    public List<GameObject> playerOrder = new List<GameObject>();
    public int turnCount;
    int actualPlayerIndex;

    public GameObject ActualPlayer
    {
        get {if (playerOrder.Count != 0){ return playerOrder[actualPlayerIndex];}
             else { return null; }
            }
    }

    public PersonnageScript ActualPlayerScript
    {
        get { if (ActualPlayer != null) { return ActualPlayer.GetComponent<PersonnageScript>(); }
              else { return null; } }
    }

    public enum PlayerState
    {
        idle,
        isMoving,
        isTargeting,
        isAttacking,
        isDying,
    }

    public void NextPlayerTurn()
    {
        if (actualPlayerIndex < playerOrder.Count -1)
        {
            actualPlayerIndex++;
        }
        else
        {
            turnCount++;
            actualPlayerIndex = 0;
        }
        ActualPlayerScript.StartTurn();
    }
}

public class GridManager : Singleton<GridManager>
{
    public LayerMask unwalkableMask;
    public LayerMask playerMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    public float gridSizexCoeff, gridSizeyCoeff;

    public GameObject gridObjPrefab;

    GameObject gridObj;

    GroundState _groundtstate;

    public PathFinding pathfinder;


    public void CreateGrid()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        gridSizexCoeff = gridSizeX / 2 - 0.5f;
        gridSizeyCoeff = gridSizeY / 2 - 0.5f;

        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                if (Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask))
                {
                    _groundtstate = GroundState.wall;
                }
                else if (Physics.CheckSphere(worldPoint, nodeRadius, playerMask))
                {
                    _groundtstate = GroundState.player;
                }
                else
                {
                    _groundtstate = GroundState.possible;
                }
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                gridObj = Instantiate(gridObjPrefab, new Vector3(x - gridSizexCoeff, 0, y - gridSizeyCoeff), Quaternion.identity);
                grid[x, y] = new Node(_groundtstate, worldPoint, x, y, gridObj);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0 || x != 0 && y != 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public List<Node> path;

    private void Update()
    {
        if (grid != null)
        {
            foreach (Node n in grid)
            {
                n.nodeObj.GetComponent<MeshRenderer>().material.color = (n.groundstate == GroundState.possible) ? Color.green : Color.gray;
                if (path != null && !distanceChecking)
                    if (path.Contains(n))
                        if (GameManager.Instance.actualPlayerState == GameManager.PlayerState.idle || GameManager.Instance.actualPlayerState == GameManager.PlayerState.isMoving)
                            n.nodeObj.GetComponent<MeshRenderer>().material.color = new Color32(252, 185, 65, 1);
                if (n.isTarget)
                    n.nodeObj.GetComponent<MeshRenderer>().material.color = new Color32(252, 185, 65, 1);
            }
        }
    }

    public List<Node> nodeToCheck = new List<Node>();

    bool distanceChecking;

    bool MaxDistanceCheck(Node n)
    {
        pathfinder.target = n.nodeObj.transform;
        pathfinder.FindPath(GameManager.Instance.ActualPlayer.transform.position, n.nodeObj.transform.position);
        return (path.Count > GameManager.Instance.ActualPlayerScript.actualMovementPoint);
    }
    public void CheckWalkable()
    {
        foreach (Node n in grid)
        {
            if (Physics.CheckSphere(n.nodeObj.transform.position, nodeRadius, unwalkableMask))
            {
                _groundtstate = GroundState.wall;
            }
            else if (Physics.CheckSphere(n.nodeObj.transform.position, nodeRadius, playerMask))
            {
                _groundtstate = GroundState.player;
            }
            else
            {
                _groundtstate = GroundState.toofar;
            }
            n.groundstate = _groundtstate;
        }

        nodeToCheck.Add(NodeFromWorldPoint(GameManager.Instance.ActualPlayer.transform.position));
        distanceChecking = true;
        while (nodeToCheck.Count > 0)
        {
            Node node = nodeToCheck[0];
            foreach (Node neighbour in GetNeighbours(node))
            {
                if (neighbour.groundstate == GroundState.toofar)
                {
                    neighbour.groundstate = GroundState.possible;
                    if (!MaxDistanceCheck(neighbour))
                    {
                        nodeToCheck.Add(neighbour);
                    }
                    else
                    {
                        neighbour.groundstate = GroundState.toofar;
                    }
                }
            }

            nodeToCheck.Remove(node);
        }
        distanceChecking = false;
    }
}

