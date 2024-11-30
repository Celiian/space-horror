using System.Collections;
using Shapes;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Manages the sound volume depending on the distance to the player.
/// </summary>
public class ManageSoundVolume : MonoBehaviour
{

    private float propagationDistance = 20f;
    private LayerMask? occlusionLayers;
    private Rigidbody2D rb = null;
    private float maxSpeed = 0f;
    private float maxVolume = 0.5f;
    private AudioSource audioSource;
    private AudioLowPassFilter lowPassFilter;
    private  Player player;
    private bool isPlayer = false;

    public void Initialize(float propagationDistance, LayerMask? occlusionLayers, float maxSpeed, float maxVolume, Rigidbody2D rb = null, bool isPlayer = false)
    {
        player = GameManager.Instance.player;
        audioSource = GetComponent<AudioSource>();
        lowPassFilter = GetComponent<AudioLowPassFilter>();
        this.propagationDistance = propagationDistance;
        this.occlusionLayers = occlusionLayers;
        this.maxVolume = maxVolume;
        this.maxSpeed = maxSpeed;
        this.isPlayer = isPlayer;
        this.rb = rb;
    }

    void Update()
    {
        float distance = CalcUtils.DistanceToTarget(player.transform, transform);
        if(distance > propagationDistance && !isPlayer) {
            audioSource.volume = 0f;
            return;
        }
        int wallCount = 0;

        Debug.DrawLine(transform.position, player.transform.position, Color.red);
        if(!isPlayer){
            Vector2 directionToTarget = (player.transform.position - transform.position).normalized;
            RaycastHit2D[] hits;
            if(occlusionLayers != null){
                hits = Physics2D.RaycastAll(transform.position, directionToTarget, distance, occlusionLayers.Value);
            } else {
                hits = Physics2D.RaycastAll(transform.position, directionToTarget, distance);
            }
            wallCount = hits.Length;
        }

        CalculateSoundVolume.SoundParameters parameters = new CalculateSoundVolume.SoundParameters(propagationDistance, maxSpeed);
        (float volume, float cutoffFrequency, float spatialBlend) = CalculateSoundVolume.CalculateSoundProperties(distance, wallCount, isPlayer, parameters, maxVolume, rb);


        audioSource.volume = volume;
        audioSource.spatialBlend = spatialBlend;
        lowPassFilter.cutoffFrequency = cutoffFrequency;
    }
}
