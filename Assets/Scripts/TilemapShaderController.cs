using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapShaderController : MonoBehaviour
{
    public Transform player;  // The player transform
    public Shader shader;     // The shader to be used
    private Material material;
    public Tilemap tilemap;   // Reference to the Tilemap component
    public float maxUpdateDistance = 10f;  // Maximum distance to update tiles

    
    void Start()
    {
        material = new Material(shader);
    }

    void Update()
    {
        // Loop through each tile in the Tilemap
        foreach (var tilePosition in GetTilePositions())
        {
            Vector3 worldPosition = tilemap.CellToWorld(tilePosition);
            
            // Check distance to player before processing
            if (Vector3.Distance(worldPosition, player.position) > maxUpdateDistance)
                continue;

            material.SetVector("_Center", player.position);
            tilemap.GetComponent<Renderer>().material = material;
        }
    }

    // Get all tile positions from the Tilemap (assuming Tilemap is populated)
    IEnumerable<Vector3Int> GetTilePositions()
    {
        // Loop through all tile positions in the Tilemap
        // You can adjust this to your specific tilemap structure if needed
        BoundsInt bounds = tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int gridPosition = new Vector3Int(x, y, 0); // Tile position in grid coordinates
                yield return gridPosition;
            }
        }
    }
}
