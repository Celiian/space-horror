using BehaviorTree;
using UnityEngine;
using System.Collections.Generic;

public class InvestigateSound : Node
{
    private Transform transform;
    private float previousSoundPositionVolume = 0;
    public InvestigateSound(Transform transform)
    {
        this.transform = transform;
        previousSoundPositionVolume = 0;
    }

    public override NodeState Evaluate()
    {
        float soundDetectionRadius = (float)GetData("hearingDetectionRadius");
        Vector3 position = transform.position;

        // Assuming you have a method to get all tiles in the game
        HashSet<Tile> allTiles = GetAllTiles();

        List<Tile> tilesInRadius = new List<Tile>();
        foreach (Tile tile in allTiles)
        {
            if (Vector3.Distance(position, SoundPropagationManager.Instance.getTilePosition(tile.position)) <= soundDetectionRadius)
            {
                SoundData soundData = tile.soundSources.Find(soundData => soundData.origin == SoundOrigin.PLAYER);
                if(soundData != null)
                {
                    tilesInRadius.Add(tile);
                }
            }
        }

        Tile loudestTile = null;
        float maxSoundVolume = 0;

        foreach(Tile tile in tilesInRadius)
        {
            float soundVolume = 0;
            foreach(SoundData soundData in tile.soundSources)
            {
                soundVolume += soundData.soundLevel;
            }
            if(soundVolume > maxSoundVolume)
            {
                maxSoundVolume = soundVolume;
                loudestTile = tile;
            }
        }
        float hearingVolumeThreshold = (float)GetData("hearingVolumeThreshold");
        if(loudestTile != null && maxSoundVolume >= hearingVolumeThreshold && maxSoundVolume > previousSoundPositionVolume)
        {
            previousSoundPositionVolume = maxSoundVolume;
            SetTopParentData("soundPosition", SoundPropagationManager.Instance.getTilePosition(loudestTile.position));
            return NodeState.SUCCESS;
        }
        
        if(GetData("soundPosition") != null)
        {
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }


    private HashSet<Tile> GetAllTiles()
    {
        return SoundPropagationManager.Instance.activeTiles;
    }

    
}