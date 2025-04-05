#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;  // For Dictionary

[ExecuteInEditMode]
public class Simple2DWorldBuilder : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float cellSize = 1.0f;

    [Header("Tile Placement")]
    public GameObject[] tilePrefabs;           // All the different tile types you can place
    [HideInInspector] public int selectedTileIndex = 0; // Which tile type is currently selected

    [Range(0, 5)]
    public int brushRadius = 0;
    // 0 = 1 cell (just the clicked cell)
    // 1 = 3×3 area (+1 cell in all directions)

    // A dictionary that keeps track of placed tiles: (col, row) = tile GameObject
    public Dictionary<Vector2Int, GameObject> placedTiles = new Dictionary<Vector2Int, GameObject>();

    // Assign a parent object for all spawned tiles to keep your hierarchy clean
    public Transform tilesParent;

    //REBUILDS THE DICTIONARY WHEN THE SCENE LOADS ----------------------------------------
    private void OnEnable()
    {
        // Rebuild the dictionary from existing tile objects.
        // This assumes that all placed tiles are children of tilesParent.
        placedTiles.Clear();
        if (tilesParent != null)
        {
            Tile2D[] tiles = tilesParent.GetComponentsInChildren<Tile2D>();
            foreach (Tile2D tile in tiles)
            {
                if (tile != null)
                {
                    // Use tile.cellCoord as the key
                    placedTiles[tile.cellCoord] = tile.gameObject;
                }
            }
        }
    }

    //DRAWS THE GRID ----------------------------------------------------------------------------------------
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        float totalWidth = gridWidth * cellSize;
        float totalHeight = gridHeight * cellSize;

        // Draw horizontal lines
        for (int row = 0; row <= gridHeight; row++)
        {
            float yPos = transform.position.y + (row * cellSize);
            Vector3 startPos = new Vector3(transform.position.x, yPos, 0);
            Vector3 endPos = new Vector3(transform.position.x + totalWidth, yPos, 0);
            Gizmos.DrawLine(startPos, endPos);
        }

        // Draw vertical lines
        for (int col = 0; col <= gridWidth; col++)
        {
            float xPos = transform.position.x + (col * cellSize);
            Vector3 startPos = new Vector3(xPos, transform.position.y, 0);
            Vector3 endPos = new Vector3(xPos, transform.position.y + totalHeight, 0);
            Gizmos.DrawLine(startPos, endPos);
        }
    }

    //PLACES A TILE ----------------------------------------------------------------------------------------------
    public void PlaceTile(int row, int col)
    {
        // Validate coordinates
        if (row < 0 || row >= gridHeight || col < 0 || col >= gridWidth)
            return;

        Vector2Int cellKey = new Vector2Int(col, row);

        // If theres already a tile here, remove it to allow replacement
        if (placedTiles.ContainsKey(cellKey))
        {
            GameObject oldTile = placedTiles[cellKey];
            if (oldTile != null)
            {
#if UNITY_EDITOR
                // Use Undo for Editor support (undo/redo)
                Undo.DestroyObjectImmediate(oldTile);
#else
                DestroyImmediate(oldTile);
#endif
            }
            placedTiles.Remove(cellKey);
        }

        // Safety check for prefab
        if (tilePrefabs == null || tilePrefabs.Length == 0 || selectedTileIndex >= tilePrefabs.Length)
        {
            Debug.LogWarning("No valid tile prefab selected!");
            return;
        }

        // Calculate the world position for the center of this cell
        float xPos = transform.position.x + (col + 0.5f) * cellSize;
        float yPos = transform.position.y + (row + 0.5f) * cellSize;
        Vector3 tilePosition = new Vector3(xPos, yPos, 0);

        // Instantiate the tile
        GameObject newTile = null;
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            // Keep prefab connection in the Editor
            newTile = PrefabUtility.InstantiatePrefab(tilePrefabs[selectedTileIndex]) as GameObject;
            if (newTile != null) newTile.transform.position = tilePosition;
        }
        else
        {
            newTile = Instantiate(tilePrefabs[selectedTileIndex], tilePosition, Quaternion.identity);
        }
#else
        newTile = Instantiate(tilePrefabs[selectedTileIndex], tilePosition, Quaternion.identity);
#endif

        // Parent the tile under tilesParent if assigned
        if (newTile != null && tilesParent != null)
        {
            newTile.transform.SetParent(tilesParent);
        }

        // Auto-fit the sprite to cell size
        if (newTile != null)
        {
            SpriteRenderer sr = newTile.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                Vector2 spriteSize = sr.sprite.bounds.size;
                float scaleX = cellSize / spriteSize.x;
                float scaleY = cellSize / spriteSize.y;
                newTile.transform.localScale = new Vector3(scaleX, scaleY, 1f);
            }

            // Attach or retrieve Tile2D for coordinate tracking
            Tile2D tileData = newTile.GetComponent<Tile2D>();
            if (tileData == null)
            {
                tileData = newTile.AddComponent<Tile2D>();
            }
            tileData.cellCoord = cellKey;
            tileData.builder = this;
        }

        // Record the new tile in the dictionary
        placedTiles[cellKey] = newTile;
    }

    // REMOVES TILE ------------------------------------------------------------------------------------------------
    public void RemoveTile(Vector2Int cellCoord)
    {
        if (placedTiles.ContainsKey(cellCoord))
        {
            placedTiles.Remove(cellCoord);
        }
    }

#if UNITY_EDITOR
    // When any public variable is changed in the Inspector, recalc all placed tiles
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            RecalculateAllTiles();
        }
    }
#endif

    //RECALCULATES ALL TILES -----------------------------------------------------------------------------------
    public void RecalculateAllTiles()
    {
        foreach (var kvp in placedTiles)
        {
            GameObject tileObj = kvp.Value;
            if (tileObj == null)
                continue;

            Tile2D tileData = tileObj.GetComponent<Tile2D>();
            if (tileData == null)
                continue;

            int col = tileData.cellCoord.x;
            int row = tileData.cellCoord.y;
            float xPos = transform.position.x + (col + 0.5f) * cellSize;
            float yPos = transform.position.y + (row + 0.5f) * cellSize;
            tileObj.transform.position = new Vector3(xPos, yPos, 0);

            SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                Vector2 spriteSize = sr.sprite.bounds.size;
                float scaleX = cellSize / spriteSize.x;
                float scaleY = cellSize / spriteSize.y;
                tileObj.transform.localScale = new Vector3(scaleX, scaleY, 1f);
            }
        }
    }
}
