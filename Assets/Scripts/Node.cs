using UnityEngine;
using System.Collections;

public class Node
{
	private GroundStateEnum groundState;
	public GroundStateEnum GroundState 
	{ 
		get { return groundState; }
        set
        {
			groundState = value;

			if (groundState == GroundStateEnum.wall || groundState == GroundStateEnum.nothing)
			{
				nodeObj.GetComponent<MeshRenderer>().material.color = Color.black;
			}
			else
			{
				nodeObj.GetComponent<MeshRenderer>().material.color = (groundState == GroundStateEnum.possible) ? Color.green : Color.gray;
			}
		}
	}

	public GameObject nodeObj;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;

	public int gCost;
	public int hCost;
	public Node parent;

	private bool isTarget;
	public bool IsTarget
	{
		get { return isTarget; }
		set
		{
			isTarget = value;
			nodeObj.GetComponent<MeshRenderer>().material.color = value ? new Color32(252, 185, 65, 1) : Color.gray;
		}
	}

	private bool isInPath;
	public bool IsInpath
	{
		get { return isInPath; }
		set
		{
			isInPath = value;
			if (groundState == GroundStateEnum.possible)
				nodeObj.GetComponent<MeshRenderer>().material.color = value ? new Color32(252, 185, 65, 1) : Color.green;
		}
	}

	public Node(GroundStateEnum _groundstate, Vector3 _worldPos, int _gridX, int _gridY, GameObject _nodeObj)
	{
		nodeObj = _nodeObj;
		GroundState = _groundstate;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
	}

	public int fCost
	{
		get
		{
			return gCost + hCost;
		}
	}
}

public enum GroundStateEnum
{
	nothing,
	possible,
	player,
	wall,
	toofar,
}
