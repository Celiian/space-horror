using BehaviorTree;
using UnityEngine;

public class LookAroundForPlayer : Node
{
    private Transform transform;
    private float rotationSpeed = 90f;
    private bool firstRotationFinished = false;
    private float totalRotation = 0f;

    public LookAroundForPlayer(Transform transform)
    {
        this.transform = transform;
    }

    public override NodeState Evaluate()
    {
        float rotationThisFrame = rotationSpeed * Time.deltaTime * (firstRotationFinished ? -1 : 1);
        float currentRotation = transform.rotation.eulerAngles.z;
        float targetRotation = currentRotation + rotationThisFrame;

        transform.rotation = Quaternion.Euler(0, 0, targetRotation);
        totalRotation += rotationThisFrame;


        if(!firstRotationFinished && totalRotation >= 45f){
            firstRotationFinished = true;
        }

        if(firstRotationFinished && totalRotation <= -45f){
            firstRotationFinished = false;
            SetTopParentData("lastKnownPlayerPosition", null);
            SetTopParentData("soundPosition", null);
            return NodeState.FAILURE;
        }

        return NodeState.RUNNING;
    }
}
