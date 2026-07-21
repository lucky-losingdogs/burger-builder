using System.Collections.Generic;
using UnityEngine;

public static class Data
{
    //<summary>
    // different cells for each rotation
    //</summary>
    public static readonly Dictionary<Ingredients, Vector2Int[]> Cells0 = new Dictionary<Ingredients, Vector2Int[]>()
    {
        { Ingredients.Bun, new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int( 2, 1) } },
        { Ingredients.Beef, new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
        { Ingredients.Lettuce, new Vector2Int[] { new Vector2Int( 1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
        { Ingredients.Tomato, new Vector2Int[] { new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
        { Ingredients.Onion, new Vector2Int[] { new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0) } },
        { Ingredients.Chicken, new Vector2Int[] { new Vector2Int( 0, 1), new Vector2Int(-1, 0), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
        { Ingredients.Cheese, new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int( 0, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
    };

    public static Vector3Int RotateCell(Vector3Int cell, int rotation)
    {
        return rotation switch
        {
            0 => cell,
            1 => new Vector3Int(cell.y, -cell.x, cell.z),
            2 => new Vector3Int(-cell.x, -cell.y, cell.z),
            3 => new Vector3Int(-cell.y, cell.x, cell.z),
            _ => cell
        };
    }
}

public enum Ingredients
{
    Bun, Beef, Lettuce, Onion, Chicken, Tomato, Cheese
}