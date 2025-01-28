using UnityEngine;
using static SoundManager;

public class PlayerAliveSounds : MonoBehaviour
{
    [SerializeField] private AudioClip normalHeartbeatSound;
    [SerializeField] private AudioClip sprintHeartbeatSound;
    [SerializeField] private float HeartbeatSoundDelay = 1f;


    [SerializeField] private AudioClip normalBreathingSound;
    [SerializeField] private AudioClip sprintBreathingSound;
    [SerializeField] private float BreathingSoundDelay = 1f;

    private float heartbeatTimer = 0;
    private float breathingTimer = 0;

    void Update(){
        heartbeatTimer += Time.deltaTime;
        breathingTimer += Time.deltaTime;

        if(heartbeatTimer >= HeartbeatSoundDelay) {
            PlayHeartbeatSound();
            heartbeatTimer = 0;
        }

        // if(breathingTimer >= BreathingSoundDelay) {
        //     PlayBreathingSound();
        //     breathingTimer = 0;
        // }
    }

    public void PlayHeartbeatSound() {
        if (PlayerMovement.Instance.currentSpeedMultiplier == 1) {
            SoundManager.Instance.PlaySoundClip(normalHeartbeatSound, transform, SoundType.BREATHING, SoundFXType.FX, followTarget: transform, additionalAttenuation: 1);
        } else {
            SoundManager.Instance.PlaySoundClip(sprintHeartbeatSound, transform, SoundType.BREATHING, SoundFXType.FX, followTarget: transform, additionalAttenuation: 1);
        }
    }

    public void PlayBreathingSound() {
        if (PlayerMovement.Instance.currentSpeedMultiplier == 1) {
            SoundManager.Instance.PlaySoundClip(normalBreathingSound, transform, SoundType.BREATHING, SoundFXType.FX, followTarget: transform, additionalAttenuation: 1);
        } else {
            SoundManager.Instance.PlaySoundClip(sprintBreathingSound, transform, SoundType.BREATHING, SoundFXType.FX, followTarget: transform, additionalAttenuation: 1);
        }
    }
}
