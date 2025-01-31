using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using static SoundManager;

public class PlayerMovement : Entity
{
    public static PlayerMovement Instance { get; private set; }

    [FoldoutGroup("Movement Settings"), SerializeField] private Rigidbody2D rb;
    [FoldoutGroup("Movement Settings"), SerializeField] private float speed;
    [FoldoutGroup("Movement Settings"), SerializeField] public float sprintCooldown = 5f;
    [FoldoutGroup("Movement Settings"), SerializeField] public float sprintMultiplier = 3f;
    [FoldoutGroup("Movement Settings"), SerializeField] public float slowMultiplier = 0.7f;
    [FoldoutGroup("Audio Settings"), SerializeField] private AudioClip[] stepSounds;
    [FoldoutGroup("Animation Settings"), SerializeField] private PlayerAnimator playerAnimator;
    [FoldoutGroup("Hearing Settings"), SerializeField] public float hearingRadius = 15;

    [FoldoutGroup("Sprint Sounds"), SerializeField] public AudioClip sprintStartSound;
    [FoldoutGroup("Sprint Sounds"), SerializeField] public AudioClip sprintResetLoopSound;
    [FoldoutGroup("Sprint Sounds"), SerializeField] public AudioClip sprintResetEndSound;


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
    private float sprintTimer = 0;
    private bool isSprinting = false;
    private bool canSprint = true;
    private float sprintDuration = 1f;

    private float baseSpeedMultiplier = 1;

    private void Awake() {
        Instance = this;
        sprintDuration = sprintStartSound.length;
    }

    public void OnRun(InputAction.CallbackContext context) {
        if(context.performed && canSprint) {
            canSprint = false;
            previousSpeedMultiplier = baseSpeedMultiplier;
            currentSpeedMultiplier = sprintMultiplier;
            isSprinting = true;
            sprintTimer = sprintDuration;
            StartCoroutine(SprintCooldown());
        }
    }

    public void OnSneak(InputAction.CallbackContext context) {
        if(isSprinting) return;
        if(context.performed) {
            previousSpeedMultiplier = baseSpeedMultiplier;
            currentSpeedMultiplier = slowMultiplier;
        } else {
            previousSpeedMultiplier = slowMultiplier;
            currentSpeedMultiplier = baseSpeedMultiplier;
        }
    }

    public void OnMove(InputAction.CallbackContext context) {
        movementDirection = context.ReadValue<Vector2>();
    }

    public override void Update() {
        if(isSprinting) {
            sprintTimer -= Time.deltaTime;
            if(sprintTimer <= 0) {
                isSprinting = false;
                previousSpeedMultiplier = baseSpeedMultiplier;
                currentSpeedMultiplier = baseSpeedMultiplier;
            }
        }

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

    private IEnumerator SprintCooldown() {
        baseSpeedMultiplier = 0.7f;
        SoundManager.Instance.PlaySoundClip(sprintStartSound, transform, SoundType.FX, SoundFXType.FX, followTarget: transform);

        yield return new WaitForSeconds(sprintStartSound.length);
        currentSpeedMultiplier = baseSpeedMultiplier;



        AudioSource audioSource = SoundManager.Instance.PlaySoundClip(sprintResetLoopSound, transform, SoundType.FX, SoundFXType.FX, looped: true, followTarget: transform);

        yield return new WaitForSeconds(sprintCooldown);

        baseSpeedMultiplier = 1;
        currentSpeedMultiplier = currentSpeedMultiplier == 0.7f ? 1 : currentSpeedMultiplier;
        canSprint = true;
        audioSource.Stop();
        SoundManager.Instance.PlaySoundClip(sprintResetEndSound, transform, SoundType.FX, SoundFXType.FX, followTarget: transform);
    }

}