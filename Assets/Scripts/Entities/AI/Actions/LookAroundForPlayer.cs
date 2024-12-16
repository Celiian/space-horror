using BehaviorTree;
using UnityEngine;

public class LookAroundForPlayer : Node
{
    private Transform transform;
    private SpriteRenderer renderer;
    private float waitTime = 0.5f;
    private float timer = 0f;
    private bool isFlipping = false;
        
    public LookAroundForPlayer(Transform transform, SpriteRenderer renderer)
    {
        this.transform = transform;
        this.renderer = renderer;
    }

    public override NodeState Evaluate()
    {
        timer += Time.deltaTime;

        if(timer >= waitTime){
            if(isFlipping){
                renderer.flipX = !renderer.flipX;
                SetTopParentData("lastKnownPlayerPosition", null);
                SetTopParentData("soundPosition", null);
                return NodeState.FAILURE;
            }
            
            renderer.flipX = !renderer.flipX;
            timer = 0f;
            isFlipping = true;
        }

        

        return NodeState.SUCCESS;
    }
}
