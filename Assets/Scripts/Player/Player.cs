
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
    public int lives = 3;
    private Color originalColor;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void TakeDamage() {
        lives--;
        if(lives <= 0) {
            SoundManager.Instance.PlaySoundClip(deathSound, transform, SoundManager.SoundType.FX, SoundManager.SoundFXType.FX);
            playerAnimator.PlayAnimation("Die");
        }
        else {
            SoundManager.Instance.PlaySoundClip(hitSound, transform, SoundManager.SoundType.FX, SoundManager.SoundFXType.FX);
            spriteRenderer.color = Color.red;
            Invoke(nameof(ResetColor), 0.1f);
        }
    }

    private void ResetColor() {
        spriteRenderer.color = originalColor;
    }
}   