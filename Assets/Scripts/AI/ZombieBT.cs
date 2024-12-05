using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using Tree = BehaviorTree.Tree;


public class ZombieBT : Tree
{
    [SerializeField]
    private ZombieStatsSO stats;

    [SerializeField]
    private AudioClip[] stepSounds;

    [SerializeField]
    private LayerMask visionLayerMask;

    private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override Node SetupTree()
    {   
        _root = new Parallel(new List<Node>{
            InitializationSequence(),
            new VisualDebugs(transform),
            MainLogic(),

        });
        return _root;
    }


    private Node MainLogic(){
        return new Selector(new List<Node>{
            PursuePlayer(),
            // InvestigateSound(),
            SearchForPlayer(),
            RandomPatrol(),
        });
    }


    private Node PursuePlayer(){
        return new Sequence(new List<Node>{
            new CheckBoolFalse("stuck"),
            new CanSeePlayer(transform, visionLayerMask),
            new MoveTowardsTarget(transform, rb, stepSounds, "currentTargetPosition", 1.5f),
        });
    }

    private Node InvestigateSound(){
        return new Sequence(new List<Node>{
        });
    }

    private Node SearchForPlayer(){
        return new Sequence(new List<Node>{
            new CheckNotNull("lastKnownPlayerPosition"),
            new Selector(new List<Node>{
                new MoveTowardsTarget(transform, rb, stepSounds, "lastKnownPlayerPosition", 1.2f),
                new LookAroundForPlayer(transform),
            }),
        });
    }

    private Node RandomPatrol(){
        return new Sequence(new List<Node>{
            new RandomPatrol(transform),
            new CheckNotNull("currentPatrolPoint"),
            new MoveTowardsTarget(transform, rb, stepSounds, "currentPatrolPoint", 0.1f),
        });
    }

    private Node InitializationSequence()
    {
        return new SequenceOnce(new List<Node>{
            new SetData(stats.GetStats()),
            new SetData(new Dictionary<string, object>{
                {"advancedSearchForPlayer", false},
                {"lastKnownPlayerPosition", null},
                {"currentTargetPosition", null},
                {"cannotGoToPlayer", false},
                {"searchForPlayer", false},
                {"pursuingPlayer", false},
                {"playerInSight", false},
                {"cannotMove", false},
                {"playerLost", false},
                {"stuck", false},
                {"debug", true}
            }),
        });
    }
}