using System.Collections;
using UnityEngine;
using Shapes;
using Sirenix.OdinInspector;
using static SoundManager;

/// <summary>
/// Manages both light and sound properties based on distance to the player and occlusions.
/// </summary>
public class ManageLightAndSound : MonoBehaviour
{
    [System.Serializable]
    public class ManageLightAndSoundSettings
    {
        [TabGroup("General Settings")]
        public float propagationDistance = 10f;
        [TabGroup("General Settings")]
        public Rigidbody2D rb;
        [TabGroup("General Settings")]
        public bool isPlayer = false;


        [TabGroup("Light Settings")]
        public GameObject lightParticleSystem;
        [TabGroup("Light Settings")]
        public Color color = Color.white;
        [TabGroup("Light Settings")]
        public float particleCount = 60;
        [TabGroup("Light Settings")]
        public float particleLifeTime = 2f;
        [TabGroup("Light Settings")]
        public float radius = 1f;
        [TabGroup("Light Settings")]
        public float timeBetweenPulses = 0.5f;

        [TabGroup("Sound Settings")]
        public LayerMask occlusionLayers;
        [TabGroup("Sound Settings")]
        public SoundManager.SoundType maxVolume;
        [TabGroup("Sound Settings")]
        public float maxSpeed = 0f;

    }

    // Serialized settings classes
    public ManageLightAndSoundSettings lightAndSoundSettings;

    // Internal components
    private AudioSource audioSource;
    private AudioLowPassFilter lowPassFilter;
    private Player player;

    public void Initialize(ManageLightAndSoundSettings lightAndSoundSettings)
    {
        player = GameManager.Instance.player;
        audioSource = GetComponent<AudioSource>();
        lowPassFilter = GetComponent<AudioLowPassFilter>();

        // Assign settings
        this.lightAndSoundSettings = lightAndSoundSettings;

        // Start coroutine for light particle effects
        StartCoroutine(LightCoroutine());
    }

    private IEnumerator LightCoroutine()
    {
        while (true)
        {
            float distance = CalcUtils.DistanceToTarget(player.transform, transform);
            if (distance < lightAndSoundSettings.propagationDistance)
            {
                ParticleSystem particle = lightAndSoundSettings.lightParticleSystem.GetComponent<ParticleSystem>();
                var shape = particle.shape;
                var main = particle.main;
                main.startLifetime = lightAndSoundSettings.particleLifeTime;
                shape.radius = lightAndSoundSettings.radius;
                particle.emission.SetBursts(new ParticleSystem.Burst[] {
                    new ParticleSystem.Burst(0f, lightAndSoundSettings.particleCount)
                });
                AttachGameObjectsToParticles lightParticle = Instantiate(lightAndSoundSettings.lightParticleSystem, transform.position, Quaternion.identity).GetComponent<AttachGameObjectsToParticles>();
                lightParticle.color = lightAndSoundSettings.color;
            }
            yield return new WaitForSeconds(lightAndSoundSettings.timeBetweenPulses);
        }
    }

    private void Update()
    {
        float distance = CalcUtils.DistanceToTarget(player.transform, transform);
        
        if(lightAndSoundSettings.isPlayer) {
            distance = 0f;
        }

        // Manage sound volume based on distance and occlusions
        if (distance > lightAndSoundSettings.propagationDistance && !lightAndSoundSettings.isPlayer)
        {
            audioSource.volume = 0f;
            return;
        }

        int wallCount = 0;
        if (!lightAndSoundSettings.isPlayer)
        {
            Vector2 directionToTarget = (player.transform.position - transform.position).normalized;
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionToTarget, distance, lightAndSoundSettings.occlusionLayers);
            wallCount = hits.Length;
        }

        CalculateSoundVolume.SoundParameters parameters = new CalculateSoundVolume.SoundParameters(lightAndSoundSettings.propagationDistance, lightAndSoundSettings.maxSpeed);
        (float volume, float cutoffFrequency, float spatialBlend) = CalculateSoundVolume.CalculateSoundProperties(distance, wallCount, lightAndSoundSettings.isPlayer, parameters, SoundVolumeMapper.GetVolume(lightAndSoundSettings.maxVolume), lightAndSoundSettings.rb);
        audioSource.volume = lightAndSoundSettings.isPlayer ? volume * 0.6f : volume;
        // audioSource.spatialBlend = spatialBlend;
        lowPassFilter.cutoffFrequency = cutoffFrequency;
    }
}
