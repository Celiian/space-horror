using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using static EnemyAnimator;
using static SoundManager;
using System;

public class Angel : Entity
{
    #region References
    private Rigidbody2D rb;
    #endregion

    #region Parameters

    [FoldoutGroup("Stats"), SerializeField]
    public float moveSpeed;

    #endregion

    #region Variables
    private bool isFacingRight;
    private List<TileNode> currentPath;
    private Vector3 currentTarget;
    private float timeSinceLastUpdate;
    private float speedMultiplier = 0.6f;
    public Vector3 movementDirection;
    public bool debug = false;
    public event Action OnPlayerDiscovered;
    private bool playerDiscovered = false;

    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        isFacingRight = true;
    }

    public override void Update()
    {
        if (isPaused) return;


        if (currentPath == null || currentPath.Count == 0) 
        {
            initPathToPlayer();
            if(!playerDiscovered){
                OnPlayerDiscovered?.Invoke();
                playerDiscovered = true;
            }
        }

        speedMultiplier = 1.5f;
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
                if (currentPath.Count == 0 || currentPath == null)
                {
                    updatePathToPlayer();
                }
            }
            if(currentPath != null && currentPath.Count > 0){
                currentTarget = currentPath.First().GetPosition();
            }
        }

        Vector2 directionToPlayer = (PlayerMovement.Instance.transform.position - transform.position).normalized;
        movementDirection = directionToPlayer;
        float distanceToPlayer = CalcUtils.DistanceToTarget(transform.position, PlayerMovement.Instance.transform.position);
        float soundLevel = SoundPropagationManager.Instance.GetTilesInRadius(transform.position, 3f).SelectMany(tile => tile.soundSources).Sum(sound => sound.soundLevel);
        if(soundLevel < 0.1f && distanceToPlayer < 0.3f) {
            Player.Instance.TakeDamage();
        }

    }

    private void FixedUpdate()
    {
        MoveTowardsTarget(currentTarget);
    }

    #region Actions
    private void MoveTowardsTarget(Vector3 target){
        if(isPaused) return;

        Tile tile = SoundPropagationManager.Instance.getClosestTileFromPosition(target);
        if(tile.soundSources.Sum(sound => sound.soundLevel) > 0.1f){
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 direction = (target - transform.position).normalized;
        rb.velocity = direction * moveSpeed * speedMultiplier;
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
        Vector3 startingPos = currentPath == null || currentPath.Count == 0 ? transform.position : currentPath.First().GetPosition();
        TileNode startNode = PathFinding.Instance.FindNodeCloseToPosition(startingPos);
        TileNode endNode = PathFinding.Instance.FindNodeCloseToPosition(PlayerMovement.Instance.transform.position);
        currentPath = PathFinding.Instance.FindPath(startNode, endNode);
    }

    #endregion

    #region Conditions

    private bool isCloseToTarget(Vector3 target){
        return CalcUtils.DistanceToTarget(transform.position, target) < 0.2f;
    }

    #endregion

}