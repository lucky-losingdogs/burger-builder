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

    public Vector3Int[] GetCells() { return m_structure.RotateCells(m_structure.rotation); }

    public Vector3Int GetPosition() { return m_structure.position; }

    public void SetPosition(Vector3Int newPos) { m_structure.position = newPos; }

    public int GetRotation() { return m_structure.rotation; }

    public void Rotate(bool clockwise)
    {
        m_structure.Rotate(clockwise);
    }

    #endregion
}

[System.Serializable]
public struct ItemStructure
{
    public ShapeData shape;
    public Vector3Int[] cells;
    public Vector3Int position;
    public int rotation;

    public void Initialise(ShapeData shape, Vector3Int position)
    {
        this.shape = shape;
        this.position = position;
        SetCells(shape);
    }

    public void SetCells(ShapeData shape)
    {
        if (shape == null || shape.GetCells() == null)
            return;
        
        Vector2Int[] tempCells = shape.GetCells();

        cells = new Vector3Int[tempCells.Length];

        for (int i = 0; i < tempCells.Length; i++)
        {
            cells[i] = (Vector3Int)tempCells[i];
        }
    }

    public void Rotate(bool clockwise)
    {
        //if rotation direction is pos, clockwise rotation
        if (clockwise)
            rotation = (rotation + 1) % 4;
        else
            rotation = (rotation + 3) % 4;
    }

    public Vector3Int[] RotateCells(int rot)
    {
        Vector3Int[] rotatedCells = new Vector3Int[cells.Length];

        for (int i = 0; i < cells.Length; i++)
        {
            rotatedCells[i] = Utilities.RotateVector3(cells[i], rot);
        }

        return rotatedCells;
    }
    
    public Vector3Int[] GetCells() { return RotateCells(rotation); }
    
    public Vector3Int GetAnchorOffset() { return Utilities.RotateVector3(shape.GetAnchorOffset(), rotation); }
}