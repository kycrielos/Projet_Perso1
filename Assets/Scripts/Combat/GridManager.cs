using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    public LayerMask unwalkableMask;
    public LayerMask playerMask;
    public LayerMask fieldOfViewMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    public float gridSizexCoeff, gridSizeyCoeff;

    public GameObject gridObjPrefab;
    public GameObject colliderObjPrefab;

    GameObject gridObj;
    GameObject colliderObj;

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
                if (Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask) || GridBorder(x,y))
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
                //bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                gridObj = Instantiate(gridObjPrefab, new Vector3(x - gridSizexCoeff, 0, y - gridSizeyCoeff), Quaternion.identity);
                colliderObj = Instantiate(colliderObjPrefab, new Vector3(x - gridSizexCoeff, -0.5f, y - gridSizeyCoeff), Quaternion.identity);
                grid[x, y] = new Node(_groundtstate, worldPoint, x, y, gridObj, colliderObj, _player);
            }
        }
    }

    bool GridBorder(int x, int y) 
    {
        if (x == 0 || y == 0 || x == gridSizeX - 1 || y == gridSizeY - 1) 
        {
            return true;
        }
        return false;
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

    public List<Node> nodeToCheck = new List<Node>();

    bool MaxDistanceCheck(Node n)
    {
        PathFinding.Instance.target = n.nodeObj.transform;
        PathFinding.Instance.FindPath(CombatManager.Instance.ActualPlayer.transform.position, n.nodeObj.transform.position);
        return (PathFinding.Instance.finalPath.Count > CombatManager.Instance.ActualPlayerScript.actualMovementPoint);
    }
    public void UpdateGridState()
    {
        switch (CombatManager.Instance.ActualPlayerState)
        {
            case CombatManager.PlayerState.idle:
                foreach (Node n in grid)
                {
                    if (Physics.CheckSphere(n.nodeObj.transform.position, nodeRadius, unwalkableMask) || GridBorder(n.gridX, n.gridY))
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

                nodeToCheck.Add(NodeFromWorldPoint(CombatManager.Instance.ActualPlayer.transform.position));
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
            case CombatManager.PlayerState.isTargeting:

                foreach (Node n in grid)
                {
                    Node playerNode = NodeFromWorldPoint(CombatManager.Instance.ActualPlayer.transform.position); //get player Node
                    int actualRange = Mathf.Abs(n.gridX - playerNode.gridX) + Mathf.Abs(n.gridY - playerNode.gridY);
                    if (Physics.CheckSphere(n.nodeObj.transform.position, nodeRadius, unwalkableMask) || GridBorder(n.gridX, n.gridY))
                    {
                        _groundtstate = GroundStateEnum.wall;
                        _player = null;
                    }
                    else if (Physics.CheckSphere(n.nodeObj.transform.position, nodeRadius, playerMask))
                    {
                        if (CheckRange(actualRange, CombatManager.Instance.actualPlayerAttack) && CheckLine(n, playerNode, CombatManager.Instance.actualPlayerAttack))
                        {
                            if (CheckLineOfSight(n, CombatManager.Instance.actualPlayerAttack))
                            {
                                _groundtstate = GroundStateEnum.targetablePlayer;
                            }
                            else
                            {
                                _groundtstate = GroundStateEnum.noView;
                            }
                            _player = Physics.OverlapSphere(n.nodeObj.transform.position, nodeRadius, playerMask)[0].gameObject;
                        }
                        else
                        {
                            _groundtstate = GroundStateEnum.player;
                            _player = Physics.OverlapSphere(n.nodeObj.transform.position, nodeRadius, playerMask)[0].gameObject;
                        }
                    }
                    else if (CheckRange(actualRange, CombatManager.Instance.actualPlayerAttack) && CheckLine(n, playerNode, CombatManager.Instance.actualPlayerAttack))
                    {
                        if (CheckLineOfSight(n, CombatManager.Instance.actualPlayerAttack))
                        {
                            _groundtstate = GroundStateEnum.targetable;
                        }
                        else
                        {
                            _groundtstate = GroundStateEnum.noView;
                        }
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
            case CombatManager.PlayerState.idleAI:
                foreach (Node n in grid)
                {
                    if (Physics.CheckSphere(n.nodeObj.transform.position, nodeRadius, unwalkableMask) || GridBorder(n.gridX, n.gridY))
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
                        _groundtstate = GroundStateEnum.possible;
                        _player = null;
                    }
                    n.GroundState = _groundtstate;
                    n.player = _player;
                }
                break;
        }
    }

    public bool CheckRange(int actualRange, SkillBase attack)
    {
        if (actualRange <= attack.Range && actualRange >= attack.MinimumRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckLine(Node n, Node playerNode, SkillBase attack)
    {
        if (attack.InLineOnly)
        {
            if (n.gridX == playerNode.gridX || n.gridY == playerNode.gridY)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }

    public bool CheckLineOfSight(Node n, SkillBase attack)
    {
        if (attack.LineOfSight)
        {
            if (Physics.CheckBox(n.colliderObj.transform.position, n.colliderObj.GetComponent<Collider>().bounds.extents / 2, n.colliderObj.transform.rotation, fieldOfViewMask))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }
}
