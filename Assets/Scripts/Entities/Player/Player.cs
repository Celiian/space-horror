
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public PlayerAnimator playerAnimator;

    public bool isDead = false;

    private void Awake() {
        Instance = this;
    }

    [SerializeField] private AudioClip deathSound;

    public void TakeDamage() {
        if(isDead) return;
        isDead = true;
        SoundManager.Instance.PlaySoundClip(deathSound, transform, SoundManager.SoundType.LOUD_FX, SoundManager.SoundFXType.FX);
        PlayerMovement.Instance.canMove = false;
    }
}   