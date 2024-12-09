using UnityEngine;
using System.Collections.Generic;

public class Decoration : MonoBehaviour
{
    private Tile tile;
    private List<SpriteRenderer> spriteRenderers;
    private bool hasBeenSeen = false;

    private void Start()
    {
        spriteRenderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
    }

    private void LateUpdate()
    {
        if (tile == null)
        {
            tile = SoundPropagationManager.Instance.getTileOnPosition(transform.position);
        }

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
        float soundLevel = 0f;
        foreach (SoundData soundData in tile.soundSources)
        {
            soundLevel += soundData.soundLevel;
        }
        return Mathf.Clamp01(soundLevel);
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