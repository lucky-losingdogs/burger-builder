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
    [Range(0, 10)]
    [field: SerializeField] private int difficulty;

    public bool CheckItemPositions(BoardRenderer boardRenderer)
    {
        int occupiedTilesCount = boardRenderer.GetOccupiedTilesCount();
        int matches = 0;

        //go through list of items
        for (int i = 0; i < ticketItems.Length; i++)
        {
            //check if the item map contains an item at the required position
            Item item = boardRenderer.CheckItem(ticketItems[i].position);

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

    public Sprite GetSprite() { return sprite; }
    public Sprite GetDiagram() { return diagramSprite; }
    public int GetDifficulty() { return difficulty; }

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