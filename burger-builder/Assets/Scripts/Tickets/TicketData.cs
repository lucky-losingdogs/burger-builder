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

    public Sprite GetSprite() { return sprite; }
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