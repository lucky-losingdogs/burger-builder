using System.Collections.Generic;
using UnityEngine;

public static class Data
{
    //<summary>
    // different cells for each rotation
    //</summary>
    public static readonly Dictionary<Ingredients, Vector2Int[]> Cells0 = new Dictionary<Ingredients, Vector2Int[]>()
    {
        { Ingredients.Bun, new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 2, 0), new Vector2Int( 3, 0) } },
        { Ingredients.Beef, new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int( 1, -1), new Vector2Int( 2, -1) } },
        { Ingredients.Lettuce, new Vector2Int[] { new Vector2Int( 2, 0), new Vector2Int(0, -1), new Vector2Int( 1, -1), new Vector2Int( 2, -1) } },
        { Ingredients.Tomato, new Vector2Int[] { new Vector2Int( 1, 0), new Vector2Int( 2, 0), new Vector2Int( 1, -1), new Vector2Int( 2, -1) } },
        { Ingredients.Onion, new Vector2Int[] { new Vector2Int( 1, 0), new Vector2Int( 2, 0), new Vector2Int(0, -1), new Vector2Int( 1, -1) } },
        { Ingredients.Chicken, new Vector2Int[] { new Vector2Int( 1, 0), new Vector2Int(0, -1), new Vector2Int( 1, -1), new Vector2Int( 2, -1) } },
        { Ingredients.Cheese, new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1, -1), new Vector2Int( 2, -1) } },
    };
}

public enum Ingredients
{
    Bun, Beef, Lettuce, Onion, Chicken, Tomato, Cheese
}