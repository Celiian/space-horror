using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using static SoundManager;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    [SerializeField] private float sprintMultiplier = 1.75f;
    [SerializeField] private AudioClip[] stepSounds;
    [SerializeField] private PlayerAnimator playerAnimator;
    
    public Vector2 movementDirection;
    public bool isMoving = false;

    public float hearingRadius = 15;
    private float stepTimer = 0;
    private float minMovementThreshold = 0.1f;

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        HandleMovement();
        HandleAnimation();
        HandleFootstepSounds();
    }

    private Vector2 GetInputMovement() {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (inputVector.magnitude > 1f) {
            inputVector.Normalize();
        }
        
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * sprintMultiplier : speed;
        return inputVector * currentSpeed;
    }

    private void HandleMovement() {
        Vector2 movement = GetInputMovement();
        rb.velocity = movement;
        movementDirection = movement;
        isMoving = movement.magnitude > minMovementThreshold;
    }

    private void HandleAnimation() {
        if (isMoving) {
            if (playerAnimator.ShouldAnimateAgain() || playerAnimator.currentAnimation != playerAnimator.GetAnimationName("Walk", "WalkUp", "WalkDown")) {
                playerAnimator.PlayAnimation(playerAnimator.GetAnimationName("Walk", "WalkUp", "WalkDown"), Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);
            }
        } else {
            if (playerAnimator.ShouldAnimateAgain() || playerAnimator.currentAnimation != playerAnimator.GetAnimationName("Idle", "IdleUp", "IdleDown")) {
                playerAnimator.PlayAnimation(playerAnimator.GetAnimationName("Idle", "IdleUp", "IdleDown"));
            }
        }
    }

    private void HandleFootstepSounds() {
        if (isMoving) {
            stepTimer += Time.deltaTime;
            
            float currentStepInterval = Input.GetKey(KeyCode.LeftShift) 
                ? SoundPropagationManager.Instance.stepInterval / sprintMultiplier 
                : SoundPropagationManager.Instance.stepInterval;

            if (stepTimer >= currentStepInterval) {
                SoundPropagationManager.Instance.PropagateSound(transform.position, SoundOrigin.PLAYER, 0.8f);
                SoundManager.Instance.PlayRandomSoundClip(stepSounds, transform, SoundType.FOOTSTEPS, SoundFXType.FX);
                stepTimer = 0;
            }
        } else {
            stepTimer = 0;
        }
    }

    public int GetDirectionInt() {
        if (rb.velocity.y > 0) return 0;
        if (rb.velocity.y < 0) return 1;
        if (rb.velocity.x < 0) return 2;
        if (rb.velocity.x > 0) return 3;
        return 0;
    }
}