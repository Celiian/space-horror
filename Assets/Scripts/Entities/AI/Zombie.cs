using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using static EnemyAnimator;
using static SoundManager;
using System;
using DG.Tweening;

public class Zombie : Entity
{
    #region References
    private Rigidbody2D rb;
    private EnemyAnimator enemyAnimator;
    private Renderer zombieRenderer;
    #endregion

    #region Parameters
    [FoldoutGroup("Appearance"), SerializeField]
    private Material zombieMaterial;

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

    [FoldoutGroup("Stats"), SerializeField]
    public GameObject[] patrolPoints;


    #endregion

    #region Variables
    private bool isFacingRight;
    private List<TileNode> currentPath;
    private Vector3 currentTarget;
    private float timeSinceLastUpdate;
    private float stepTimer;
    private float speedMultiplier = 0.6f;
    private bool isPatrolling = false;
    public Vector3 movementDirection;
    public bool debug = false;
    public event Action OnPlayerDiscovered;
    private int currentPatrolIndex = 0;
    private int hearingRadius = 5;
    private float hearingTreeshold = 0.4f;
    private bool playerDiscovered = false;

    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<EnemyAnimator>();
        zombieRenderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        isFacingRight = true;
        if (zombieRenderer != null && zombieMaterial != null)
        {
            zombieRenderer.material = zombieMaterial;
        }
    }

    public override void Update()
    {
        if (isPaused) return;
        Tile tile = null;
        if(canHearPlayer()){
            tile = locatePlayerSound();
        }

        if ((isPlayerInRadius() && isPlayerInView() && isPlayerVisible()) || tile != null)
        {
            PlayDiscoverEffect();
            if (currentPath == null || currentPath.Count == 0 || isPatrolling) 
            {
                initPathToPlayer(tile);
                if(!playerDiscovered){
                    OnPlayerDiscovered?.Invoke();
                    playerDiscovered = true;
                }
            }

            speedMultiplier = 1.5f;
            isPatrolling = false;
            timeSinceLastUpdate += Time.deltaTime;


            if(timeSinceLastUpdate > 0.2f)
            {
                updatePathToPlayer(tile);
                timeSinceLastUpdate = 0;
            }

            if (currentPath != null && currentPath.Count > 0)
            {
                if (isCloseToTarget(currentPath.First().GetPosition()))
                {
                    currentPath.RemoveAt(0);
                    if (currentPath.Count == 0 || currentPath == null)
                    {
                        updatePathToPlayer(tile);
                    }
                }
                if(currentPath != null && currentPath.Count > 0){
                    currentTarget = currentPath.First().GetPosition();
                }
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
            StopDiscoverEffect();

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
            else if(patrolPoints.Length > 0){
                playerDiscovered = false;
                speedMultiplier = 0.6f;
                isPatrolling = true;
                updatePatrolPath();
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
        rb.velocity = direction * moveSpeed * speedMultiplier;

        HandleFootstepSounds();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void initPathToPlayer(Tile tile = null){
        Vector3 target = tile != null ? tile.getPosition() : PlayerMovement.Instance.transform.position;
        TileNode startNode = PathFinding.Instance.FindNodeCloseToPosition(transform.position);
        TileNode endNode = PathFinding.Instance.FindNodeCloseToPosition(target);
        currentPath = PathFinding.Instance.FindPath(startNode, endNode);
    }

    private void updatePathToPlayer(Tile tile = null)
    {
        Vector3 target = tile != null ? tile.getPosition() : PlayerMovement.Instance.transform.position;
        Vector3 startingPos = currentPath == null || currentPath.Count == 0 ? transform.position : currentPath.First().GetPosition();
        TileNode startNode = PathFinding.Instance.FindNodeCloseToPosition(startingPos);
        TileNode endNode = PathFinding.Instance.FindNodeCloseToPosition(target);
        currentPath = PathFinding.Instance.FindPath(startNode, endNode);
    }

    private void updatePatrolPath(Tile tile = null){
        Vector3 target = tile != null ? tile.getPosition() : patrolPoints[currentPatrolIndex].transform.position;
        currentPatrolIndex++;
        if (currentPatrolIndex >= patrolPoints.Length) {
            currentPatrolIndex = 0;
        }

        TileNode startNode = PathFinding.Instance.FindNodeCloseToPosition(transform.position);
        TileNode endNode = PathFinding.Instance.FindNodeCloseToPosition(target);
        currentPath = PathFinding.Instance.FindPath(startNode, endNode);
    }

    #endregion

    #region Conditions
    private bool canHearPlayer(){
        float currentSpeedMultiplier = PlayerMovement.Instance.currentSpeedMultiplier;
        return CalcUtils.DistanceToTarget(transform.position, PlayerMovement.Instance.transform.position) < (hearingRadius * currentSpeedMultiplier);
    }

    private Tile locatePlayerSound(){
        // Get the tiles around the zombie
        List<Tile> tiles = SoundPropagationManager.Instance.GetTilesInRadius(transform.position, hearingRadius);
        Tile highestSoundTile = null;
        foreach(Tile tile in tiles){
            float soundLevel = 0;
            foreach(var sound in tile.soundSources){
                if(sound.origin == SoundOrigin.PLAYER){
                    soundLevel += sound.soundLevel;
                }
            }
            if(soundLevel > hearingTreeshold){
                if(highestSoundTile == null || soundLevel > highestSoundTile.soundSources.Sum(sound => sound.origin == SoundOrigin.PLAYER ? sound.soundLevel : 0)){
                    highestSoundTile = tile;
                }
            }
        }
        return highestSoundTile;
    }

    private bool isCloseToTarget(Vector3 target){
        return CalcUtils.DistanceToTarget(transform.position, target) < 0.2f;
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
                SoundPropagationManager.Instance.PropagateSound(transform.position, SoundOrigin.ZOMBIE, 0.6f * speedMultiplier);
                SoundManager.Instance.PlayRandomSoundClip(stepSounds, transform, SoundType.FOOTSTEPS, SoundFXType.FX, followTarget: transform);

                stepTimer = 0; // Reset timer
            }
        } else {
            // Reset timer when not moving
            stepTimer = 0;
        }
    }
    #endregion



    #region Effects
    private void PlayDiscoverEffect() {
        float blurAmount = UnityEngine.Random.Range(0.005f, 0.01f);
        zombieRenderer.material.SetFloat("_BlurAmount", blurAmount);
    }

    private void StopDiscoverEffect() {
        zombieRenderer.material.SetFloat("_BlurAmount", 0);
    }

    #endregion

}