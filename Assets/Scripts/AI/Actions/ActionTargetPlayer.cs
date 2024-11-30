using BehaviorTree;
using UnityEngine;

public class ActionTargetPlayer : Node
{
    private Transform transform;
    private Rigidbody2D rb;
    public ActionTargetPlayer(Transform transform, Rigidbody2D rb){
        this.transform = transform;
        this.rb = rb;
    }

    public override NodeState Evaluate()
    {
        Transform player = GameManager.Instance.player.transform;
        SetTopParentData("target", player);
        SetTopParentData("searchForPlayer", false);
        SetTopParentData("playerLost", true);
        Debug.Log("Targeting player");


        if(Vector2.Distance(transform.position, player.position) <= 1.2f){
            if(!player.GetComponent<Player>().isDead){
                player.GetComponent<Player>().Die();
                rb.velocity = Vector2.zero;
            }
        }
        return NodeState.SUCCESS;
    }
}
