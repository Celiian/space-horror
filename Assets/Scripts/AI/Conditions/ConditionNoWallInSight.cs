using BehaviorTree;
using Sirenix.Utilities;
using UnityEngine;

public class ConditionNoWallInSight : Node
{
    private Transform transform;
    private LayerMask visionLayerMask;
    public ConditionNoWallInSight(Transform transform, LayerMask visionLayerMask){
        this.transform = transform;
        this.visionLayerMask = visionLayerMask;
    }

    public override NodeState Evaluate()
    {
        return ProcessRaycastHit();
    }



    private NodeState ProcessRaycastHit()
    {
        if(visionLayerMask == 0){
            return NodeState.SUCCESS;
        }
        
        float visionLength = (float)GetData("visionLength");
        Transform player = GameManager.Instance.player.transform;
        Vector3 direction = player.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position + direction.normalized, direction, visionLength, visionLayerMask);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Wall"))
            {
                return NodeState.FAILURE;
            }
            if (hit.collider.CompareTag("Player"))
            {
                return NodeState.SUCCESS;
            }
        }
        return NodeState.FAILURE;
    }
}
