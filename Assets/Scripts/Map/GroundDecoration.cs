using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundDecoration : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
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
            Color color = DetermineColor(soundLevel, tile);

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

        return soundLevel;
    }

    

    private Color DetermineColor(float soundLevel, Tile tile)
    {
        return tile.hasBeenSeen ? new Color(soundLevel, soundLevel, soundLevel) : Color.black;
    }

    private void LateUpdate()
    {
        PaintTiles();
    }
}
