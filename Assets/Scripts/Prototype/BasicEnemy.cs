using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class DetectionSettings {
    public LayerMask detectionMask;
    public LayerMask wallLayer;
    public float detectionRadius;
    public float visionRadius;
    public float soundDetectionRadius;
}

[System.Serializable]
public class MovementSettings {
    public float speed;
    public float rotationSpeed = 10f;
}

public class BasicEnemy : MonoBehaviour
{
    [SerializeField] private DetectionSettings detectionSettings;
    [SerializeField] private MovementSettings movementSettings;
    [SerializeField] private Transform target;

    private Player player;
    private Rigidbody2D rb;
    private FootSteps footSteps;

    private (Vector3 position, bool isPlayer) targetToFollow;
    private int currentPatrolPointIndex = 0;
    private List<PathNode> pathNodes = new List<PathNode>();

    void Start(){
        player = FindObjectOfType<Player>();
        PathNode startNode = PathFinding.Instance.FindNodeCloseToPosition(transform.position);
        // pathNodes = PathFinding.Instance.GetRandomPath(start: startNode);
        targetToFollow = (pathNodes[0].transform.position, false);

        rb = GetComponent<Rigidbody2D>();
        footSteps = GetComponent<FootSteps>();
    }

    private void DrawVisionCone(){
        float leftAngle = -detectionSettings.visionRadius / 2;
        float rightAngle = detectionSettings.visionRadius / 2;

        Vector2 leftDirection = Quaternion.Euler(0, 0, leftAngle) * transform.right;
        Debug.DrawRay(transform.position, leftDirection * detectionSettings.detectionRadius, Color.red);

        Vector2 rightDirection = Quaternion.Euler(0, 0, rightAngle) * transform.right;
        Debug.DrawRay(transform.position, rightDirection * detectionSettings.detectionRadius, Color.red);
    }

    void Update()
    {
        DrawVisionCone();
        Vector2 directionToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        
        targetToFollow.isPlayer = false;
        if(!player.isHiding && !player.isDead){
            if (IsTargetWithinDetectionRadius(distanceToTarget) && IsTargetWithinVisionCone(directionToTarget))
            {
                ProcessRaycastHit(directionToTarget);
            }
        }

        if(!targetToFollow.isPlayer){
            targetToFollow.position = pathNodes[currentPatrolPointIndex].transform.position;
        }
    }

    private bool IsTargetWithinDetectionRadius(float distance)
    {
        return distance < detectionSettings.detectionRadius;
    }

    private bool IsTargetWithinVisionCone(Vector2 directionToTarget)
    {
        float angle = Vector2.Angle(transform.right, directionToTarget);
        return angle < detectionSettings.visionRadius / 2;
    }

    private void ProcessRaycastHit(Vector2 directionToTarget)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, detectionSettings.detectionRadius, detectionSettings.detectionMask);
        Debug.DrawRay(transform.position, directionToTarget * detectionSettings.detectionRadius, Color.red);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Wall"))
            {
                return;
            }
            if (hit.collider.CompareTag("Player"))
            {
                targetToFollow.position = hit.collider.transform.position;
                targetToFollow.isPlayer = true;
            }
        }
    }

    void FixedUpdate()
    {
        float distanceToTarget = Vector2.Distance(transform.position, targetToFollow.position);
        if(targetToFollow.isPlayer){
            Debug.Log("Pursuit mode");
        }
        if(distanceToTarget > 0.1f){
            Vector2 directionToTarget = (targetToFollow.position - transform.position).normalized;
            float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

            rb.rotation = Mathf.LerpAngle(rb.rotation, targetAngle, movementSettings.rotationSpeed * Time.fixedDeltaTime);
            if(targetToFollow.isPlayer){
                rb.velocity = directionToTarget * movementSettings.speed * 1.5f;
                footSteps.speed = 0.9f;
            }
            else{
                rb.velocity = directionToTarget * movementSettings.speed;
                footSteps.speed = 1f;
            }
        }

        if(distanceToTarget <= 2.2f && targetToFollow.isPlayer){
            if(!target.GetComponent<Player>().isDead){
                target.GetComponent<Player>().Die();
                rb.velocity = Vector2.zero;
            }
        }
        
        if(distanceToTarget <= 0.1f && !targetToFollow.isPlayer){
            if(currentPatrolPointIndex == pathNodes.Count - 1){
                // pathNodes = PathFinding.Instance.GetRandomPath(start: pathNodes[currentPatrolPointIndex]);
                currentPatrolPointIndex = 0;
            }
            else{
                currentPatrolPointIndex++;
            }
        }
    }

}
