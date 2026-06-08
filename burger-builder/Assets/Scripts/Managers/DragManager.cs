using UnityEngine;
using UnityEngine.EventSystems;

public class DragManager : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private float m_dragSpeed = 1.7f;

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
        if (CheckRMB(eventData))
            return;

        if (m_currentItem == null)
            return;

        //convert the mouse delta pos to world pos
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(eventData.position);
        mouseWorld.z = 0f;

        //convert world pos to cell pos using the tilemap in board renderer
        Vector3Int cellPos = m_boardRenderer.WorldToCell(mouseWorld);

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
        if (CheckRMB(eventData))
            return;

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

    private void CheckOverlap(OverlappedTile overlap)
    {
        if (overlap.GetOverlapping())
        {
            m_boardRenderer.SetGhostTile(overlap.GetPositions());
        }
    }

    #endregion

    //clamps the movement of the draggable to the bounds of the bg/screen
    //public Vector2 ClampToBounds(Vector2 targetPos, RectTransform item)
    //{
    //    //bounds panel corners
    //    Vector3[] panelCorners = new Vector3[4];
    //    m_defaultPanel.GetWorldCorners(panelCorners);

    //    //get the draggable corners
    //    Vector3[] itemCorners = new Vector3[4];
    //    item.GetWorldCorners(itemCorners);

    //    float itemWidth = itemCorners[2].x - itemCorners[0].x;
    //    float itemHeight = itemCorners[2].y - itemCorners[0].y;

    //    //convert target position to world position
    //    Vector3 worldPos = item.parent.TransformPoint(targetPos);

    //    //adjust bounds to account for item size + pivot
    //    float minX = panelCorners[0].x + itemWidth * (item.pivot.x * 1.5f);
    //    float maxX = panelCorners[2].x - itemWidth * (1 - (item.pivot.x / 1.5f));

    //    float minY = panelCorners[0].y + itemHeight * (item.pivot.y * 1.5f);
    //    float maxY = panelCorners[2].y - itemHeight * (1 - (item.pivot.y / 1.5f));

    //    //clamp the position that the item can be moved to
    //    //to prevent leaving bounds
    //    worldPos.x = Mathf.Clamp(worldPos.x, minX, maxX);
    //    worldPos.y = Mathf.Clamp(worldPos.y, minY, maxY);

    //    return item.parent.InverseTransformPoint(worldPos);
    //}

    //private void SnapToGrid(Draggable draggable)
    //{
    //    RectTransform rect = draggable.GetComponent<RectTransform>();
    //    Vector2 gridLengths = m_gridGenerator.GetGridWorldSize();

    //    //if the item is dragged within the bounds of the grid
    //    //snap the target pos to match the grid
    //    if (GridSnapping.CheckBoxInBounds(rect, m_gridRect,gridLengths))
    //    {
    //        rect.position = GridSnapping.GetGridPosition(rect.position, m_gridRect, m_cellSize, m_snapTolerance);
    //    }
    //}
}