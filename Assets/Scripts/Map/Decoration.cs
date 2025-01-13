using UnityEngine;
using System.Collections.Generic;

public class Decoration : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> positions;

    private List<Tile> tiles;
    private List<SpriteRenderer> spriteRenderers;
    private bool hasBeenSeen = false;
    public bool isBlocking = true;

    public float maxLightLevel = 1f;

    private void Start()
    {
        spriteRenderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
        tiles = new List<Tile>();
        foreach(GameObject position in positions)
        {   
            Tile tile = SoundPropagationManager.Instance.getClosestTileFromPosition(position.transform.position);
            tiles.Add(tile);

        }
    }

    private void LateUpdate()
    {
        float soundLevel = CalculateSoundLevel();

        if (soundLevel > 0 && !hasBeenSeen)
        {
            hasBeenSeen = checkHasBeenSeen();
        }

        Color color = DetermineColor(soundLevel);

        ApplyColorToSprites(color);
    }

    private bool checkHasBeenSeen()
    {
        foreach(Tile tile in tiles)
        {
            if(tile.hasBeenSeen)
                return true;
        }
        return false;
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
        float newSoundLevel = soundLevel > maxLightLevel ? maxLightLevel : soundLevel;
        Color color = new Color(newSoundLevel, newSoundLevel, newSoundLevel);


        if(!hasBeenSeen)
            color = Color.black;

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