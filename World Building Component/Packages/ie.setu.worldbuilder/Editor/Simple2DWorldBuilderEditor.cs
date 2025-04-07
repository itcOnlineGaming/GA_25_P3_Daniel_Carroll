using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR

[CustomEditor(typeof(Simple2DWorldBuilder))]
public class Simple2DWorldBuilderEditor : Editor
{
    private Vector2Int lastPaintCenter = new Vector2Int(int.MinValue, int.MinValue);
    private Vector2Int lastEraseCenter = new Vector2Int(int.MinValue, int.MinValue);

    private void OnSceneGUI()
    {
        Simple2DWorldBuilder builder = (Simple2DWorldBuilder)target;

        // Clean up null tiles
        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var kvp in builder.placedTiles)
        {
            if (kvp.Value == null)
                toRemove.Add(kvp.Key);
        }
        foreach (var cellKey in toRemove)
            builder.placedTiles.Remove(cellKey);

        Event e = Event.current;

        // Left click = PAINT
        if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
            && e.button == 0 && !e.alt && !e.control && !e.shift)
        {
            Vector2Int centerCell = GetCellUnderMouse(builder);

            // Only paint if mouse has moved to a new center cell
            if (centerCell != lastPaintCenter)
            {
                PaintArea(builder, centerCell);
                lastPaintCenter = centerCell;
            }

            e.Use();
        }
        else if (e.type == EventType.MouseUp && e.button == 0)
        {
            lastPaintCenter = new Vector2Int(int.MinValue, int.MinValue);
        }

        // Right click = ERASE
        if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
            && e.button == 1 && !e.alt && !e.control && !e.shift)
        {
            Vector2Int centerCell = GetCellUnderMouse(builder);

            // Only erase if mouse has moved to a new center cell
            if (centerCell != lastEraseCenter)
            {
                EraseArea(builder, centerCell);
                lastEraseCenter = centerCell;
            }

            e.Use();
        }
        else if (e.type == EventType.MouseUp && e.button == 1)
        {
            lastEraseCenter = new Vector2Int(int.MinValue, int.MinValue);
        }
    }


    /// Places tiles in all cells within brushRadius of centerCell
    private void PaintArea(Simple2DWorldBuilder builder, Vector2Int centerCell)
    {
        int r = builder.brushRadius;
        for (int dx = -r; dx <= r; dx++)
        {
            for (int dy = -r; dy <= r; dy++)
            {
                int col = centerCell.x + dx;
                int row = centerCell.y + dy;
                builder.PlaceTile(row, col);
            }
        }
    }


    /// Erases tiles in all cells within brushRadius of centerCell
    private void EraseArea(Simple2DWorldBuilder builder, Vector2Int centerCell)
    {
        int r = builder.brushRadius;
        for (int dx = -r; dx <= r; dx++)
        {
            for (int dy = -r; dy <= r; dy++)
            {
                int col = centerCell.x + dx;
                int row = centerCell.y + dy;

                Vector2Int cellKey = new Vector2Int(col, row);
                if (builder.placedTiles.ContainsKey(cellKey))
                {
                    GameObject tileToRemove = builder.placedTiles[cellKey];
#if UNITY_EDITOR
                    Undo.DestroyObjectImmediate(tileToRemove);
#else
                    DestroyImmediate(tileToRemove);
#endif
                }
            }
        }
    }


    /// Converts the Scene mouse position to (col, row) cell coordinates
    private Vector2Int GetCellUnderMouse(Simple2DWorldBuilder builder)
    {
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
        float colF = (mousePos.x - builder.transform.position.x) / builder.cellSize;
        float rowF = (mousePos.y - builder.transform.position.y) / builder.cellSize;
        int col = Mathf.FloorToInt(colF);
        int row = Mathf.FloorToInt(rowF);
        return new Vector2Int(col, row);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Simple2DWorldBuilder builder = (Simple2DWorldBuilder)target;

        if (builder.tilePrefabs != null && builder.tilePrefabs.Length > 0)
        {
            string[] tileNames = new string[builder.tilePrefabs.Length];
            for (int i = 0; i < builder.tilePrefabs.Length; i++)
            {
                tileNames[i] = builder.tilePrefabs[i] ? builder.tilePrefabs[i].name : "Null";
            }
            builder.selectedTileIndex = EditorGUILayout.Popup("Selected Tile", builder.selectedTileIndex, tileNames);
        }
        else
        {
            EditorGUILayout.HelpBox("No tile prefabs assigned.", MessageType.Warning);
        }
    }
}
#endif