using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "TicketData", menuName = "Scriptable Objects/TicketData")]
public class TicketData : ScriptableObject
{
    [field: SerializeField] private ItemStructure[] ticketItems;
    [field: SerializeField] private Sprite sprite;
    [field: SerializeField] private Sprite diagramSprite;
    [Range(1, 5)]
    [field: SerializeField] private int difficulty;

    private Vector3Int globalAnchorOffset = new Vector3Int(-2, -3, 0);

    public bool CheckItemCount(BoardRenderer boardRenderer)
    {
        int itemCount = boardRenderer.GetComponentsInChildren<Item>().Length;
        return itemCount == ticketItems.Length;
    }
    
    public bool CheckItemPositions(BoardRenderer boardRenderer)
    {
        int occupiedTilesCount = boardRenderer.GetOccupiedTilesCount();
        int matches = 0;

        //go through list of items
        for (int i = 0; i < ticketItems.Length; i++)
        {
            //check if the item map contains an item at the required position
            Vector3Int ticketPos = ConvertTicketPositions(ticketItems[i].position, ticketItems[i].GetAnchorOffset());
            Item item = boardRenderer.CheckItem(ticketPos);

            if (item != null)
            {
                
                
                
                Debug.Log("item pos"+item.GetPosition());
                Debug.Log("item rot"+item.GetRotation());
            }
            
            //check if the shape of the item on the map matches the required item & required rotation
            if (item != null && ticketItems[i].shape == item.GetShape() && ticketItems[i].rotation == item.GetRotation())
            {
                matches++;

                //for each cell in the item, subtract from the number of total tiles there should be
                foreach (Vector3Int cell in ticketItems[i].cells)
                {
                    occupiedTilesCount--;
                }
            }
        }

        //if the number of correct items is the number of required items
        return matches == ticketItems.Length && occupiedTilesCount == 0;
    }

    //convert from ticket ui tilemap coords into actual tilemap coords
    private Vector3Int ConvertTicketPositions(Vector3Int ticketPos, Vector3Int anchorOffset)
    {
        Vector3Int result = ticketPos + new Vector3Int(-2, -3, 0) + anchorOffset;
    
        Debug.Log($"TicketPos: {ticketPos}, Anchor: {anchorOffset} Result: {result}");

        return result;
    }

    public Sprite GetSprite() { return sprite; }
    public Sprite GetDiagram() { return diagramSprite; }
    public int GetDifficulty() { return difficulty; }
    public ItemStructure[] GetItems() { return ticketItems; }

#if UNITY_EDITOR

    private void OnValidate()
    {
        SetStructureCells();
    }

    //initialise the cells in the item structures
    private void SetStructureCells()
    {
        for (int i = 0; i < ticketItems.Length; i++)
        {
            ticketItems[i].SetCells(ticketItems[i].shape);
        }
    }

#endif
}