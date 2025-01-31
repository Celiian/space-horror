using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PlayerDangerDetector : MonoBehaviour
{
    [SerializeField] private AudioClip dangerBip;
    [SerializeField] public float dangerDistance = 10f;
    [SerializeField] public GameObject dangerIndicator;
    [SerializeField] private Material dangerIndicatorMaterialPreset;


    private List<Zombie> zombies;
    private Angel angel;
    private List<Entity> entities;
    private float beepInterval = 3f; // Maximum interval
    private float minBeepInterval = 0.5f; // Minimum interval
    private float lastBeepTime;
    private Material dangerIndicatorMaterial;
    public float closestDistance ;


    private void Start()
    {
        closestDistance = dangerDistance;
        zombies = FindObjectsOfType<Zombie>().ToList();
        angel = FindObjectOfType<Angel>();
        entities = new List<Entity>();
        entities.AddRange(zombies);
        entities.Add(angel);
        dangerIndicator.GetComponent<SpriteRenderer>().material = dangerIndicatorMaterialPreset; 
        dangerIndicatorMaterial = dangerIndicator.GetComponent<SpriteRenderer>().material;
    }

    private void Update()
    {
        Entity closestEntity = entities
            .Where(entity => !entity.isPaused)
            .Select(entity => new { Entity = entity, Distance = CalcUtils.DistanceToTarget(entity.transform.position, transform.position) })
            .OrderBy(e => e.Distance)
            .FirstOrDefault()?.Entity;

        if(!closestEntity) return;

        closestDistance = CalcUtils.DistanceToTarget(closestEntity.transform.position, transform.position);

        if (closestDistance < dangerDistance)
        {
            // Calculate the beep interval based on the distance
            beepInterval = Mathf.Lerp(minBeepInterval, 3f, closestDistance / dangerDistance);
            dangerIndicatorMaterial.SetFloat("_Intensity", Mathf.Lerp(2, 1, closestDistance / dangerDistance));

            if (Time.time - lastBeepTime >= beepInterval)
            {
                SoundManager.Instance.PlaySoundClip(dangerBip, closestEntity.transform, SoundManager.SoundType.FX, SoundManager.SoundFXType.FX);
                lastBeepTime = Time.time;
            }
        }
    }
}