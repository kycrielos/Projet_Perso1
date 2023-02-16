using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinding : Singleton<PathFinding>
{
	public Transform target;
	private List<Node> path;

	void Update()
	{
		if (GameManager.Instance.actualPlayerState == GameManager.PlayerState.idle)
		{
			FindPath(GameManager.Instance.ActualPlayer.transform.position, target.position);
		}
	}

	public void FindPath(Vector3 startPos, Vector3 targetPos)
	{
		if (path != null)
		{
			foreach (Node n in path)
			{
				n.IsInpath = false;
			}
		}

		Node startNode = GridManager.Instance.NodeFromWorldPoint(startPos);
		Node targetNode = GridManager.Instance.NodeFromWorldPoint(targetPos);

		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);

		while (openSet.Count > 0)
		{
			Node node = openSet[0];

			for (int i = 1; i < openSet.Count; i++)
			{
				if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
				{
					if (openSet[i].hCost < node.hCost)
						node = openSet[i];
				}
			}

			if (node == targetNode)
			{
				RetracePath(startNode, targetNode);
				return;
			}

			openSet.Remove(node);
			closedSet.Add(node);
			node.IsInpath = false;

			foreach (Node neighbour in GridManager.Instance.GetNeighbours(node))
			{
				if (neighbour.GroundState != GroundStateEnum.possible || closedSet.Contains(neighbour))
				{
					continue;
				}

				int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
				{
					neighbour.gCost = newCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = node;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}
	}

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
