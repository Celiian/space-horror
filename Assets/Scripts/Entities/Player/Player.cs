
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public PlayerAnimator playerAnimator;

    private void Awake() {
        Instance = this;
    }

    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hitSound;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void TakeDamage() {
        SoundManager.Instance.PlaySoundClip(deathSound, transform, SoundManager.SoundType.FX, SoundManager.SoundFXType.FX);
        playerAnimator.PlayAnimation("Die");
        PlayerMovement.Instance.canMove = false;
    }

    private void ResetColor() {
        spriteRenderer.color = originalColor;
    }
}   