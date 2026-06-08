using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridGenerator : MonoBehaviour
{
    [Header("Grid")]
    [Range(2,7)]
    [SerializeField] private int m_gridSize = 4;
    [SerializeField] private Transform m_gridParent;
    [SerializeField] private GridLayoutGroup m_layoutGroup;

    [Header("Cell")]
    [SerializeField] private GameObject m_cellPrefab;
    [SerializeField] private float m_cellSize = 100;

    [SerializeField] private bool m_deleteAllCells = false;

    private List<GameObject> m_cellList = new();
    private int m_cellTotal;

    #region Run-Time Getters

    public float GetCellSize()
    {
        return m_cellSize;
    }

    public List<GameObject> GetCellList()
    {
        return m_cellList;
    }

    //get height and width of the grid based off of the num of cells and cell size
    //and cell spacing
    public Vector2 GetGridWorldSize()
    {
        //cell spacing for the number of cells - 1 (to only get spaces in between cells and not at the edge)
        float width = (m_gridSize * m_cellSize) + (m_layoutGroup.spacing.x * (m_gridSize - 1));
        float height = (m_gridSize * m_cellSize) + (m_layoutGroup.spacing.y * (m_gridSize - 1));
        return new Vector2(width, height);
    }

    #endregion

    #region Grid Generation In Editor

#if UNITY_EDITOR
    private void OnValidate()
    {
        EditorApplication.delayCall -= RefreshGrid;
        EditorApplication.delayCall += RefreshGrid;
    }

    private void RefreshGrid()
    {
        if (this == null)
            return;

        //prevent prefab asset editing
        if (EditorUtility.IsPersistent(gameObject))
            return;

        //prevent running in play mode/compiling
        if (EditorApplication.isCompiling || EditorApplication.isPlaying)
            return;

        FindExistingCells();

        //removes any null cell objects from the list
        m_cellList.RemoveAll(cell => cell == null);

        CalcCellTotal();

        AddCells();
        RemoveAllCells();

        UpdateCellSize();
        UpdateLayout();
    }

    //repopulate cell list with existing cells
    private void FindExistingCells()
    {
        if (m_gridParent == null)
            return;

        m_cellList.Clear();

        foreach (Transform cell in m_gridParent)
        {
            m_cellList.Add(cell.gameObject);
        }
    }

    //instantiate new cells when the grid size increases and add to cell list
    private void AddCells()
    {
        //cell list count shouldnt be greater than cell total, so prevent any extra being added when cells need to be removed
        if (m_cellList.Count > m_cellTotal)
        {
            RemoveCells();
            return;
        }

        for (int i = m_cellList.Count; i < m_cellTotal; i++)
        {
            GameObject newCell = Instantiate(m_cellPrefab);

            if (m_gridParent != null)
            {
                newCell.transform.SetParent(m_gridParent, false);
            }

            m_cellList.Add(newCell);
        }
    }

    //if the grid size decreases the cell count should now be smaller than the cell list count
    //so destroy the excess cells
    private void RemoveCells()
    {
        for (int i = m_cellList.Count - 1; i >= m_cellTotal; i--)
        {
            DestroyCell(i);
        }
    }

    //if the deleteAllCells bool is set true in the inpsector
    //delete all the cells in cell list and reset bool
    private void RemoveAllCells()
    {
        if (m_deleteAllCells)
        {
            for (int i = m_cellList.Count - 1; i >= 0; i--)
            {
                DestroyCell(i);
            }

            m_deleteAllCells = false;
        }
    }

    //destroy the indexed cell gameobject and remove from list
    private void DestroyCell(int index)
    {
        DestroyImmediate(m_cellList[index]);
        m_cellList.Remove(m_cellList[index]);
    }

    //the number of cells is the set gridSize^2 to make a square
    private void CalcCellTotal()
    {
        m_cellTotal = m_gridSize * m_gridSize;
    }

    private void UpdateCellSize()
    {
        m_layoutGroup.cellSize = new (m_cellSize, m_cellSize);
    }

    //update the grid layout group's column constraint size to be the grid size
    //so that the grid is a square
    private void UpdateLayout()
    {
        if (m_layoutGroup == null)
            return;

        if (m_layoutGroup.constraint != GridLayoutGroup.Constraint.FixedColumnCount)
            m_layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        
        m_layoutGroup.constraintCount = m_gridSize;
    }
#endif

    #endregion
}
