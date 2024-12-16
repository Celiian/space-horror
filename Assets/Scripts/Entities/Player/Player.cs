
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public PlayerAnimator playerAnimator;

    private void Awake() {
        Instance = this;
    }

    [SerializeField] private AudioClip deathSound;

    public void TakeDamage() {
        SoundManager.Instance.PlaySoundClip(deathSound, transform, SoundManager.SoundType.LOUD_FX, SoundManager.SoundFXType.FX);
        PlayerMovement.Instance.canMove = false;
    }
}   