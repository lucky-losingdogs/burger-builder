using System.Collections.Generic;
using UnityEngine;
using Logic;

public static class Data
{
    public static readonly Dictionary<Ingredients, Vector2Int[]> Cells = new Dictionary<Ingredients, Vector2Int[]>()
    {
        { Ingredients.Bun, new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 2, 0), new Vector2Int( 3, 0) } },
        { Ingredients.Beef, new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int( 1, -1), new Vector2Int( 2, -1) } },
        { Ingredients.Lettuce, new Vector2Int[] { new Vector2Int( 2, 0), new Vector2Int(0, -1), new Vector2Int( 1, -1), new Vector2Int( 2, -1) } },
        { Ingredients.Tomato, new Vector2Int[] { new Vector2Int( 1, 0), new Vector2Int( 2, 0), new Vector2Int( 1, -1), new Vector2Int( 2, -1) } },
        { Ingredients.Onion, new Vector2Int[] { new Vector2Int( 1, 0), new Vector2Int( 2, 0), new Vector2Int(0, -1), new Vector2Int( 1, -1) } },
        { Ingredients.Chicken, new Vector2Int[] { new Vector2Int( 1, 0), new Vector2Int(0, -1), new Vector2Int( 1, -1), new Vector2Int( 2, -1) } },
        { Ingredients.Cheese, new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1, -1), new Vector2Int( 2, -1) } },
    };
    
    public static readonly Dictionary<PerkTypes, PerkLogic> Perks = new Dictionary<PerkTypes, PerkLogic>()
    {
        { PerkTypes.ClearAllTickets, new ClearAllTickets() },
        { PerkTypes.ClearAllOfType, new ClearAllOfType() },
        { PerkTypes.DecreaseComboDecrement, new DecreaseComboDecrement() },
        { PerkTypes.IncreaseComboLimit, new IncreaseComboLimit() },
        { PerkTypes.ComboMultiplier, new ComboMultiplier() },
        { PerkTypes.LowerItemCooldown, new LowerItemCooldown() },
        { PerkTypes.Null, new NullPerk() }
    };
}

public enum Ingredients
{
    Bun, Beef, Lettuce, Onion, Chicken, Tomato, Cheese
}

public enum PerkTypes
{
    ClearAllOfType, ClearAllTickets, DecreaseComboDecrement, IncreaseComboLimit, ComboMultiplier,
    LowerItemCooldown, Null
}