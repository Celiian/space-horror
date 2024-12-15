using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WallDecoration : MonoBehaviour
{
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private bool minimumLight = false;
    private SoundPropagationManager soundManager;

    private void Start()
    {
        soundManager = SoundPropagationManager.Instance;
    }

    private void PaintTiles()
    {
        foreach (Vector3Int position in wallTilemap.cellBounds.allPositionsWithin)
        {
            if (!wallTilemap.HasTile(position)) continue;
            var soundLevel = 0f;

            var neighbors = soundManager.GetNeighbors(position);
            foreach (Tile neighbor in neighbors)
            {
                if (neighbor.type == TileType.FLOOR)
                {
                    soundLevel = CalculateSoundLevel(neighbor);
                    break;
                }
            }

            Color color = DetermineColor(soundLevel);

            wallTilemap.SetTileFlags(position, TileFlags.None);
            wallTilemap.SetColor(position, color);
        }
    }

    private float CalculateSoundLevel(Tile tile)
    {
        float soundLevel = 0f;
        foreach (SoundData soundData in tile.soundSources)
        {
            soundLevel += soundData.soundLevel;
        }
        soundLevel = Mathf.Clamp01(soundLevel);

        // Ensure the sound level is at least 0.1 if the tile has been seen
        if (tile.hasBeenSeen && soundLevel < 0.1f && minimumLight)
        {
            soundLevel = 0.5f;
        }
        
        return soundLevel;
    }


    private Color DetermineColor(float soundLevel)
    {
        return Color.Lerp(Color.black, Color.white, soundLevel);
    }

    private void LateUpdate()
    {
        PaintTiles();
    }
}
