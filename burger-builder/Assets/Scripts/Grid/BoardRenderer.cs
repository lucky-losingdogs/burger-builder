using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class BoardRenderer : MonoBehaviour
{
    [SerializeField] private Tile m_ghostTile;
    
    private Tilemap m_tilemap;
    private List<Item> m_allItems = new List<Item>();

    private ItemMap m_itemMap = new ItemMap();
    private OverlappedTile m_currentOverlap = new OverlappedTile();

    private void Awake()
    {
        m_tilemap = GetComponentInChildren<Tilemap>();
    }

    #region Tile Rendering

    //set tiles in the position of the cells stored in the draggable's shape
    public void SetTiles(Item item)
    {
        Vector3Int[] tempCells = item.GetCells();
        ShapeData tempData = item.GetShape();

        for (int i = 0; i < tempCells.Length; i++)
        {
            Vector3Int position = tempCells[i] + item.GetPosition();
            m_tilemap.SetTile(position, tempData.GetTile());

            m_itemMap.AddTile(position, item);
        }
    }

    //clear all the tiles of the draggable's cells
    public void ClearTiles(Item item)
    {
        Vector3Int[] tempCells = item.GetCells();
        ShapeData tempData = item.GetShape();

        for (int i = 0; i < tempCells.Length; i++)
        {
            Vector3Int position = tempCells[i] + item.GetPosition();
            m_tilemap.SetTile(position, null);

            m_itemMap.RemoveTile(position);
        }
    }

    public void SetGhostTile(List<Vector3Int> cellPositions)
    {
        foreach (Vector3Int position in cellPositions)
        {
            m_tilemap.SetTile(position, m_ghostTile);
        }
    }

    public Item CheckClickedItem(Vector3 cursorPos)
    {
        //check if there is a tile at the cursor position
        Vector3Int tilePos = WorldToCell(cursorPos);
        return m_itemMap.GetItem(tilePos);
    }

    //check if the currently dragged item is being moved to overlap with another item
    //and change the overlapping cells to ghost tiles to indicate overlap
    public OverlappedTile CheckOverlappingTiles(Item item, Vector3Int position)
    {
        OverlappedTile overlap = new OverlappedTile();

        foreach (Vector3Int cell in item.GetCells())
        {
            Item checkedItem = m_itemMap.GetItem(cell + position);

            //check if the tilemap already has a tile there (another item in that position)
            if (checkedItem != null && checkedItem != item)
            {
                overlap.SetOverlapping(true);
                overlap.AddPosition(cell + position);
            }
        }

        RestoreOverlapTiles(overlap);
        m_currentOverlap = overlap;

        return overlap;
    }

    //compare the current overlapped positions and the new overlapped positions
    //to set tiles that are no longer being overlapped
    private void RestoreOverlapTiles(OverlappedTile newOverlap)
    {
        List<Vector3Int> oldList = m_currentOverlap.GetPositions();
        List<Vector3Int> newList = newOverlap.GetPositions();
        for (int i = 0; i < oldList.Count; i++)
        {
            Debug.Log($"oldList: {oldList[i]}");
        }
        for (int i = 0; i < newList.Count; i++)
        {
            Debug.Log($"newList: {newList[i]}");
        }

        //check if there's anything in the old list thats missing from the new list
        List<Vector3Int> exceptions = new List<Vector3Int>(oldList.Except(newList)).ToList();

        //if there are differences between the lists set a tile
        //for each position that is no longer being overlapped
        for (int i = 0; i < exceptions.Count; i++)
        {
            Item bottomItem = m_itemMap.GetItem(exceptions[i]); //nullreferror??
            if (bottomItem == null)
            {
                Debug.LogWarning($"No item found at {exceptions[i]}");
                continue;
            }
            ShapeData tempData = bottomItem.GetShape();
            m_tilemap.SetTile(exceptions[i], tempData.GetTile());
        }
    }

    public Vector3Int WorldToCell(Vector3 vect3) { return m_tilemap.WorldToCell(vect3); }

    #endregion

    #region Manage List

    public void AddItem(Item item) { m_allItems.Add(item); }

    public void RemoveItem(Item item) { m_allItems.Remove(item); }

    #endregion
}

//<summary>
//stores positions of occupied tiles in dictionary with the items associated with the tiles
//</summary>
public class ItemMap
{
    private Dictionary<Vector3Int, Item> m_occupiedTiles = new Dictionary<Vector3Int, Item>();

    public void AddTile(Vector3Int pos, Item item) { m_occupiedTiles.Add(pos, item); }

    public void RemoveTile(Vector3Int pos)
    {
        if (m_occupiedTiles.ContainsKey(pos))
            m_occupiedTiles.Remove(pos);
    }

    public bool CheckOccupiedTile(Vector3Int pos) { return m_occupiedTiles.ContainsKey(pos); }

    public Item GetItem(Vector3Int pos)
    {
        m_occupiedTiles.TryGetValue(pos, out Item item);
        return item;
    }
}

//<summary>
//stores if there is an overlap and the positions where there are tile overlaps
//</summary>
public class OverlappedTile
{
    private bool isOverlapping;
    private List<Vector3Int> positions = new List<Vector3Int>();

    public bool GetOverlapping() { return isOverlapping; }
    public void SetOverlapping(bool overlap) { isOverlapping = overlap; }
    public List<Vector3Int> GetPositions() { return positions; }
    public Vector3Int GetPosition(int i) { return positions[i]; }
    public void AddPosition(Vector3Int newPos) { positions.Add(newPos); } 
    public void ClearPositions() { positions.Clear(); }

}