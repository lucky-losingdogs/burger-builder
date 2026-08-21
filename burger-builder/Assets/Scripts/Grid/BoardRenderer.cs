using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardRenderer : MonoBehaviour
{
    [SerializeField] private Tile m_ghostTile;
    
    private Tilemap m_tilemap;
    [SerializeField] private Vector2Int m_boardSize = new Vector2Int(6, 6);
    [SerializeField] private Vector2Int m_boardOffset = new Vector2Int(0, -1);
    private RectInt m_bounds;

    private ItemMap m_itemMap = new ItemMap();
    private OverlappedTile m_currentOverlap = new OverlappedTile();

    private bool m_itemInUse = false;

    #region Set Up

    private void Awake()
    {
        m_tilemap = GetComponentInChildren<Tilemap>();
        m_bounds = CalculateBounds();
    }

    private void OnEnable()
    {
        LevelManager.OnTicketCleared += ClearAllTiles;
        LevelManager.OnLevelEnd += ClearAllTiles;
    }
    
    private void OnDisable()
    {
        LevelManager.OnTicketCleared -= ClearAllTiles;
        LevelManager.OnLevelEnd -= ClearAllTiles;
    }

    #endregion

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
            //save the item in the map to track item positions
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
            //clear from item map
            m_itemMap.RemoveTile(position, item);
        }
    }

    public void ClearAllTiles()
    {
        m_tilemap.ClearAllTiles();

        foreach (List<Item> itemList in m_itemMap.GetValues())
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i] != null)
                    StartCoroutine(C_WaitDestroyItem(itemList[i]));
            }
        }
        
        m_itemMap.ResetMap();
    }

    //sets overlapping tiles to the ghost tile to indicate where overlapping tiles are
    public void SetGhostTile(List<Vector3Int> cellPositions)
    {
        foreach (Vector3Int position in cellPositions)
        {
            m_tilemap.SetTile(position, m_ghostTile);
        }
    }

    //return the top item at the tile position from the item map
    public Item CheckClickedItem(Vector3 cursorPos)
    {
        Vector3Int tilePos = WorldToCell(cursorPos);
        return m_itemMap.GetTopItem(tilePos);
    }

    public Item CheckItem(Vector3Int tilePos)
    {
        return m_itemMap.GetTopItem(tilePos);
    }

    //check if the currently dragged item is being moved to overlap with another item
    //and change the overlapping cells to ghost tiles to indicate overlap
    public OverlappedTile CheckOverlappingTiles(Item item, Vector3Int position)
    {
        m_itemInUse = true;
        
        OverlappedTile overlap = new OverlappedTile();

        foreach (Vector3Int cell in item.GetCells())
        {
            //check if there is already another item stored at that position
            Item checkedItem = m_itemMap.GetTopItem(cell + position);

            if (checkedItem != null && checkedItem != item)
            {
                overlap.SetOverlapping(true);
                overlap.AddPosition(cell + position);
            }
        }

        UpdateOverlaps(m_currentOverlap, overlap);
        m_currentOverlap = overlap;

        m_itemInUse = false;

        return overlap;
    }


    //compare the current overlapped positions and the new overlapped positions
    //to set tiles that are no longer being overlapped to non-ghost tiles
    public void UpdateOverlaps(OverlappedTile currentOverlap, OverlappedTile newOverlap)
    {
        List<Vector3Int> currentList = currentOverlap.GetPositions();
        List<Vector3Int> newList = newOverlap.GetPositions();

        //check if there's anything in the old list thats missing from the new list
        List<Vector3Int> exceptions = new List<Vector3Int>(currentList.Except(newList)).ToList();
        RestoreOverlapTiles(exceptions);
    }

    private void RestoreOverlapTiles(List<Vector3Int> overlapExceptions)
    {
        //if there are differences between the overlaps,
        //set a tile for each position that is no longer being overlapped
        for (int i = 0; i < overlapExceptions.Count; i++)
        {
            Item item = m_itemMap.GetTopItem(overlapExceptions[i]);
            if (item == null)
            {
                Debug.LogWarning($"No item found at {overlapExceptions[i]}");
                continue;
            }
            ShapeData tempData = item.GetShape();
            m_tilemap.SetTile(overlapExceptions[i], tempData.GetTile());
        }
    }

    public IEnumerator C_WaitDestroyItem(Item item)
    {
        yield return new WaitWhile(ItemInUse);
        Destroy(item.gameObject);
        m_itemInUse = false;
    }

    private bool ItemInUse() { return m_itemInUse; }

    public OverlappedTile GetCurrentOverlap() { return m_currentOverlap; }

    public Vector3Int WorldToCell(Vector3 vect3) { return m_tilemap.WorldToCell(vect3); }

    public ItemMap GetItemMap() { return m_itemMap; }

    public int GetOccupiedTilesCount() { return m_itemMap.GetKeys().Count; }

    public bool CheckOccupiedTile(Vector3Int pos) { return m_tilemap.HasTile(pos); }

    #endregion

    #region Bound Checks

    //calculate bounds of the grid using the grid size and the origin of the grid
    private RectInt CalculateBounds()
    {
        Vector2Int position = new Vector2Int(-m_boardSize.x / 2, -m_boardSize.y / 2) + m_boardOffset;
        return new RectInt(position, m_boardSize);
    }

    public void CheckDroppedOnGrid(Vector3Int position, Item item)
    {
        //go through each cell in the item and clear all the cells
        //if any have been dropped off the grid
        foreach (Vector3Int cell in item.GetCells())
        {
            if (!CheckInBounds(cell + position))
            {
                ClearTiles(item);
                CheckOverlappingTiles(item, position);
                StartCoroutine(C_WaitDestroyItem(item));
                return;
            }
        }
    }

    public bool FindClosestFreeSpace(Vector3Int startPos, Item item, out Vector3Int freeSpace)
    {
        Queue<Vector3Int> tileQueue = new Queue<Vector3Int>();
        HashSet<Vector3Int> visitedTiles = new HashSet<Vector3Int>();

        tileQueue.Enqueue(startPos);
        visitedTiles.Add(startPos);

        Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };
        freeSpace = Vector3Int.zero;

        while (tileQueue.Count > 0)
        {
            //get the tile at the top of the queue
            //and if its not stored in the item map its an empty cell
            Vector3Int currentTile = tileQueue.Dequeue();
            if (!m_itemMap.CheckOccupiedTile(currentTile))
            {
                bool freeCells = false;
                
                //check if there are enough free cells to fit every cell in the item
                foreach (Vector3Int cell in item.GetCells())
                {
                    Vector3Int worldPos = cell + currentTile;

                    //check if the space is in bounds or has a tile there already
                    //if there are any non free spaces, set freeeCells false and break early
                    if (!CheckInBounds(worldPos) || m_itemMap.CheckOccupiedTile(worldPos))
                    {
                        freeCells = false;
                        break;
                    }
                    else
                        freeCells = true;
                }

                if (freeCells)
                {
                    freeSpace = currentTile;
                    return true;
                }
            }

            //add every direction around the current tile to queue
            foreach (Vector3Int dir in directions)
            {
                Vector3Int neighbour = currentTile + dir;
                //prevent rechecking visited tiles
                if (CheckInBounds(neighbour) && visitedTiles.Add(neighbour))
                {
                    tileQueue.Enqueue(neighbour);
                }
            }
        }

        return false;
    }

    private bool CheckInBounds(Vector3Int position) { return m_bounds.Contains((Vector2Int)position); }

    #endregion
}

