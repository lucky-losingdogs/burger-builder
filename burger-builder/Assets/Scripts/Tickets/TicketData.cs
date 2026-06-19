using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TicketData", menuName = "Scriptable Objects/TicketData")]
public class TicketData : ScriptableObject
{
    [field: SerializeField] private ItemStructure[] ticketItems;
    [field: SerializeField] private Sprite sprite;
    [field: SerializeField] private float duration;

    public bool CheckItemPositions(ItemMap itemMap)
    {
        Debug.Log("beginning item check");
        
        List<bool> checks = new List<bool>();
        
        foreach (ItemStructure structure in ticketItems)
        {
            Item item = itemMap.GetTopItem(structure.position);
            if (item != null)
            {
                if (structure.shape == item.GetShape())
                {
                    Debug.Log("item in correct position!");
                    checks.Add(true);
                }
            }
        }
        Debug.Log("item check results: " + ( checks.Count == ticketItems.Length));
        return checks.Count == ticketItems.Length;
    }

    public Sprite GetSprite() { return sprite; }
}