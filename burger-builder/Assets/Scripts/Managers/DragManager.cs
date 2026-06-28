using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class DragManager : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private BoardRenderer m_boardRenderer;
    private Item m_currentItem;

    #region Set Up

    private void Start()
    {
        m_boardRenderer = GetComponentInParent<BoardRenderer>();
    }

    #endregion

    #region Dragging

    private void HandleClick(Vector2 mousePos)
    {
        //set cursor world pos
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        //get the item with a cell at that tile position
        Item currentItem = m_boardRenderer.CheckClickedItem(worldPos);
        m_currentItem = currentItem;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CheckRMB(eventData))
            return;

        HandleClick(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (CheckRMB(eventData) || m_currentItem == null)
            return;

        Vector3Int cellPos = ConvertMouseToGrid(eventData);

        //if the position has changed, the new pos != the current/old pos
        //clear tiles, update pos and set tiles again with the new pos
        if (m_currentItem.GetPosition() != cellPos)
        {
            //clear first to prevent checking overlap on own tiles
            m_boardRenderer.ClearTiles(m_currentItem);

            //check overlap before setting the new position
            OverlappedTile overlap = m_boardRenderer.CheckOverlappingTiles(m_currentItem, cellPos);

            m_currentItem.SetPosition(cellPos);
            m_boardRenderer.SetTiles(m_currentItem);

            //check overlap results and set ghost tiles on overlapped cells
            CheckOverlap(overlap);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (CheckRMB(eventData) || m_currentItem == null)
            return;

        CheckOverlapsOnDrop(eventData);
        CheckCompletedTicket();

        m_currentItem = null;
    }

    private bool CheckRMB(PointerEventData eventData)
    {
        //return if dragging with right click
        if (eventData.button == PointerEventData.InputButton.Right)
            return true;
        else
            return false;
    }

    private Vector3Int ConvertMouseToGrid(PointerEventData eventData)
    {
        //convert the mouse delta pos to world pos
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(eventData.position);
        mouseWorld.z = 0f;

        //convert world pos to cell pos using the tilemap in board renderer
        return m_boardRenderer.WorldToCell(mouseWorld);
    }

    private void CheckOverlap(OverlappedTile overlap)
    {
        if (overlap.GetOverlapping())
        {
            m_boardRenderer.SetGhostTile(overlap.GetPositions());
        }
    }

    private void CheckOverlapsOnDrop(PointerEventData eventData)
    {
        Vector3Int cellPos = ConvertMouseToGrid(eventData);

        //clear first to prevent checking overlap on own tiles
        m_boardRenderer.ClearTiles(m_currentItem);

        //check if the item is overlapping when dropped
        OverlappedTile dropOverlap = m_boardRenderer.CheckOverlappingTiles(m_currentItem, cellPos).Clone();
        bool spaceFound = true;

        if (dropOverlap.GetOverlapping())
        {
            //get the closest free space that can fit the cells in the item
            spaceFound = m_boardRenderer.FindClosestFreeSpace(cellPos, m_currentItem, out Vector3Int freeSpace);
            if (spaceFound)
            {
                //set the position and tiles to the closest free space
                m_currentItem.SetPosition(freeSpace);
            }
            else
            {
                //if no space was found, move far off grid to count as a change in overlap
                m_currentItem.SetPosition(new Vector3Int(999, 999, 999));
            }

            m_boardRenderer.SetTiles(m_currentItem);
            //check for overlap where the item was moved to
            //and restore the previously overlapped cells
            OverlappedTile movedOverlap = m_boardRenderer.CheckOverlappingTiles(m_currentItem, freeSpace);
            m_boardRenderer.UpdateOverlaps(dropOverlap, movedOverlap);
        }
        else
            m_boardRenderer.SetTiles(m_currentItem);

        if (spaceFound)
        {
            //clear the item if its been dropped off the grid
            m_boardRenderer.CheckDroppedOnGrid(ConvertMouseToGrid(eventData), m_currentItem);
        }
        else //if no space was found, clear the item
        {
            m_boardRenderer.ClearTiles(m_currentItem);
            StartCoroutine(m_boardRenderer.C_WaitDestroyItem(m_currentItem));
        }
    }

    private void CheckCompletedTicket()
    {
        TicketData currentTicket = GameManager.s_instance.GetCurrentTicket();
        if (currentTicket != null)
        {
            if (currentTicket.CheckItemPositions(m_boardRenderer))
                GameManager.s_instance.TicketCleared();
        }
    }

    #endregion
}