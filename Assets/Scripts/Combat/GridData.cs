using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridData : MonoBehaviour
{
	public LayerMask unwalkableMask;
	public LayerMask playerMask;
	public LayerMask fieldOfViewMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	public Node[,] grid;

	public float gridSizexCoeff, gridSizeyCoeff;

	public GameObject gridObjPrefab;
	public GameObject colliderObjPrefab;

	void Awake()
	{
		SetupVar();
		GridManager.Instance.CreateGrid();
	}

	public void SetupVar()
	{
		GridManager.Instance.fieldOfViewMask = fieldOfViewMask;
		GridManager.Instance.unwalkableMask = unwalkableMask;
		GridManager.Instance.playerMask = playerMask;
		GridManager.Instance.gridWorldSize = gridWorldSize;
		GridManager.Instance.nodeRadius = nodeRadius;
		GridManager.Instance.grid = grid;
		GridManager.Instance.gridSizexCoeff = gridSizexCoeff;
		GridManager.Instance.gridSizeyCoeff = gridSizeyCoeff;
		GridManager.Instance.gridObjPrefab = gridObjPrefab;
		GridManager.Instance.colliderObjPrefab = colliderObjPrefab;

	}
    private void Start()
    {
		GridManager.Instance.UpdateGridState();
    }
}