//<summary>
//stores positions of occupied tiles in dictionary with the items associated with the tiles
//</summary>
public class ItemMap
{
    private Dictionary<Vector3Int, List<Item>> m_occupiedTiles = new Dictionary<Vector3Int, List<Item>>();

    public void AddTile(Vector3Int pos, Item item)
    {
        if (!m_occupiedTiles.TryGetValue(pos, out List<Item> list))
        {
            list = new List<Item>();
            m_occupiedTiles[pos] = list;
        }

        m_occupiedTiles[pos].Add(item);
    }

    public void RemoveTile(Vector3Int pos, Item item)
    {
        if (!m_occupiedTiles.TryGetValue(pos, out List<Item> list))
            return;

        m_occupiedTiles[pos].Remove(item);

        if (list.Count == 0)
            m_occupiedTiles.Remove(pos);
    }

    public Item GetTopItem(Vector3Int pos)
    {
        if (!m_occupiedTiles.TryGetValue(pos, out List<Item> items) || items.Count == 0)
            return null;

        return items[^1];
    }

    public bool CheckOccupiedTile(Vector3Int pos) { return m_occupiedTiles.ContainsKey(pos); }

    public Dictionary<Vector3Int, List<Item>>.KeyCollection GetKeys() { return m_occupiedTiles.Keys; }
    public Dictionary<Vector3Int, List<Item>>.ValueCollection GetValues() { return m_occupiedTiles.Values; }

    public void ResetMap() { m_occupiedTiles = new Dictionary<Vector3Int, List<Item>>(); }
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

    public OverlappedTile Clone()
    {
        OverlappedTile clone = new OverlappedTile();

        clone.isOverlapping = isOverlapping;
        clone.positions = new List<Vector3Int>(positions);

        return clone;
    }
}