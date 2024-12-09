using BehaviorTree;
using UnityEngine;

public class VisualDebugs : Node
{
    private Transform transform;
    private GameObject visualObject;
    public VisualDebugs(Transform transform){
        this.transform = transform;
        visualObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    }

    public override NodeState Evaluate(){
        bool debug = (bool?)GetData("debug") ?? false;
        if (debug){
            DrawVisionCone();
            DrawLineToTarget();
        }
        return NodeState.FAILURE;
    }

    private void DrawVisionCone(){
        float visionAngle = (float?)GetData("visionAngle") ?? 0;
        float visionLength = (float?)GetData("visionLength") ?? 0;
        float leftAngle = -visionAngle / 2;
        float rightAngle = visionAngle / 2;

        // Draw the main rays
        Vector2 leftDirection = Quaternion.Euler(0, 0, leftAngle) * transform.right;
        Vector2 rightDirection = Quaternion.Euler(0, 0, rightAngle) * transform.right;
        Debug.DrawRay(transform.position, leftDirection * visionLength, Color.red);
        Debug.DrawRay(transform.position, rightDirection * visionLength, Color.red);

        // Draw arc segments
        int segments = 20;
        Vector2 previousPoint = transform.position + (Vector3)(leftDirection * visionLength);
        
        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = leftAngle + ((rightAngle - leftAngle) * i / segments);
            Vector2 direction = (Vector2)(Quaternion.Euler(0, 0, currentAngle) * transform.right);
            Vector2 currentPoint = (Vector2)transform.position + (direction * visionLength);
            
            Debug.DrawLine(previousPoint, currentPoint, Color.red);
            previousPoint = currentPoint;
        }
    }

    private void DrawLineToTarget(){
        object target = GetData("target");
        Vector3 targetPosition;

        switch (target)
        {
            case TileNode tileNode:
                targetPosition = tileNode.GetPosition();
                Debug.DrawLine(transform.position, targetPosition, Color.blue);
                break;

            case Transform targetTransform:
                targetPosition = targetTransform.position;
                Debug.DrawLine(transform.position, targetPosition, Color.blue);
                break;

            default:
                break;
        }
    }


    private void VisualizeTarget(){
        Vector3 currentWayPoint = (Vector3?)GetData("currentTargetPosition") ?? Vector3.zero;

        if(currentWayPoint != Vector3.zero){
            visualObject.SetActive(true);
            visualObject.transform.position = currentWayPoint;
        }
        else {
            visualObject.SetActive(false);
        }
    }

}