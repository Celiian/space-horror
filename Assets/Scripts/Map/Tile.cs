using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SoundData
{
    public float lastSoundUpdate;
    public float soundLevel;
    public SoundOrigin origin;

    public SoundData(float soundLevel, SoundOrigin origin)
    {
        this.lastSoundUpdate = Time.time;
        this.soundLevel = soundLevel;
        this.origin = origin;
    }
}

public class Tile
{
    public Vector3Int position;
    public TileType type;
    public Tilemap tilemap;
    public List<SoundData> soundSources = new List<SoundData>();
    public bool hasBeenSeen = false;
    public Tile(Vector3Int position, TileType type, Tilemap tilemap)
    {
        this.tilemap = tilemap;
        this.position = position;
        this.type = type;
    }

    public void paint(bool debug = false)
    {
        Color color = Color.black;
        var soundLevel = 0f;
        foreach(SoundData soundData in soundSources)
        {
            soundLevel += soundData.soundLevel;
        }
        soundLevel = Mathf.Clamp01(soundLevel);
        switch (type)
        {
            case TileType.FLOOR:
                color = Color.Lerp(Color.black, Color.white, soundLevel);
                break;
            case TileType.WALL:
                if (soundLevel > 0)
                    color = new Color(0.35f, 0.35f, 0.35f);
                break;
        }

        bool playerInRange = CalcUtils.DistanceToTarget(position, PlayerMovement.Instance.transform.position) > PlayerMovement.Instance.hearingRadius * 1.4f;

        if(!debug && type == TileType.FLOOR && !playerInRange)
        {
            color = Color.black;
        }
        
        tilemap.SetTileFlags(position, TileFlags.None);
        tilemap.SetColor(position, color);
    }

    public void UpdateSoundLevel(float newSoundLevel, SoundOrigin origin)
    {
        SoundData soundData = soundSources.Find(soundData => soundData.origin == origin);
        if (soundData != null)
        {
            soundData.soundLevel = newSoundLevel;
            soundData.lastSoundUpdate = Time.time;
        }
        else
        {
            soundSources.Add(new SoundData(newSoundLevel, origin));
        }

        if(origin == SoundOrigin.PLAYER)
            hasBeenSeen = true;
    }
}



public enum TileType
{
    FLOOR,
    WALL
}


public enum SoundOrigin
{
    PLAYER,
    ZOMBIE,
    INTERACTIBLE,
    ITEM,
}