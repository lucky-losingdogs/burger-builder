using UnityEngine;

public class Item : MonoBehaviour
{
    private ShapeData m_shape;
    private Vector3Int[] m_cells;
    private Vector3Int m_position;

    #region Initialisation

    public void Initialise(ShapeData shape, Vector3Int position)
    {
        m_shape = shape;
        m_position = position;
        SetCells(shape);
    }

    private void SetCells(ShapeData shape)
    {
        Vector2Int[] tempCells = shape.GetCells();

        if (m_cells == null)
        {
            m_cells = new Vector3Int[tempCells.Length];
        }

        for (int i = 0; i < tempCells.Length; i++)
        {
            m_cells[i] = (Vector3Int)tempCells[i];
        }
    }

    #endregion

    #region Getters / Setters

    public ShapeData GetShape() { return m_shape; }

    public Vector3Int[] GetCells() { return m_cells; }

    public Vector3Int GetPosition() { return m_position; }

    public void SetPosition(Vector3Int newPos) { m_position = newPos; }

    #endregion
}