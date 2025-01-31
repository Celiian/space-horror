using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using static SoundManager;

public class PlayerMovement : Entity
{
    public static PlayerMovement Instance { get; private set; }

    [FoldoutGroup("Movement Settings"), SerializeField] private Rigidbody2D rb;
    [FoldoutGroup("Movement Settings"), SerializeField] private float speed;
    [FoldoutGroup("Movement Settings"), SerializeField] public float sprintMultiplier = 1.5f;
    [FoldoutGroup("Movement Settings"), SerializeField] public float slowMultiplier = 0.7f;
    [FoldoutGroup("Audio Settings"), SerializeField] private AudioClip[] stepSounds;
    [FoldoutGroup("Animation Settings"), SerializeField] private PlayerAnimator playerAnimator;
    [FoldoutGroup("Hearing Settings"), SerializeField] public float hearingRadius = 15;


    [ReadOnly]
    public bool canMove = true;
    [ReadOnly]
    public bool isCrouching = false;
    [ReadOnly]
    public bool isMoving = false;
    [ReadOnly]
    public float currentSpeedMultiplier = 1;
    [ReadOnly]
    public Vector2 movementDirection;
    
    private float minMovementThreshold = 0.1f;
    private float previousSpeedMultiplier = 1;


    private void Awake() {
        Instance = this;
    }

    public void OnRun(InputAction.CallbackContext context) {
        if(context.performed) {
            previousSpeedMultiplier = 1;
            currentSpeedMultiplier = sprintMultiplier;
        } else {
            previousSpeedMultiplier = sprintMultiplier;
            currentSpeedMultiplier = 1;
        }
    }

    public void OnSneak(InputAction.CallbackContext context) {
        if(context.performed) {
            previousSpeedMultiplier = 1;
            currentSpeedMultiplier = slowMultiplier;
        } else {
            previousSpeedMultiplier = slowMultiplier;
            currentSpeedMultiplier = 1;
        }
    }

    public void OnMove(InputAction.CallbackContext context) {
        movementDirection = context.ReadValue<Vector2>();
    }

    public override void Update() {
        HandleAnimation();
        if(isPaused || !canMove) {
            isMoving = false;
            rb.velocity = Vector2.zero;
            return;
        };

        HandleMovement();
    }

    private void HandleMovement() {
        Vector2 movement = movementDirection.normalized * speed * currentSpeedMultiplier;
        rb.velocity = movement;
        movementDirection = movement;
        isMoving = movement.magnitude > minMovementThreshold;
    }

    private void HandleAnimation() {
        if (isMoving) {
            if (playerAnimator.ShouldAnimateAgain() || playerAnimator.currentAnimation != playerAnimator.GetAnimationName("Walk", "WalkUp", "WalkDown") || previousSpeedMultiplier != currentSpeedMultiplier) {
                previousSpeedMultiplier = currentSpeedMultiplier;
                playerAnimator.PlayAnimation(playerAnimator.GetAnimationName("Walk", "WalkUp", "WalkDown"), currentSpeedMultiplier);
            }
        } else {
            if (playerAnimator.ShouldAnimateAgain() || playerAnimator.currentAnimation != playerAnimator.GetAnimationName("Idle", "IdleUp", "IdleDown")) {
                playerAnimator.PlayAnimation(playerAnimator.GetAnimationName("Idle", "IdleUp", "IdleDown"));
            }
        }
    }

    public void HandleFootstepSounds() {
        if(isMoving) {
            SoundPropagationManager.Instance.PropagateSound(transform.position, SoundOrigin.PLAYER, 0.8f * currentSpeedMultiplier);
            SoundManager.Instance.PlayRandomSoundClip(stepSounds, transform, SoundType.FOOTSTEPS, SoundFXType.FX, followTarget: transform, additionalAttenuation: currentSpeedMultiplier);
        }
    }

}