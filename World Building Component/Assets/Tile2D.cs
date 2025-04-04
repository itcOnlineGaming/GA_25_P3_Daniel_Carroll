using UnityEngine;

public class Tile2D : MonoBehaviour
{
    // Store the cell coordinate for this tile
    public Vector2Int cellCoord;

    // Reference to the builder that created this tile
    public Simple2DWorldBuilder builder;

    private void OnDestroy()
    {
        // When the tile is destroyed, remove it from the dictionary
        if (builder != null)
        {
            builder.RemoveTile(cellCoord);
        }
    }
}
