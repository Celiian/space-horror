using UnityEngine;
using static SoundManager;

public class PlayerAliveSounds : MonoBehaviour
{
    [SerializeField] private AudioClip normalHeartbeatSound;
    [SerializeField] private float HeartbeatBpm = 80f;

    PlayerDangerDetector playerDangerDetector;

    public int overrideHeartbeatBpm = 0;
    private int minHeartbeatBpm = 80;

    // [SerializeField] private AudioClip normalBreathingSound;
    // [SerializeField] private AudioClip sprintBreathingSound;
    // [SerializeField] private float BreathingSoundDelay = 1f;

    private void Start() {
        playerDangerDetector = FindObjectOfType<PlayerDangerDetector>();
    }

    private float heartbeatTimer = 0;
    // private float breathingTimer = 0;

    void Update(){
        heartbeatTimer += Time.deltaTime;
        // breathingTimer += Time.deltaTime;
        UpdateHeartbeatBpm();

        if(overrideHeartbeatBpm != 0) {
            HeartbeatBpm = overrideHeartbeatBpm;
        }

        if(heartbeatTimer >= 60 / HeartbeatBpm) {
            PlayHeartbeatSound();
            heartbeatTimer = 0;
        }

        // if(breathingTimer >= BreathingSoundDelay) {
        //     PlayBreathingSound();
        //     breathingTimer = 0;
        // }
    }

    private void UpdateHeartbeatBpm() {
        // If the player is not moving or is moving slowly, lower the heartbeat speed
        if(PlayerMovement.Instance.currentSpeedMultiplier < 1 || !PlayerMovement.Instance.isMoving) {
            minHeartbeatBpm = 60;
        }
        // If the player is running, increase the min heartbeat speed
        else if(PlayerMovement.Instance.currentSpeedMultiplier > 1) {
            minHeartbeatBpm = 100;
        }
        // If the player is moving at a normal speed, set the min heartbeat speed to 80
        else {
            minHeartbeatBpm = 80;
        }

        HeartbeatBpm = Mathf.Lerp(220, minHeartbeatBpm, playerDangerDetector.closestDistance / playerDangerDetector.dangerDistance);
    }

    public void PlayHeartbeatSound() {
        SoundManager.Instance.PlaySoundClip(normalHeartbeatSound, transform, SoundType.BREATHING, SoundFXType.FX, followTarget: transform, additionalAttenuation: 1);
        
    }

    // public void PlayBreathingSound() {
    //     if (PlayerMovement.Instance.currentSpeedMultiplier == 1) {
    //         SoundManager.Instance.PlaySoundClip(normalBreathingSound, transform, SoundType.BREATHING, SoundFXType.FX, followTarget: transform, additionalAttenuation: 1);
    //     } else {
    //         SoundManager.Instance.PlaySoundClip(sprintBreathingSound, transform, SoundType.BREATHING, SoundFXType.FX, followTarget: transform, additionalAttenuation: 1);
    //     }
    // }
}
