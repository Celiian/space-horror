using UnityEngine;
using static SoundManager;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    [SerializeField] private float sprintMultiplier = 1.75f;
    [SerializeField] private AudioClip[] stepSounds;

    public float hearingRadius = 15;
    private float stepTimer = 0;
    private float minMovementThreshold = 0.1f; // Minimum movement speed to trigger steps

    private void Awake() {
        Instance = this;
    }

    private Vector2 GetInputMovement() {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (inputVector.magnitude > 1f) {
            inputVector.Normalize();
        }
        
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * sprintMultiplier : speed;
        return inputVector * currentSpeed;
    }

    private void Update() {
        Vector2 movement = GetInputMovement();
        rb.velocity = movement;

        // Handle footstep sounds
        if (rb.velocity.magnitude > minMovementThreshold) {
            stepTimer += Time.deltaTime;
            
            float currentStepInterval = Input.GetKey(KeyCode.LeftShift) 
                ? SoundPropagationManager.Instance.stepInterval / sprintMultiplier 
                : SoundPropagationManager.Instance.stepInterval;

            if (stepTimer >= currentStepInterval) {
                // Propagate sound at player position
                SoundPropagationManager.Instance.PropagateSound(transform.position, SoundOrigin.PLAYER);
                SoundManager.Instance.PlayRandomSoundClip(stepSounds, transform, SoundType.FOOTSTEPS, SoundFXType.FX);
                stepTimer = 0; // Reset timer
            }
        } else {
            // Reset timer when not moving
            stepTimer = 0;
        }
    }
}
