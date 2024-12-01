using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using Tree = BehaviorTree.Tree;


public class ZombieBT : Tree
{
    [SerializeField]
    private ZombieStatsSO stats;

    [SerializeField]
    private LayerMask visionLayerMask;


    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected override Node SetupTree()
    {   
        // _root = new Selector(
        //     new List<Node>{
        //         new SequenceOnce( new List<Node>{
        //             new SetData(stats.GetStats()),
        //               new SetData(new Dictionary<string, object>{
        //                     {"pursuingPlayer", false},
        //                     {"playerLost", false},
        //                     {"searchForPlayer", false},
        //                     {"advancedSearchForPlayer", false},
        //                     {"cannotMove", false}
        //                 }),
        //         }),
        //         new Selector( new List<Node>{
        //             new Sequence(new List<Node>{
        //                 new ConditionIsNotMoving(rb),
        //                 new ActionMoveOutOfTheWay(transform, rb, visionLayerMask),
        //             }),
        //             new Sequence( new List<Node>{
        //                 new ConditionPlayerInRange(transform),
        //                 new ConditionPlayerInView(transform),
        //                 new ConditionNoWallInSight(transform, visionLayerMask),
        //                 new ActionTargetPlayer(transform, rb),
        //                 new CheckNotNull("target"),
        //                 new ActionMoveToTarget(transform, rb),
        //             }),
        //             new Sequence( new List<Node>{
        //                 new Selector( new List<Node>{
        //                     new CheckBool("playerLost"),
        //                     new CheckBool("searchForPlayer"),
        //                     new CheckBool("advancedSearchForPlayer"),
        //                 }),
        //                 new SearchForPlayer(transform),
        //                 new CheckNotNull("target"),
        //                 new ActionMoveToTarget(transform, rb),
        //             }),
        //             new Sequence( new List<Node>{
        //                 new ActionRandomPatrol(transform),
        //                 new CheckNotNull("target"),
        //                 new ActionMoveToTarget(transform, rb),
        //             }),
        //         }),
        //     }
        // );
        return _root;
    }
}