using UnityEngine;
using UnityEngine.Tilemaps;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "ShapeData", menuName = "Scriptable Objects/ShapeData")]
public class ShapeData : ScriptableObject
{
    [field: SerializeField] private Tile tile;
    [field: SerializeField] private Tile ghostTile;
    [field: SerializeField] private Ingredients ingredient;
    [SerializeField] private Vector2Int[] cells;
    [field: SerializeField] private Vector3Int anchorOffset; 

    public Tile GetTile() { return tile; }
    public Tile GetGhostTile() { return ghostTile; }
    public Vector2Int[] GetCells() { return cells; }
    public Vector3Int GetAnchorOffset() { return anchorOffset; }


#if UNITY_EDITOR

    private void OnValidate()
    {
        SetCells();
    }

    private void SetCells()
    {
        Data.Cells0.TryGetValue(ingredient, out cells);
    }

#endif
}