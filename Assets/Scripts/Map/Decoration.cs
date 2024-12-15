using UnityEngine;
using System.Collections.Generic;

public class Decoration : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> positions;

    private List<Tile> tiles;
    private List<SpriteRenderer> spriteRenderers;
    private bool hasBeenSeen = false;

    private void Start()
    {
        spriteRenderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
        tiles = new List<Tile>();
        foreach(GameObject position in positions)
        {   
            Tile tile = SoundPropagationManager.Instance.getClosestTileFromPosition(position.transform.position);
            tiles.Add(tile);
            PathFinding.Instance.AddObstacle(tile.position);
        }
    }

    private void LateUpdate()
    {
        float soundLevel = CalculateSoundLevel();

        if (soundLevel > 0 && !hasBeenSeen)
        {
            hasBeenSeen = true;
        }

        Color color = DetermineColor(soundLevel);

        ApplyColorToSprites(color);
    }

    private float CalculateSoundLevel()
    {
        float totalSoundLevel = 0f;
        int soundDataCount = 0;

        foreach (Tile tile in tiles)
        {
            foreach (SoundData soundData in tile.soundSources)
            {
                totalSoundLevel += soundData.soundLevel;
                soundDataCount++;
            }
        }

        if (soundDataCount == 0) return 0f;

        float averageSoundLevel = totalSoundLevel / soundDataCount;
        return Mathf.Clamp01(averageSoundLevel);
    }

    private Color DetermineColor(float soundLevel)
    {
        Color color = Color.Lerp(Color.black, new Color(0.5f, 0.5f, 0.5f), soundLevel);

        if (hasBeenSeen && soundLevel <= 0.1f)
        {
            color = Color.Lerp(Color.black, Color.white, 0.05f);
        }

        return color;
    }

    private void ApplyColorToSprites(Color color)
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = color;
        }
    }
}