using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class TilemapUI : MonoBehaviour
{
    public List<List<Image>> m_tileImages;
    [SerializeField] private GridLayoutGroup m_grid;

    public void Show()
    {
        ToggleTilemap(true);
    }
    
    public void Hide()
    {
        ToggleTilemap(false);
    }

    private void ToggleTilemap(bool show)
    {
        if (show)
        {
            m_grid.gameObject.SetActive(true);
        }
        else
        {
            m_grid.gameObject.SetActive(false);
        }
    }
    
    public void SetGridDiagram(ItemStructure[] ticketItems)
    {
        for (int i = 0; i < ticketItems.Length; i++)
        {
            PlaceItem(ticketItems[i]);
        }
    }
    
    //set tiles in the position of the cells stored in the draggable's shape
    private void PlaceItem(ItemStructure item)
    {
        Vector3Int[] tempCells = item.GetCells();
        ShapeData tempData = item.shape;

        for (int i = 0; i < tempCells.Length; i++)
        {
            Vector3Int position = tempCells[i] + item.position;
            SetTile(position, tempData.GetTile().sprite);
        }
    }
    
    //sets the sprite at the given grid coordinate (column, row)
    private void SetTile(Vector3Int position, Sprite sprite)
    {
        int x = position.x;
        int y = position.y;
        
        //check pos is within bounds
        if (y < 0 || y >= m_tileImages.Count)
            return;
        if (x < 0 || x >= m_tileImages[y].Count)
            return;
        
        
        Image img = m_tileImages[y][x];
    
        if (sprite == null)
        {
            img.enabled = false;
        }
        else
        {
            img.enabled = true;
            img.sprite = sprite;
        }
    }

    //Clears the grid by setting all tiles to null
    public void ClearGrid()
    {
        foreach (List<Image> row in m_tileImages)
        {
            foreach (Image img in row)
            {
                img.enabled = false;
            }
        }
    }   

    public void SetTilemap()
    {
        m_tileImages = new List<List<Image>>();
        
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)m_grid.transform);
        
        int count = m_grid.transform.childCount;
        if (count == 0)
            return;
        
        float lastY = ((RectTransform)m_grid.transform.GetChild(0)).anchoredPosition.y;
        List<Image> currentRow = new List<Image>();

        for (int i = 0; i < count; i++)
        {
            RectTransform tile = (RectTransform)m_grid.transform.GetChild(i);
            float currentY = tile.anchoredPosition.y;
            
            //if y coord has changed, new row reached
            if (currentY > lastY)
            {
                m_tileImages.Add(currentRow);
                currentRow =  new List<Image>();
            }

            Image img = tile.GetComponent<Image>();
            if (img != null)
                currentRow.Add(img);
            else
                Debug.LogError("Grid child missing Image component!");
            lastY = currentY;
            
        }
        
        if (currentRow.Count > 0)
            m_tileImages.Add(currentRow);
    }

    public void RebuildLayout(Vector2 size, Vector2 position)
    {
        RectTransform gridRect = m_grid.GetComponent<RectTransform>();
        if (gridRect == null)
            return;
        
        Vector2 originalCellSize = m_grid.cellSize;
        
        //adjust size and pos to match the parent ticket's
        gridRect.sizeDelta = size * 0.8f;
        gridRect.position = position;
        
        //change the cell size of the grid based on the scale of the increase in size
        float scale = (size.x / originalCellSize.x) * 0.2f;
        m_grid.cellSize = new Vector2(originalCellSize.x * scale, originalCellSize.y * scale);
        LayoutRebuilder.ForceRebuildLayoutImmediate(gridRect);
    }
    
    public GridLayoutGroup GetGrid() { return m_grid; }
}
