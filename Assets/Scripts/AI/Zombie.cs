using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using static EnemyAnimator;
using static SoundManager;

public class Zombie : MonoBehaviour
{
    #region References
    private Rigidbody2D rb;
    private EnemyAnimator enemyAnimator;
    #endregion

    #region Parameters
    [FoldoutGroup("Sounds"), SerializeField]
    private AudioClip[] stepSounds;


    [FoldoutGroup("Stats"), SerializeField]
    private LayerMask visionLayerMask;

    [FoldoutGroup("Stats"), SerializeField]
    public float detectionRadius;
    
    [FoldoutGroup("Stats"), SerializeField]
    public float visionAngle;

    [FoldoutGroup("Stats"), SerializeField]
    public float moveSpeed;

    #endregion

    #region Variables
    private bool isFacingRight;
    private List<TileNode> currentPath;
    private Vector3 currentTarget;
    private float timeSinceLastUpdate;
    private float stepTimer;
    private float speedMultiplier = 1f;
    private bool isPatrolling = false;
    public Vector3 movementDirection;
    public bool debug = false;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<EnemyAnimator>();
    }

    private void Start()
    {
        isFacingRight = true;
    }

    private void Update()
    {
        if (isPlayerInRadius() && isPlayerInView() && isPlayerVisible())
        {
            if (currentPath == null || currentPath.Count == 0 || isPatrolling) 
            {
                initPathToPlayer();
            }

            speedMultiplier = 1.5f;
            isPatrolling = false;
            timeSinceLastUpdate += Time.deltaTime;


            if(timeSinceLastUpdate > 0.2f)
            {
                updatePathToPlayer();
                timeSinceLastUpdate = 0;
            }

            if (currentPath != null && currentPath.Count > 0)
            {
                if (isCloseToTarget(currentPath.First().GetPosition()))
                {
                    currentPath.RemoveAt(0);
                    if (currentPath.Count == 0)
                    {
                        updatePathToPlayer();
                    }
                }

                currentTarget = currentPath.First().GetPosition();
            }

            Vector2 directionToPlayer = (PlayerMovement.Instance.transform.position - transform.position).normalized;
            movementDirection = directionToPlayer;
            float distanceToPlayer = CalcUtils.DistanceToTarget(transform.position, PlayerMovement.Instance.transform.position);
            if(distanceToPlayer < 0.1f) {
                Player.Instance.TakeDamage();
            }
        }
        else
        {

            if (currentPath != null && currentPath.Count > 0)
            {
                Vector2 directionToTarget = (currentPath.First().GetPosition() - transform.position).normalized;
                movementDirection = directionToTarget;
                currentTarget = currentPath.First().GetPosition();

                if (isCloseToTarget(currentPath.First().GetPosition()))
                {
                    currentPath.RemoveAt(0);
                }

            }
            else {
                updatePatrolPath();
                speedMultiplier = 1f;
                isPatrolling = true;
            }
        }

        HandleAnimations();
    }

    private void FixedUpdate()
    {
        MoveTowardsTarget(currentTarget);
    }

    #region Actions
    private void MoveTowardsTarget(Vector3 target){
        Vector2 direction = (target - transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        HandleFootstepSounds();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void initPathToPlayer(){
        TileNode startNode = PathFinding.Instance.FindNodeCloseToPosition(transform.position);
        TileNode endNode = PathFinding.Instance.FindNodeCloseToPosition(PlayerMovement.Instance.transform.position);
        currentPath = PathFinding.Instance.FindPath(startNode, endNode);
    }

    private void updatePathToPlayer()
    {
        Vector3 startingPos = currentPath.Count == 0 ? transform.position : currentPath.First().GetPosition();

        TileNode startNode = PathFinding.Instance.FindNodeCloseToPosition(startingPos);
        TileNode endNode = PathFinding.Instance.FindNodeCloseToPosition(PlayerMovement.Instance.transform.position);
        currentPath = PathFinding.Instance.FindPath(startNode, endNode);
    }

    private void updatePatrolPath(){
        TileNode startNode = PathFinding.Instance.FindNodeCloseToPosition(transform.position);
        currentPath = PathFinding.Instance.GetRandomPath(startNode);
    }

    #endregion

    #region Conditions
    private bool isCloseToTarget(Vector3 target){
        return CalcUtils.DistanceToTarget(transform.position, target) < 0.1f;
    }

    private bool isPlayerInRadius(){
        if(debug) {
            Debug.DrawLine(transform.position, PlayerMovement.Instance.transform.position, Color.red);
            
            // Draw a circle representing the detection radius
            int segments = 36; // Number of segments to approximate the circle
            float angleStep = 360f / segments;
            for (int i = 0; i < segments; i++)
            {
                float angle1 = Mathf.Deg2Rad * angleStep * i;
                float angle2 = Mathf.Deg2Rad * angleStep * (i + 1);
                Vector3 point1 = transform.position + new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1)) * detectionRadius;
            }
        }
        return CalcUtils.DistanceToTarget(transform.position, PlayerMovement.Instance.transform.position) < detectionRadius;
    }

    private bool isPlayerInView()
    {
        var playerPosition = PlayerMovement.Instance.transform.position;
        var directionToPlayer = (playerPosition - transform.position).normalized;
        float angle = 0f;

        Vector2 facingDirection;

        switch (enemyAnimator.currentDirection)
        {
            case Direction.Up:
                angle = Vector2.Angle(Vector2.up, directionToPlayer);
                facingDirection = Vector2.up;
                break;
            case Direction.Down:
                angle = Vector2.Angle(Vector2.down, directionToPlayer);
                facingDirection = Vector2.down;
                break;
            case Direction.Left:
                angle = Vector2.Angle(Vector2.left, directionToPlayer);
                facingDirection = Vector2.left;
                break;
            case Direction.Right:
                angle = Vector2.Angle(Vector2.right, directionToPlayer);
                facingDirection = Vector2.right;
                break;
            default:
                facingDirection = Vector2.right;
                break;
        }

        if (debug)
        {
            // Calculate the left and right boundaries of the vision cone
            Vector3 leftBoundary = Quaternion.Euler(0, 0, visionAngle / 2) * facingDirection;
            Vector3 rightBoundary = Quaternion.Euler(0, 0, -visionAngle / 2) * facingDirection;

            // Draw the vision cone
            Debug.DrawLine(transform.position, transform.position + leftBoundary * detectionRadius, Color.yellow);
            Debug.DrawLine(transform.position, transform.position + rightBoundary * detectionRadius, Color.yellow);
        }
        bool playerInView = angle <= visionAngle / 2;
        return playerInView;
    }

    private bool isPlayerVisible(){
        RaycastHit2D hit = Physics2D.Raycast(transform.position, PlayerMovement.Instance.transform.position - transform.position, detectionRadius, visionLayerMask);        
        if(debug) 
            Debug.DrawLine(transform.position, PlayerMovement.Instance.transform.position, Color.red);

        if(hit.collider != null) {
            if(hit.collider.CompareTag("Player")) {
                return true;
            }
            else if (hit.collider.CompareTag("Wall"))
            {
                return false;
            }
        }
        
        return true;
    }
    #endregion


    #region Handlers
    private void HandleAnimations(){
        if (rb.velocity.magnitude > 0.1f)
            if(enemyAnimator.ShouldAnimateAgain())
                enemyAnimator.PlayAnimation(enemyAnimator.GetAnimationName("Walk", "WalkUp", "WalkDown"), speedMultiplier);
        else
            if(enemyAnimator.ShouldAnimateAgain())
                enemyAnimator.PlayAnimation(enemyAnimator.GetAnimationName("Idle", "IdleUp", "IdleDown"));
    }

    private void HandleFootstepSounds()
    {
        if (rb.velocity.magnitude > 0.1f)
        {
            stepTimer += Time.deltaTime * speedMultiplier;
            
            if (stepTimer >= SoundPropagationManager.Instance.stepInterval)
            {
                // Propagate sound at current position
                SoundPropagationManager.Instance.PropagateSound(transform.position, SoundOrigin.ZOMBIE, 0.8f * speedMultiplier);
                SoundManager.Instance.PlayRandomSoundClip(stepSounds, transform, SoundType.FOOTSTEPS, SoundFXType.FX);

                stepTimer = 0; // Reset timer
            }
        } else {
            // Reset timer when not moving
            stepTimer = 0;
        }
    }
    #endregion

}