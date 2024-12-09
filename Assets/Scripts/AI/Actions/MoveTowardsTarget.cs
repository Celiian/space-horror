using UnityEngine;
using BehaviorTree;
using static SoundManager;

public class MoveTowardsTarget : Node
{
    private Transform transform;
    private Rigidbody2D rb;
    private AudioClip[] stepSounds;
    private string currentTargetPositionKey;
    private float stepTimer = 0;
    private float distanceThreshold = 0.1f;
    private float minMovementThreshold = 0.1f;
    private float speedMultiplier = 1;
    private EnemyAnimator enemyAnimator;
    public MoveTowardsTarget(Transform transform, Rigidbody2D rb, AudioClip[] stepSounds, string currentTargetPositionKey, float distanceThreshold, EnemyAnimator enemyAnimator, float speedMultiplier = 1)
    {
        this.transform = transform;
        this.rb = rb;
        this.stepSounds = stepSounds;
        this.currentTargetPositionKey = currentTargetPositionKey;
        this.distanceThreshold = distanceThreshold;
        this.speedMultiplier = speedMultiplier;
        this.enemyAnimator = enemyAnimator;
    }

    public override NodeState Evaluate()
    {
        object targetData = GetData(currentTargetPositionKey);
        if (targetData == null)
        {
            return NodeState.FAILURE;
        }

        Vector3 targetPosition = (Vector3)targetData;
        return MoveTo(targetPosition);
    }

    private NodeState MoveTo(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        float speed = (float)GetData("speed") * speedMultiplier;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        SetTopParentData("movementDirection", direction);
        
        if (distanceToTarget > distanceThreshold)
        {
            if(enemyAnimator.ShouldAnimateAgain())
                enemyAnimator.PlayAnimation(enemyAnimator.GetAnimationName("Walk", "WalkUp", "WalkDown"));
            MoveCharacter(direction, speed);
            HandleFootstepSounds();
            return NodeState.SUCCESS;
        }
        else
        {
            if(enemyAnimator.ShouldAnimateAgain())
                enemyAnimator.PlayAnimation(enemyAnimator.GetAnimationName("Idle", "IdleUp", "IdleDown"));
            StopCharacter();
            return NodeState.FAILURE;
        }
    }

    private void MoveCharacter(Vector3 direction, float speed)
    {
        rb.velocity = direction * speed;
        // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void StopCharacter()
    {
        rb.velocity = Vector2.zero;
    }

    private void HandleFootstepSounds()
    {
        if (rb.velocity.magnitude > minMovementThreshold)
        {
            stepTimer += Time.deltaTime * speedMultiplier;
            
            if (stepTimer >= SoundPropagationManager.Instance.stepInterval)
            {
                // Propagate sound at current position
                SoundPropagationManager.Instance.PropagateSound(transform.position, SoundOrigin.ZOMBIE, 0.8f);
                SoundManager.Instance.PlayRandomSoundClip(stepSounds, transform, SoundType.FOOTSTEPS, SoundFXType.FX);

                stepTimer = 0; // Reset timer
            }
        } else {
            // Reset timer when not moving
            stepTimer = 0;
        }
    }
}
