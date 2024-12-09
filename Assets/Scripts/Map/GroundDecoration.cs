using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundDecoration : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private bool minimumLight = false;
    private SoundPropagationManager soundManager;

    private void Start()
    {
        soundManager = SoundPropagationManager.Instance;
    }

    private void PaintTiles()
    {
        foreach (Vector3Int position in groundTilemap.cellBounds.allPositionsWithin)
        {
            if (!groundTilemap.HasTile(position)) continue;

            Tile tile = soundManager.getTileOnPosition(position);
            if (tile == null) continue;

            float soundLevel = CalculateSoundLevel(tile);
            Color color = DetermineColor(soundLevel);

            groundTilemap.SetTileFlags(position, TileFlags.None);
            groundTilemap.SetColor(position, color);
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
            soundLevel = 0.1f;
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
