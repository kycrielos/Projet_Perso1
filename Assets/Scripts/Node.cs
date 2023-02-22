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

			if (groundState != GroundStateEnum.player)
            {
				player = null;
            }

			//everytime the state change change the color too
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

	public int gCost; //distance from the starting node (used in the pathfinder)
	public int hCost; //distance from the ending node (used in the pathfinder)


	public Node parent;

	private bool isTarget;

	public GameObject player;

	public bool IsTarget
	{
		get { return isTarget; }
		set
		{
			isTarget = value;

			//if the square is targeted change its color
			if (groundState == GroundStateEnum.possible)
			{
				nodeObj.GetComponent<MeshRenderer>().material.color = value ? new Color32(252, 185, 65, 1) : Color.green;
			}
			else if(groundState != GroundStateEnum.wall && groundState != GroundStateEnum.nothing)
			{
				nodeObj.GetComponent<MeshRenderer>().material.color = value ? new Color32(252, 185, 65, 1) : Color.gray;
			}
		}
	}

	private bool isInPath;
	public bool IsInpath
	{
		get { return isInPath; }
		set
		{
			isInPath = value;

			//if the square is in the path change its color
			if (groundState == GroundStateEnum.possible)
				nodeObj.GetComponent<MeshRenderer>().material.color = value ? new Color32(252, 185, 65, 1) : Color.green;
		}
	}

	//I forgot why it's like this and i'm to scared to try to change it
	public Node(GroundStateEnum _groundstate, Vector3 _worldPos, int _gridX, int _gridY, GameObject _nodeObj, GameObject _player)
	{
		nodeObj = _nodeObj;
		GroundState = _groundstate;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
		player = _player;
	}

	public int fCost // total cost (used in the pathfinder)
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
