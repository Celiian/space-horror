using UnityEngine;
using System.Collections.Generic;
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private AudioSource soundFXPrefab;
    [SerializeField] private AudioSource ambientPrefab;

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
            { SoundType.AMBIENT, 0.1f },
            { SoundType.FX, 0.4f },
            { SoundType.LOUD_FX, 0.8f },
            { SoundType.MUSIC, 0.5f },
            { SoundType.FOOTSTEPS, 0.1f },
            { SoundType.MONSTER, 0.6f },
            { SoundType.WHISPERS, 0.15f },
            { SoundType.BREATHING, 0.2f },
            { SoundType.DOOR_CREAK, 0.25f }
        };

        public static float GetVolume(SoundType soundType) {
            return soundVolumeMap[soundType];
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void PlaySoundClip(AudioClip audioClip, Transform transform, SoundType volume, SoundFXType soundFXType, float? distanceMax = null) {
        distanceMax ??= PlayerMovement.Instance.hearingRadius;
        float distance = CalcUtils.DistanceToTarget(transform.position, PlayerMovement.Instance.transform.position);
        if(distance > distanceMax * 1.3f)
            return;
            
        float volumeValue = SoundVolumeMapper.GetVolume(volume);
        // Apply distance-based attenuation between 0.67x and 1x of max distance
        if (distance > distanceMax.Value) {
            float attenuation = 1f - ((distance - distanceMax.Value) / (distanceMax.Value * 0.5f));
            volumeValue *= attenuation;
        }
        PlaySound(audioClip, transform, volumeValue, soundFXType);
    }

    public void PlayRandomSoundClip(AudioClip[] audioClips, Transform transform, SoundType volume, SoundFXType soundFXType, float? distanceMax = null) {
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
        AudioClip audioClip = audioClips[Random.Range(0, audioClips.Length)];
        PlaySound(audioClip, transform, volumeValue, soundFXType);
    }

    private void PlaySound(AudioClip audioClip, Transform transform, float volume, SoundFXType soundFXType) {
        AudioSource prefab = soundFXType == SoundFXType.FX ? soundFXPrefab : ambientPrefab;
        AudioSource audioSource = Instantiate(prefab, transform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audioSource.gameObject, audioClip.length);
    }
}
