using BehaviorTree;
using UnityEngine;
public class CanSeePlayer : Node
{
    private Transform transform;
    private LayerMask visionLayerMask;
    public CanSeePlayer(Transform transform, LayerMask visionLayerMask)
    {
        this.transform = transform;
        this.visionLayerMask = visionLayerMask;
    }

    public override NodeState Evaluate()
    {
        Vector3 playerPosition = PlayerMovement.Instance.transform.position;

        if (IsPlayerInLineOfSight(playerPosition))
        {
            SetTopParentData("playerInSight", true);
            SetTopParentData("currentTargetPosition", playerPosition);
            SetTopParentData("lastKnownPlayerPosition", playerPosition);
            return NodeState.SUCCESS;
        }
        else
        {
            SetTopParentData("currentTargetPosition", null);
            SetTopParentData("playerInSight", false);
            return NodeState.FAILURE;
        }
    }

    private bool IsPlayerInLineOfSight(Vector3 playerPosition)
    {
        if (IsPlayerInRange(playerPosition) && IsPlayerInView(playerPosition) && IsPlayerVisible(playerPosition))
        {
            return true;
        }
        return false;
    }


    private bool IsPlayerInRange(Vector3 playerPosition)
    {
        float visionLength = (float)GetData("visionLength");
        float DistanceToTarget = CalcUtils.DistanceToTarget(transform.position, playerPosition);

        if (DistanceToTarget < visionLength)
        {
            Debug.Log("Player in range");
            return true;
        }
        return false;
    }

    private bool IsPlayerInView(Vector3 playerPosition)
    {
        float visionAngle = (float?)GetData("visionAngle") ?? 0;
        float halfVisionAngle = visionAngle / 2;

        var currentDirection = (Vector3?)GetData("movementDirection");
        if (currentDirection == null) return false;
        Vector2 forwardDirection = ((Vector2)currentDirection.Value).normalized;

        Vector2 directionToTarget = (playerPosition - transform.position).normalized;
        float angle = Vector2.Angle(forwardDirection, directionToTarget);

        if (angle <= halfVisionAngle)
        {
            Debug.Log("Player in view");
            return true;
        }
        return false;
    }

    private bool IsPlayerVisible(Vector3 playerPosition)
    {
        float visionLength = (float)GetData("visionLength");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, playerPosition - transform.position, visionLength, visionLayerMask);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            Debug.DrawRay(transform.position, playerPosition - transform.position, Color.green);
            Debug.Log("Player visible");
            return true;
        }
        else if (hit.collider != null && hit.collider.CompareTag("Wall"))
        {
            Debug.DrawRay(transform.position, playerPosition - transform.position, Color.red);
            return false;
        }
        return false;
    }
}
