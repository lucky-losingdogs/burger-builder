using UnityEngine;

public class Item : MonoBehaviour
{
    private ItemStructure m_structure;

    #region Initialisation

    public void Initialise(ShapeData shape, Vector3Int position)
    {
        m_structure.Initialise(shape, position);
    }

    #endregion

    #region Getters / Setters

    public ShapeData GetShape() { return m_structure.shape; }

    public Vector3Int[] GetCells() { return m_structure.cells; }

    public Vector3Int GetPosition() { return m_structure.position; }

    public void SetPosition(Vector3Int newPos) { m_structure.position = newPos; }

    #endregion
}

[System.Serializable]
public struct ItemStructure
{
    public ShapeData shape;
    public Vector3Int[] cells;
    public Vector3Int position;

    public void Initialise(ShapeData shape, Vector3Int position)
    {
        this.shape = shape;
        this.position = position;
        SetCells(shape);
    }

    private void SetCells(ShapeData shape)
    {
        Vector2Int[] tempCells = shape.GetCells();

        if (cells == null)
        {
            cells = new Vector3Int[tempCells.Length];
        }

        for (int i = 0; i < tempCells.Length; i++)
        {
            cells[i] = (Vector3Int)tempCells[i];
        }
    }
}