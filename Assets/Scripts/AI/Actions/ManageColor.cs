using BehaviorTree;
using UnityEngine;

public class ManageColor : Node
{
    private SpriteRenderer _renderer;
    private Transform _player;
    private Color _defaultColor;

    public ManageColor(SpriteRenderer renderer)
    {
        _renderer = renderer;
        _defaultColor = renderer.color;
    }

    public override NodeState Evaluate()
    {
        float distanceToPlayer = CalcUtils.DistanceToTarget(PlayerMovement.Instance.transform.position, _renderer.transform.position);
        float hearRadius = PlayerMovement.Instance.hearingRadius;
        // Calculate alpha based on distance
        float alpha;
        if (distanceToPlayer <= hearRadius)
        {
            alpha = 1f;
        }
        else if (distanceToPlayer >= hearRadius * 1.4f)
        {
            alpha = 0f;
        }
        else
        {
            // Smooth transition between hearRadius and hearRadius * 1.4
            alpha = 1f - ((distanceToPlayer - hearRadius) / (hearRadius * 0.4f));
        }

        Color newColor = _defaultColor;
        newColor.a = alpha;
        _renderer.color = newColor;
        _renderer.enabled = alpha > 0;

        return NodeState.FAILURE;
    }
}