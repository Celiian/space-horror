using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine.Rendering;

public class SoundPropagationManager : MonoBehaviour
{
    private static SoundPropagationManager _instance;
    public static SoundPropagationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SoundPropagationManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("SoundPropagationManager");
                    _instance = go.AddComponent<SoundPropagationManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    [FoldoutGroup("References")]
    [SerializeField] private Transform player;
    [FoldoutGroup("References")]
    [SerializeField] private Grid grid;
    [FoldoutGroup("References")]
    [SerializeField] private Tilemap wallTilemap;
    [FoldoutGroup("References")]
    [SerializeField] private Tilemap floorTilemap;

    [FoldoutGroup("Sound Settings")]
    [SerializeField] private int maxRadius = 10;
    [FoldoutGroup("Sound Settings")]
    [SerializeField] private float attenuationPerTile = 0.1f;
    [FoldoutGroup("Sound Settings")]
    [SerializeField] private float wallAttenuation = 0.5f;
    [FoldoutGroup("Sound Settings")]
    [SerializeField] private float minLevel = 0.01f;
    [FoldoutGroup("Sound Settings")]
    [SerializeField] private float updateInterval = 0.1f;
    [FoldoutGroup("Sound Settings")]
    [SerializeField] public float stepInterval = 0.8f;

    public List<Vector3Int> walls = new List<Vector3Int>();
    private List<Tile> tiles = new List<Tile>();
    public HashSet<Tile> activeTiles = new HashSet<Tile>();
    private Dictionary<Vector3Int, Tile> tilesByPosition;
    private float lastUpdateTime = 0f;

    private void Start()
    {
        tilesByPosition = new Dictionary<Vector3Int, Tile>();
        populateTiles();
    }

    private void populateTiles()
    {
        // Populate floor tiles
        foreach (Vector3Int tilePosition in floorTilemap.cellBounds.allPositionsWithin)
        {
            if (!floorTilemap.HasTile(tilePosition)) continue;
            
            var tile = new Tile(tilePosition, TileType.FLOOR, floorTilemap);
            tile.paint();
            tiles.Add(tile);
            tilesByPosition[tilePosition] = tile;
        }

        // Populate wall tiles (only those adjacent to floor)
        foreach (Vector3Int tilePosition in wallTilemap.cellBounds.allPositionsWithin)
        {
            if (!wallTilemap.HasTile(tilePosition)) continue;

            var tile = new Tile(tilePosition, TileType.WALL, wallTilemap);
            bool hasFloorNeighbor = false;
            tile.paint();
            
            foreach (Tile neighbor in GetNeighbors(tile))
            {
                if (neighbor != null && neighbor.type == TileType.FLOOR)
                {
                    hasFloorNeighbor = true;
                    break;
                }
            }

            if (hasFloorNeighbor)
            {
                tiles.Add(tile);
                tilesByPosition[tilePosition] = tile;
            }
        }
    }

    private void LateUpdate()
    {
        // Only update sound levels based on updateInterval
        if (Time.time - lastUpdateTime < updateInterval) return;
        lastUpdateTime = Time.time;

        // Only update active tiles
        foreach (Tile tile in activeTiles.ToList())
        {
            
            if (tile.soundSources.Count > 0)
            {
                if(tile.type == TileType.WALL)
                {
                    tiles.Remove(tile);
                    activeTiles.Remove(tile);
                    tilesByPosition.Remove(tile.position);
                }
                else {
                    List<SoundData> updatedSoundSources = new List<SoundData>();
                    foreach (SoundData soundData in tile.soundSources)
                    {
                        var soundLevel = soundData.soundLevel;
                        
                        float timeSinceUpdate = Time.time - soundData.lastSoundUpdate;
                        float decayFactor = Mathf.Exp(-timeSinceUpdate * 0.3f);
                        float newSoundLevel = soundLevel * decayFactor;

                        updatedSoundSources.Add(new SoundData(newSoundLevel, soundData.origin));
                    }

                    tile.soundSources.Clear();
                    tile.soundSources.AddRange(updatedSoundSources);

                    while (Time.time - tile.soundSources[0].lastSoundUpdate > stepInterval)
                    {
                        tile.soundSources.RemoveAt(0);
                        if(tile.soundSources.Count == 0)
                        {
                            activeTiles.Remove(tile);
                        }
                    }
                }
            }
           
            tile.paint();
            
        }
    }

