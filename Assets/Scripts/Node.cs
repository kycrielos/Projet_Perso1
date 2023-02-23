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
			switch (groundState)
			{
				case GroundStateEnum.wall:
					nodeObj.GetComponent<MeshRenderer>().material.color = Color.black;
					colliderObj.layer = 11;
					break;
				case GroundStateEnum.nothing:
					nodeObj.GetComponent<MeshRenderer>().material.color = Color.black;
					colliderObj.layer = 13;
					break;
				case GroundStateEnum.possible:
					nodeObj.GetComponent<MeshRenderer>().material.color = Color.green;
					colliderObj.layer = 13;
					break;
				case GroundStateEnum.targetable:
					nodeObj.GetComponent<MeshRenderer>().material.color = Color.cyan;
					colliderObj.layer = 13;
					break;
				case GroundStateEnum.targetablePlayer:
					nodeObj.GetComponent<MeshRenderer>().material.color = Color.cyan;
					colliderObj.layer = 13;
					break;
				case GroundStateEnum.noView:
					nodeObj.GetComponent<MeshRenderer>().material.color = Color.red;
					colliderObj.layer = 13;
					break;
				case GroundStateEnum.player:
					nodeObj.GetComponent<MeshRenderer>().material.color = Color.grey;
					colliderObj.layer = 13;
					break;
				default:
					nodeObj.GetComponent<MeshRenderer>().material.color = Color.grey;
					colliderObj.layer = 13;
					break;
			}
		}
	}

	public GameObject nodeObj;
	public GameObject colliderObj;
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
			if (groundState == GroundStateEnum.targetable || groundState == GroundStateEnum.targetablePlayer)
			{
				nodeObj.GetComponent<MeshRenderer>().material.color = value ? new Color32(252, 185, 65, 1) : Color.cyan;
			}
			else if (groundState == GroundStateEnum.noView)
			{
				nodeObj.GetComponent<MeshRenderer>().material.color = value ? new Color32(252, 185, 65, 1) : Color.red;
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
	public Node(GroundStateEnum _groundstate, Vector3 _worldPos, int _gridX, int _gridY, GameObject _nodeObj, GameObject _colliderObj, GameObject _player)
	{
		nodeObj = _nodeObj;
		colliderObj = _colliderObj;
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
	noView,
	targetable,
	targetablePlayer,
	player,
	wall,
	toofar,
}
