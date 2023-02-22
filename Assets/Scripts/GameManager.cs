﻿using System.Collections;
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
    public SkillBase actualPlayerAttack;

    public List<GameObject> playerOrder = new List<GameObject>();
    public int turnCount;
    int actualPlayerIndex;

    //get the playing character
    public GameObject ActualPlayer
    {
        get {if (playerOrder.Count != 0){ return playerOrder[actualPlayerIndex];}
             else { return null; }
            }
    }


    //get the playing character script
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

    public delegate void StartTurnEventHandler();
    public static event StartTurnEventHandler StartTurnEvent;

    //when called skip to the next character turn
    public void NextPlayerTurn()
    {
        ActualPlayerScript.EndTurn();
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
        RaiseStartTurnEvent();
    }

    protected virtual void RaiseStartTurnEvent()
    {
        StartTurnEvent?.Invoke();
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

    GroundStateEnum _groundtstate;

    GameObject _player;


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
                    _groundtstate = GroundStateEnum.wall;
                    _player = null;
                }
                else if (Physics.CheckSphere(worldPoint, nodeRadius, playerMask))
                {
                    _groundtstate = GroundStateEnum.player;
                    _player = Physics.OverlapSphere(worldPoint, nodeRadius, playerMask)[0].gameObject;
                }
                else
                {
                    _groundtstate = GroundStateEnum.possible;
                    _player = null;
                }
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                gridObj = Instantiate(gridObjPrefab, new Vector3(x - gridSizexCoeff, 0, y - gridSizeyCoeff), Quaternion.identity);
                grid[x, y] = new Node(_groundtstate, worldPoint, x, y, gridObj, _player);
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

    public List<Node> nodeToCheck = new List<Node>();

    bool MaxDistanceCheck(Node n)
    {
        PathFinding.Instance.target = n.nodeObj.transform;
        PathFinding.Instance.FindPath(GameManager.Instance.ActualPlayer.transform.position, n.nodeObj.transform.position);
        return (path.Count > GameManager.Instance.ActualPlayerScript.actualMovementPoint);
    }
    public void UpdateGridState()
    {
        switch (GameManager.Instance.actualPlayerState)
        {
            case GameManager.PlayerState.idle:
                foreach (Node n in grid)
                {
                    if (Physics.CheckSphere(n.nodeObj.transform.position, nodeRadius, unwalkableMask))
                    {
                        _groundtstate = GroundStateEnum.wall;
                        _player = null;
                    }
                    else if (Physics.CheckSphere(n.nodeObj.transform.position, nodeRadius, playerMask))
                    {
                        _groundtstate = GroundStateEnum.player;
                        _player = Physics.OverlapSphere(n.nodeObj.transform.position, nodeRadius, playerMask)[0].gameObject;
                    }
                    else
                    {
                        _groundtstate = GroundStateEnum.toofar;
                        _player = null;
                    }
                    n.GroundState = _groundtstate;
                    n.player = _player;
                }

                nodeToCheck.Add(NodeFromWorldPoint(GameManager.Instance.ActualPlayer.transform.position));
                while (nodeToCheck.Count > 0)
                {
                    Node node = nodeToCheck[0];
                    foreach (Node neighbour in GetNeighbours(node))
                    {
                        if (neighbour.GroundState == GroundStateEnum.toofar)
                        {
                            neighbour.GroundState = GroundStateEnum.possible;
                            if (!MaxDistanceCheck(neighbour))
                            {
                                nodeToCheck.Add(neighbour);
                            }
                            else
                            {
                                neighbour.GroundState = GroundStateEnum.toofar;
                            }
                        }
                    }

                    nodeToCheck.Remove(node);
                }
                break;
            case GameManager.PlayerState.isTargeting:

                foreach (Node n in grid)
                {
                    Node playerNode = NodeFromWorldPoint(GameManager.Instance.ActualPlayer.transform.position); //get player Node
                    int actualRange = Mathf.Abs(n.gridX - playerNode.gridX) + Mathf.Abs(n.gridY - playerNode.gridY);
                    if (Physics.CheckSphere(n.nodeObj.transform.position, nodeRadius, unwalkableMask))
                    {
                        _groundtstate = GroundStateEnum.wall;
                        _player = null;
                    }
                    else if (Physics.CheckSphere(n.nodeObj.transform.position, nodeRadius, playerMask))
                    {
                        _groundtstate = GroundStateEnum.player;
                        _player = Physics.OverlapSphere(n.nodeObj.transform.position, nodeRadius, playerMask)[0].gameObject;
                    }
                    else if (actualRange <= GameManager.Instance.actualPlayerAttack.Range && actualRange >= GameManager.Instance.actualPlayerAttack.MinimumRange)
                    {
                        _groundtstate = GroundStateEnum.possible;
                        _player = null;
                    }
                    else
                    {
                        _groundtstate = GroundStateEnum.toofar;
                        _player = null;
                    }
                    n.GroundState = _groundtstate;
                    n.player = _player;
                }
                break;
        }
    }
}

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
        GameManager.Instance.actualPlayerState = GameManager.PlayerState.idle;
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
