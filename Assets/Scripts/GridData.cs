using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridData : MonoBehaviour
{
	public LayerMask unwalkableMask;
	public LayerMask playerMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	public Node[,] grid;

	public float gridSizexCoeff, gridSizeyCoeff;

	public GameObject gridObjPrefab;

	void Awake()
	{
		SetupVar();
		GridManager.Instance.CreateGrid();
	}

	public void SetupVar()
    {
		GridManager.Instance.unwalkableMask = unwalkableMask;
		GridManager.Instance.playerMask = playerMask;
		GridManager.Instance.gridWorldSize = gridWorldSize;
		GridManager.Instance.nodeRadius = nodeRadius;
		GridManager.Instance.grid = grid;
		GridManager.Instance.gridSizexCoeff = gridSizexCoeff;
		GridManager.Instance.gridSizeyCoeff = gridSizeyCoeff;
		GridManager.Instance.gridObjPrefab = gridObjPrefab;

	}
    private void Start()
    {
		GridManager.Instance.CheckWalkable();
    }
}
