using System.Collections.Generic;
using BehaviorTree;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Tree = BehaviorTree.Tree; 

public class ZombieBT : Tree
{
    [SerializeField]
    private ZombieStatsSO stats;

    [SerializeField]
    private AudioClip[] stepSounds;

    [SerializeField]
    private LayerMask visionLayerMask;

    [SerializeField]
    private EnemyAnimator enemyAnimator;



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

    [Button]
    private async void ResetTree(){
        _root = null;
        await System.Threading.Tasks.Task.Delay(1000);
        _root = new Parallel(new List<Node>{
            InitializationSequence(),
            new VisualDebugs(transform),
            MainLogic(),
        });
    }


    private Node MainLogic(){
        return new Selector(new List<Node>{
            PursuePlayer(),
            // InvestigateSound(),
            // SearchForPlayer(),
            RandomPatrol(),
        });
    }


    private Node PursuePlayer(){
        return new Sequence(new List<Node>{
            new CanSeePlayer(transform, visionLayerMask),
            new TargetPlayer(transform),
            new CheckNotNull("currentPlayerPosition"),
            new MoveTowardsTarget(transform, rb, stepSounds, "currentPlayerPosition", 1.5f, enemyAnimator),
        });
    }

    private Node InvestigateSound(){
        return new Sequence(new List<Node>{
            new InvestigateSound(transform),
            new CheckNotNull("soundPosition"),
            new MoveToSoundHeard(transform),
            new MoveTowardsTarget(transform, rb, stepSounds, "currentSoundTargetPosition", 0.1f, enemyAnimator),
            new LookAroundForPlayer(transform, spriteRenderer),
        });
    }

    private Node SearchForPlayer(){
        return new Sequence(new List<Node>{
            new CheckNotNull("lastKnownPlayerPosition"),
            new Selector(new List<Node>{
                new MoveTowardsTarget(transform, rb, stepSounds, "lastKnownPlayerPosition", 0.1f,enemyAnimator, 1.2f),
                new LookAroundForPlayer(transform, spriteRenderer),
            }),
        });
    }

    private Node RandomPatrol(){
        return new Sequence(new List<Node>{
            new RandomPatrol(transform),
            new CheckNotNull("currentPatrolPoint"),
            new MoveTowardsTarget(transform, rb, stepSounds, "currentPatrolPoint", 0.1f, enemyAnimator),
        });
    }

    private Node InitializationSequence()
    {
        return new SequenceOnce(new List<Node>{
            new SetData(stats.GetStats()),
            new SetData(new Dictionary<string, object>{
                {"currentSoundTargetPosition", null},
                {"movementDirection", Vector3.zero},
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