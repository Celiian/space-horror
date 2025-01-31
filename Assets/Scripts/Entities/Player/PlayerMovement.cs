using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.InputSystem;
using static SoundManager;

public class PlayerMovement : Entity
{
    public static PlayerMovement Instance { get; private set; }

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float slowMultiplier = 0.7f;
    [SerializeField] private AudioClip[] stepSounds;
    [SerializeField] private PlayerAnimator playerAnimator;

    public bool canMove = true;
    public bool isCrouching = false;
    
    public Vector2 movementDirection;
    public bool isMoving = false;
    public float hearingRadius = 15;
    private float minMovementThreshold = 0.1f;
    public float currentSpeedMultiplier = 1;
    private void Awake() {
        Instance = this;
    }

    public void OnRun(InputAction.CallbackContext context) {
        if(context.performed) {
            currentSpeedMultiplier = sprintMultiplier;
        } else {
            currentSpeedMultiplier = 1;
        }
    }

    public void OnSneak(InputAction.CallbackContext context) {
        if(context.performed) {
            currentSpeedMultiplier = slowMultiplier;
        } else {
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
            if (playerAnimator.ShouldAnimateAgain() || playerAnimator.currentAnimation != playerAnimator.GetAnimationName("Walk", "WalkUp", "WalkDown")) {
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