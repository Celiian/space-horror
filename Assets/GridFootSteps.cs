using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridFootSteps : MonoBehaviour
{
    public Tilemap tilemap;
    public Tilemap _wallTilemap;
    public TileBase groundTileBase;
    public TileBase groundTileIlluminated;
    public int maxRadius = 4;
    public float delay = 0.5f;
    private float timer = 0f;
    private List<Vector3Int> blockedCells = new List<Vector3Int>();
    private List<Vector3Int> paintedTiles = new List<Vector3Int>();

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= delay)
        {
            timer = 0f;
            CreateFootSteps();
        }
    }

    private void CreateFootSteps()
    {
        ResetTile();
        StartCoroutine(PaintTilesGradually());
    }

    private IEnumerator PaintTilesGradually()
    {
        Vector3Int cell = tilemap.WorldToCell(transform.position);
        tilemap.SetTile(cell, groundTileIlluminated);
        paintedTiles.Add(cell);
        int radius = 1;

        while (radius <= maxRadius)
        {
            List<Vector3Int> newTiles = new List<Vector3Int>();
            yield return new WaitForSeconds(0.02f);
            paintedTiles.ForEach(tile =>
            {
                GetAdjacentTiles(tile).ForEach(adjacentTile =>
                {
                    if (blockedCells.Contains(adjacentTile))
                        return;
                    if (paintedTiles.Contains(adjacentTile) || newTiles.Contains(adjacentTile))
                        return;
                    newTiles.Add(adjacentTile);
                    tilemap.SetTile(adjacentTile, groundTileIlluminated);
                });
            });

            paintedTiles.AddRange(newTiles);

            radius++;
        }
    }

    private List<Vector3Int> GetAdjacentTiles(Vector3Int tile)
    {
        List<Vector3Int> adjacentTiles = new List<Vector3Int>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                adjacentTiles.Add(new Vector3Int(tile.x + x, tile.y + y, tile.z));
            }
        }

        return adjacentTiles;
    }

    private void ResetTile()
    {
        foreach (var tile in paintedTiles)
        {
            tilemap.SetTile(tile, groundTileBase);
        }
        paintedTiles.Clear();
    }

    private void Start()
    {
        BoundsInt bounds = _wallTilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new(x, y, 0);
                if (_wallTilemap.HasTile(pos))
                    blockedCells.Add(new(x, y));
            }
        }
    }
}
