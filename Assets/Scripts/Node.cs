using UnityEngine;
using System.Collections;

public class Node
{
	public GroundState groundstate;
	public GameObject nodeObj;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;

	public int gCost;
	public int hCost;
	public Node parent;

	public Node(GroundState _groundstate, Vector3 _worldPos, int _gridX, int _gridY, GameObject _nodeObj)
	{
		groundstate = _groundstate;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
		nodeObj = _nodeObj;
	}

	public int fCost
	{
		get
		{
			return gCost + hCost;
		}
	}
}

public enum GroundState
{
	nothing,
	possible,
	player,
	wall,
	toofar,
}
