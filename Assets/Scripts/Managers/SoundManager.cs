using UnityEngine;
using System.Collections.Generic;
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private AudioSource soundFXPrefab;
    [SerializeField] private AudioSource ambientPrefab;

    private Dictionary<AudioSource, Transform> currentlyPlayingSound = new Dictionary<AudioSource, Transform>();

    public enum SoundFXType {
        FX,
        AMBIENT
    }

    public enum SoundType {
        AMBIENT,
        FX,
        LOUD_FX,
        MUSIC,
        FOOTSTEPS,
        MONSTER,
        WHISPERS,
        BREATHING,
        DOOR_CREAK
    }

    public static class SoundVolumeMapper {
        private static readonly Dictionary<SoundType, float> soundVolumeMap = new Dictionary<SoundType, float> {
            { SoundType.AMBIENT, 0.01f },
            { SoundType.FX, 0.01f },
            { SoundType.LOUD_FX, 0.08f },
            { SoundType.MUSIC, 0.05f },
            { SoundType.FOOTSTEPS, 0.003f },
            { SoundType.MONSTER, 0.06f },
            { SoundType.WHISPERS, 0.015f },
            { SoundType.BREATHING, 0.005f },
            { SoundType.DOOR_CREAK, 0.025f }
        };

        public static float GetVolume(SoundType soundType) {
            return soundVolumeMap[soundType];
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public AudioSource PlaySoundClip(AudioClip audioClip, Transform transform, SoundType volume, SoundFXType soundFXType, bool looped = false, float? distanceMax = null, Transform followTarget = null, float? additionalAttenuation = null) {
        distanceMax ??= PlayerMovement.Instance.hearingRadius;
        float distance = CalcUtils.DistanceToTarget(transform.position, PlayerMovement.Instance.transform.position);
        if(distance > distanceMax * 1.3f)
            return null;
            
        float volumeValue = SoundVolumeMapper.GetVolume(volume);
        // Apply distance-based attenuation between 0.67x and 1x of max distance
        if (distance > distanceMax.Value) {
            float attenuation = 1f - ((distance - distanceMax.Value) / (distanceMax.Value * 0.5f));
            volumeValue *= attenuation;
        }
        if(additionalAttenuation != null) {
            volumeValue *= additionalAttenuation.Value;
        }

        return looped ? PlayLoopedSound(audioClip, transform, volumeValue, soundFXType) : PlaySound(audioClip, transform, volumeValue, soundFXType, followTarget);
    }


    public void PlayRandomSoundClip(AudioClip[] audioClips, Transform transform, SoundType volume, SoundFXType soundFXType, float? distanceMax = null, Transform followTarget = null, float? additionalAttenuation = null) {
        distanceMax ??= PlayerMovement.Instance.hearingRadius;
        float distance = CalcUtils.DistanceToTarget(transform.position, PlayerMovement.Instance.transform.position);
        if(distance > distanceMax * 1.5f)
            return;
            
        float volumeValue = SoundVolumeMapper.GetVolume(volume);
        // Apply distance-based attenuation between 0.67x and 1x of max distance
        if (distance > distanceMax.Value) {
            float attenuation = 1f - ((distance - distanceMax.Value) / (distanceMax.Value * 0.5f));
            volumeValue *= attenuation;
        }
        if(additionalAttenuation != null) {
            volumeValue *= additionalAttenuation.Value;
        }
        AudioClip audioClip = audioClips[Random.Range(0, audioClips.Length)];
        PlaySound(audioClip, transform, volumeValue, soundFXType, followTarget);
    }

    public AudioSource PlayLoopedSound(AudioClip audioClip, Transform transform, float volume, SoundFXType soundFXType) {
        AudioSource prefab = soundFXType == SoundFXType.FX ? soundFXPrefab : ambientPrefab;
        AudioSource audioSource = Instantiate(prefab, transform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.Play();
        return audioSource;
    }


    private AudioSource PlaySound(AudioClip audioClip, Transform transform, float volume, SoundFXType soundFXType, Transform followTarget = null) {
        AudioSource prefab = soundFXType == SoundFXType.FX ? soundFXPrefab : ambientPrefab;
        AudioSource audioSource = Instantiate(prefab, transform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        if(followTarget != null) {
            currentlyPlayingSound.Add(audioSource, followTarget);
        }
        Destroy(audioSource.gameObject, audioClip.length);
        return audioSource;
    }

    private void Update() {
        List<AudioSource> keysToRemove = new List<AudioSource>();

        foreach (var audioSource in currentlyPlayingSound) {
            if (audioSource.Key == null) {
                keysToRemove.Add(audioSource.Key);
                continue;
            }
            audioSource.Key.transform.position = audioSource.Value.position;
        }

        foreach (var key in keysToRemove) {
            currentlyPlayingSound.Remove(key);
        }
    }
}
