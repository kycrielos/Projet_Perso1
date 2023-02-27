using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecialEffect", menuName = "SpecialEffect/Create new MovementSpecialEffect")]

public class MoveTarget : SpecialEffect
{
    [Tooltip("0 = pull, 1 = push, 2 = teleport, 3 = switch place")]
    [SerializeField] int movementType;
    [SerializeField] bool playerIsMoved;
    [SerializeField] int movementValue;

    GameObject player;
    Node playerNode;
    Node targetNode;

    public override void Init(GameObject target)
    {
        player = GameManager.Instance.ActualPlayer;
        playerNode = GridManager.Instance.NodeFromWorldPoint(player.transform.position);
        targetNode = GridManager.Instance.NodeFromWorldPoint(target.transform.position);
        if (playerIsMoved)
        {
            switch (movementType)
            {
                case 0:
                    if (playerNode.gridY == targetNode.gridY)
                    {
                        if (playerNode.gridX - targetNode.gridX < 0)
                        {
                            for (int i = 1; i <= movementValue; i++)
                            {
                                if (IsNodeAvailable(playerNode.gridX + i, playerNode.gridY))
                                {
                                    player.transform.position = GridManager.Instance.grid[playerNode.gridX + i, playerNode.gridY].worldPosition;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = -1; -i <= movementValue; i--)
                            {
                                if (IsNodeAvailable(playerNode.gridX + i, playerNode.gridY))
                                {
                                    player.transform.position = GridManager.Instance.grid[playerNode.gridX + i, playerNode.gridY].worldPosition;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (playerNode.gridY - targetNode.gridY < 0)
                        {
                            for (int i = 1; i <= movementValue; i++)
                            {
                                if (IsNodeAvailable(playerNode.gridX, playerNode.gridY + i))
                                {
                                    player.transform.position = GridManager.Instance.grid[playerNode.gridX, playerNode.gridY + i].worldPosition;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = -1; -i <= movementValue; i--)
                            {
                                if (IsNodeAvailable(playerNode.gridX, playerNode.gridY + i))
                                {
                                    player.transform.position = GridManager.Instance.grid[playerNode.gridX, playerNode.gridY + i].worldPosition;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    break;
                case 1:
                    if (playerNode.gridY == targetNode.gridY)
                    {
                        if (playerNode.gridX - targetNode.gridX > 0)
                        {
                            for (int i = 1; i <= movementValue; i++)
                            {
                                if (IsNodeAvailable(playerNode.gridX + i, playerNode.gridY))
                                {
                                    player.transform.position = GridManager.Instance.grid[playerNode.gridX + i, playerNode.gridY].worldPosition;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = -1; -i <= movementValue; i--)
                            {
                                if (IsNodeAvailable(playerNode.gridX + i, playerNode.gridY))
                                {
                                    player.transform.position = GridManager.Instance.grid[playerNode.gridX + i, playerNode.gridY].worldPosition;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (playerNode.gridY - targetNode.gridY > 0)
                        {
                            for (int i = 1; i <= movementValue; i++)
                            {
                                if (IsNodeAvailable(playerNode.gridX, playerNode.gridY + i))
                                {
                                    player.transform.position = GridManager.Instance.grid[playerNode.gridX, playerNode.gridY + i].worldPosition;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = -1; -i <= movementValue; i--)
                            {
                                if (IsNodeAvailable(playerNode.gridX, playerNode.gridY + i))
                                {
                                    player.transform.position = GridManager.Instance.grid[playerNode.gridX, playerNode.gridY + i].worldPosition;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    break;
                case 2:
                    player.transform.position = target.transform.position;
                    break;
                case 3:
                    Vector3 a = player.transform.position;
                    player.transform.position = target.transform.position;
                    target.transform.position = a;
                    break;
            }
        }
        else
        {
            switch (movementType)
            {
                case 0:
                    if (playerNode.gridY == targetNode.gridY)
                    {
                        if (targetNode.gridX - playerNode.gridX < 0)
                        {
                            for (int i = 1; i <= movementValue; i++)
                            {
                                if (IsNodeAvailable(targetNode.gridX + i, targetNode.gridY))
                                {
                                    target.transform.position = GridManager.Instance.grid[targetNode.gridX + i, targetNode.gridY].worldPosition;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = -1; -i <= movementValue; i--)
                            {
                                if (IsNodeAvailable(targetNode.gridX + i, targetNode.gridY))
                                {
                                    target.transform.position = GridManager.Instance.grid[targetNode.gridX + i, targetNode.gridY].worldPosition;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (targetNode.gridY - playerNode.gridY < 0)
                        {
                            for (int i = 1; i <= movementValue; i++)
                            {
                                if (IsNodeAvailable(targetNode.gridX, targetNode.gridY + i))
                                {
                                    target.transform.position = GridManager.Instance.grid[targetNode.gridX, targetNode.gridY + i].worldPosition;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = -1; -i <= movementValue; i--)
                            {
                                if (IsNodeAvailable(targetNode.gridX, targetNode.gridY + i))
                                {
                                    target.transform.position = GridManager.Instance.grid[targetNode.gridX, targetNode.gridY + i].worldPosition;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    break;
                case 1:
                    if (targetNode.gridY == playerNode.gridY)
                    {
                        if (targetNode.gridX - playerNode.gridX > 0)
                        {
                            for (int i = 1; i <= movementValue; i++)
                            {
                                if (IsNodeAvailable(targetNode.gridX + i, targetNode.gridY))
                                {
                                    target.transform.position = GridManager.Instance.grid[playerNode.gridX + i, targetNode.gridY].worldPosition;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = -1; -i <= movementValue; i--)
                            {
                                if (IsNodeAvailable(targetNode.gridX + i, targetNode.gridY))
                                {
                                    target.transform.position = GridManager.Instance.grid[targetNode.gridX + i, targetNode.gridY].worldPosition;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (targetNode.gridY - playerNode.gridY > 0)
                        {
                            for (int i = 1; i <= movementValue; i++)
                            {
                                if (IsNodeAvailable(targetNode.gridX, targetNode.gridY + i))
                                {
                                    target.transform.position = GridManager.Instance.grid[targetNode.gridX, targetNode.gridY + i].worldPosition;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = -1; -i <= movementValue; i--)
                            {
                                if (IsNodeAvailable(targetNode.gridX, targetNode.gridY + i))
                                {
                                    target.transform.position = GridManager.Instance.grid[targetNode.gridX, targetNode.gridY + i].worldPosition;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    break;
            }
        }
        GridManager.Instance.UpdateGridState();
    }

    bool IsNodeAvailable(int GridX, int GridY)
    {
        if (GridX > GridManager.Instance.grid.GetLength(0) || (GridY > GridManager.Instance.grid.GetLength(1)) || GridX < 0 || GridY < 0)
        {
            return false;
        }

        Node n = GridManager.Instance.grid[GridX, GridY];

        if (n.GroundState == GroundStateEnum.nothing || n.GroundState == GroundStateEnum.wall || n.GroundState == GroundStateEnum.player || n.GroundState == GroundStateEnum.targetablePlayer)
        {
            return false;
        }
        return true;
    }
}
