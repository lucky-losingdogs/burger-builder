using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Linq;

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
    
    [field: SerializeField] private SymmetricalRotations[] symmetricalRotations;

    public Tile GetTile() { return tile; }
    public Tile GetGhostTile() { return ghostTile; }
    public Vector2Int[] GetCells() { return cells; }
    public Vector3Int GetAnchorOffset() { return anchorOffset; }

    public int[][] GetSymmetricalRotations()
    {
        return symmetricalRotations.Select(x => x.symmetricalRotations).ToArray();
    }

    [Serializable]
    private struct SymmetricalRotations
    {
        public int[] symmetricalRotations;
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        SetCells();
    }

    private void SetCells()
    {
        Data.Cells.TryGetValue(ingredient, out cells);
    }

#endif
}