using UnityEngine;

public class GridSnapping : MonoBehaviour
{
    //check if the corners of the items are in bounds
    public static bool CheckBoxInBounds(RectTransform itemRect, RectTransform gridRect, Vector2 gridWorldSize)
    {
        if (gridRect == null)
            return false;
        
        //get the corners of the the draggable item
        Vector3[] itemCorners = new Vector3[4];
        //convert item rect to world corners
        itemRect.GetWorldCorners(itemCorners);
        
        Vector3 center = gridRect.position;
        float halfWidth = gridWorldSize.x * 0.5f;
        float halfHeight = gridWorldSize.y * 0.5f;
        Rect gridBounds = new (center.x - halfWidth, center.y - halfHeight, gridWorldSize.x, gridWorldSize.y);

        //index through item corners to check if any corner
        //is in the switching zone rect
        for (int i = 0; i < itemCorners.Length; i++)
        {
            if (gridBounds.Contains(itemCorners[i]))
                return true;
        }

        return false;
    }

    //round given position to be aligned with the grid
    public static Vector2 GetGridPosition(Vector2 position, RectTransform gridRect, float cellSize, float tolerance)
    {
        //convert the position to be local to the grid
        Vector2 localPos = position - (Vector2)gridRect.localPosition;

        //snap position to nearest cell
        float snappedX = Mathf.Round(localPos.x / cellSize) * cellSize + (cellSize * 0.5f);
        float snappedY = Mathf.Round(localPos.y / cellSize) * cellSize + (cellSize * 0.5f);

        //find the distance from the nearest cell center
        float distanceX = Mathf.Abs(localPos.x - snappedX);
        float distanceY = Mathf.Abs(localPos.y - snappedY);

        //if the distance is within the tolerance snap to that cell
        if (distanceX <= tolerance)
            localPos.x = snappedX;

        if (distanceY <= tolerance)
            localPos.y = snappedY;

        //return position & convert back to screen positions
        return localPos + (Vector2)gridRect.localPosition;
    }

    //Vector2 localPos =
    //    position - (Vector2)gridRect.localPosition;

    //// convert position to grid coordinates
    //int column = Mathf.RoundToInt(localPos.x / cellSize);
    //int row = Mathf.RoundToInt(localPos.y / cellSize);

    //// clamp to valid cells
    //column = Mathf.Clamp(column, 0, gridSize - 1);
    //row = Mathf.Clamp(row, 0, gridSize - 1);

    //// convert back to position
    //float snappedX =
    //    (column * cellSize) + (cellSize * 0.5f);

    //float snappedY =
    //    (row * cellSize) + (cellSize * 0.5f);

    //return new Vector2(snappedX, snappedY)
    //    + (Vector2) gridRect.localPosition;
}
