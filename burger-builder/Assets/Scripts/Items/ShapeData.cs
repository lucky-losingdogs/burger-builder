using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[CreateAssetMenu(fileName = "ShapeData", menuName = "Scriptable Objects/ShapeData")]
public class ShapeData : ScriptableObject
{
    [field: SerializeField] private Tile tile;
    [field: SerializeField] private Shapes shape;

    public Tile GetTile() { return tile; }

    public Shapes GetShape() { return shape; }

    public Vector2Int[] GetCells() { return shape.cells; }
}

public enum Ingredients
{ 
    Bun, Beef, Lettuce, Onion
}

[Serializable]
public struct Shapes
{
    public Ingredients ingredient;
    public Vector2Int[] cells;
}