    public void PropagateSound(Vector3 source, SoundOrigin origin, float volumeAttenuation = 1.0f)
    {
        Vector3Int sourceInt = grid.WorldToCell(source);
        if (!tilesByPosition.TryGetValue(sourceInt, out Tile sourceTile)) return;

        Queue<Tile> queue = new Queue<Tile>();
        Dictionary<Vector3Int, float> soundLevels = new Dictionary<Vector3Int, float>();

        queue.Enqueue(sourceTile);
        soundLevels[sourceTile.position] = 1.0f * volumeAttenuation;
        sourceTile.UpdateSoundLevel(1.0f * volumeAttenuation, origin);
        activeTiles.Add(sourceTile);

        while (queue.Count > 0)
        {
            Tile current = queue.Dequeue();
            float currentLevel = soundLevels[current.position];

            if (currentLevel <= minLevel) continue;

            // Make an interesting effect by propagating sound diagonally
            // Vector3Int.up + Vector3Int.right, Vector3Int.up + Vector3Int.left, Vector3Int.down + Vector3Int.right, Vector3Int.down + Vector3Int.left 
            Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right, Vector3Int.up + Vector3Int.right, Vector3Int.up + Vector3Int.left, Vector3Int.down + Vector3Int.right, Vector3Int.down + Vector3Int.left  };
            foreach (Vector3Int direction in directions)
            {
                Vector3Int neighborPos = current.position + direction;
                if (!tilesByPosition.TryGetValue(neighborPos, out Tile neighbor)) continue;

                float distanceFromSource = Vector3Int.Distance(sourceInt, neighborPos);
                if (distanceFromSource > maxRadius) continue;

                float attenuation = Mathf.Exp(-distanceFromSource * attenuationPerTile);
                if (neighbor.type == TileType.WALL || walls.Contains(neighborPos))
                {
                    attenuation *= Mathf.Exp(-wallAttenuation);
                }

                float newLevel = currentLevel * attenuation;
                if (!soundLevels.ContainsKey(neighborPos))
                {
                    soundLevels[neighborPos] = newLevel;
                    neighbor.UpdateSoundLevel(newLevel, origin);
                    activeTiles.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    private List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();
        Vector3Int[] positions = {
            tile.position + Vector3Int.up,
            tile.position + Vector3Int.down,
            tile.position + Vector3Int.left,
            tile.position + Vector3Int.right,
            tile.position + Vector3Int.up + Vector3Int.right,
            tile.position + Vector3Int.up + Vector3Int.left,
            tile.position + Vector3Int.down + Vector3Int.right,
            tile.position + Vector3Int.down + Vector3Int.left
        };

        foreach (Vector3Int pos in positions)
        {
            if (tilesByPosition.TryGetValue(pos, out Tile neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    public Vector3 getTilePosition(Vector3Int position)
    {
        return grid.CellToWorld(position);
    }

    public Tile getTileOnPosition(Vector3Int position)
    {
        return tilesByPosition.TryGetValue(position, out Tile tile) ? tile : null;
    }

    public Tile getTileOnPosition(Vector3 position)
    {
        Vector3Int positionInt = grid.WorldToCell(position);
        return tilesByPosition.TryGetValue(positionInt, out Tile tile) ? tile : null;
    }

    public Tile getClosestTileFromPosition(Vector3 position)
    {
        Vector3Int positionInt = grid.WorldToCell(position);
        Tile closestTile = null;
        float closestDistance = float.MaxValue;

        foreach (var kvp in tilesByPosition)
        {
            float distance = Vector3Int.Distance(positionInt, kvp.Key);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTile = kvp.Value;
            }
        }

        return closestTile;
    }

    public void addWall(Vector3 position)
    {
        Vector3Int positionInt = grid.WorldToCell(position);
        walls.Add(positionInt);
    }

    public void removeWall(Vector3 position)
    {
        Vector3Int positionInt = grid.WorldToCell(position);
        walls.Remove(positionInt);
    }
}
