using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinding : Singleton<PathFinding>
{
	public Transform target;
	private List<Node> path;

	void Update()
	{
		if (CombatManager.Instance.ActualPlayerState == CombatManager.PlayerState.idle)
		{
			FindPath(CombatManager.Instance.ActualPlayer.transform.position, target.position);
		}
	}

	//Will find the shortest path between 2 nodes
	public void FindPath(Vector3 startPos, Vector3 targetPos)
	{
		//Update the last path state
		if (path != null) //safety
		{
			foreach (Node n in path)
			{
				n.IsInpath = false;
			}
		}

		//Set up
		Node startNode = GridManager.Instance.NodeFromWorldPoint(startPos);
		Node targetNode = GridManager.Instance.NodeFromWorldPoint(targetPos);
		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();
		
		//A* Algorithm :

		openSet.Add(startNode);

		while (openSet.Count > 0)
		{
			//check the next node waiting in the open list
			Node node = openSet[0];

			//test if any node in open list have a lowest cost, so it check the one with the lowest cost before the others
			for (int i = 1; i < openSet.Count; i++)
			{
				if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
				{
					if (openSet[i].hCost < node.hCost)
						node = openSet[i];
				}
			}

			//end if the target has been found
			if (node == targetNode)
			{
				RetracePath(startNode, targetNode);
				return;
			}

			openSet.Remove(node);
			closedSet.Add(node);

			foreach (Node neighbour in GridManager.Instance.GetNeighbours(node))
			{
				//if the adjacent cell is unwalkable or the adjacent cell is in closed list skip to the next adjacent cell
				if (neighbour.GroundState != GroundStateEnum.possible || closedSet.Contains(neighbour))
				{
					continue;
				}

				//calcul the new path cost
				int newMovementCostToNeighbour = node.gCost + GetDistance(node, neighbour);

				//if the new path to adjacent cell is shorter or adjacent cell is not in the open list : set the cost and if it is not in it add to open list 
				if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
				{
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = node;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}
	}

	//Track back the path
	void RetracePath(Node startNode, Node endNode)
	{
		path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode)
		{
			currentNode.IsInpath = true;
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse();

		GridManager.Instance.path = path;
	}

	int GetDistance(Node nodeA, Node nodeB)
	{
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14 * dstY + 10 * (dstX - dstY);
		return 14 * dstX + 10 * (dstY - dstX);
	}
}
