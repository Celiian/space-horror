using UnityEngine;
using static SoundManager;

public class ZombieSounds : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] discoverPlayerSound;

    private Zombie zombie;

    private void Start() {
        zombie = GetComponent<Zombie>();
        zombie.OnPlayerDiscovered += PlayDiscoverSound;
    }

    private void OnDestroy() {
        zombie.OnPlayerDiscovered -= PlayDiscoverSound;
    }

    private void PlayDiscoverSound() {
        SoundManager.Instance.PlayRandomSoundClip(discoverPlayerSound, transform, SoundType.FX, SoundFXType.FX, followTarget: transform);
    }
}