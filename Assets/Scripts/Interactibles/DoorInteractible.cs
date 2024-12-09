using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using BehaviorTree;
using Tree = BehaviorTree.Tree;
using Unity.VisualScripting;

public class DoorInteractible : IInteractible
{
    [SerializeField] private GameObject closedDoor;
    [SerializeField] private GameObject openDoor;
    [SerializeField] private BoxCollider2D doorCollider;

    [SerializeField] private AudioClip openDoorSound;
    [SerializeField] private AudioClip closeDoorSound;

    private List<Tree> enemiesClose = new List<Tree>();

    private List<Vector3> positions = new List<Vector3>();

    private void Start()
    {
        base.Init();
        toggleableOptions.Add("isOpen", false);
        toggleableOptions.Add("isClosed", true);
        closedDoor.SetActive(true);
        openDoor.SetActive(false);
        InitWallsPositions();
        AddOrRemoveWalls(true);
    }

    private void AddOrRemoveWalls(bool add)
    {
        foreach (Vector3 position in positions)
        {
            if (add)
            {
                SoundPropagationManager.Instance.addWall(position);
            }
            else
            {
                SoundPropagationManager.Instance.removeWall(position);
            }
        }
    }

    private void InitWallsPositions()
    {
        // Get the bounds of the door collider
        Bounds bounds = doorCollider.bounds;
        
        // Calculate the range of tiles covered by the collider
        Vector2Int minTile = new Vector2Int(Mathf.RoundToInt(bounds.min.x), Mathf.RoundToInt(bounds.min.y));
        Vector2Int maxTile = new Vector2Int(Mathf.RoundToInt(bounds.max.x), Mathf.RoundToInt(bounds.max.y));
        // Iterate over each tile within the bounds
        for (int x = minTile.x; x <= maxTile.x; x++)
        {
            for (int y = minTile.y; y <= maxTile.y; y++)
            {
                Vector2 tileCenter = new Vector2(x + 0.5f, y + 0.5f);
                if(tileCenter.x > maxTile.x || tileCenter.y > maxTile.y || tileCenter.x < minTile.x || tileCenter.y < minTile.y)
                    continue;
                positions.Add(tileCenter);
            }
        }
    }


    public override void Interact()
    {
        // TODO : display options
    }

    public override bool IsInteractible()
    {
        return true;
    }

    public void CloseDoor()
    {
        toggleableOptions["isOpen"] = false;
        toggleableOptions["isClosed"] = true;
        closedDoor.SetActive(true);
        openDoor.SetActive(false);
        AddOrRemoveWalls(true);
        CheckOptions();
        SoundManager.Instance.PlaySoundClip(closeDoorSound, transform, SoundManager.SoundType.FX, SoundManager.SoundFXType.FX);
        SoundPropagationManager.Instance.PropagateSound(transform.position, SoundOrigin.INTERACTIBLE, 0.5f);
    }

    public void OpenDoor()
    {
        toggleableOptions["isOpen"] = true;
        toggleableOptions["isClosed"] = false;
        closedDoor.SetActive(false);
        openDoor.SetActive(true);
        AddOrRemoveWalls(false);
        CheckOptions();
        SoundManager.Instance.PlaySoundClip(openDoorSound, transform, SoundManager.SoundType.FX, SoundManager.SoundFXType.FX);
        SoundPropagationManager.Instance.PropagateSound(transform.position, SoundOrigin.INTERACTIBLE, 0.5f);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        float distance = Vector2.Distance(transform.position, other.transform.position);
         if (other.CompareTag("Enemy"))
        {
            enemiesClose.Add(other.GetComponent<Tree>());
        }
    }


    private IEnumerator CloseDoorCoroutine()
    {
        yield return new WaitForSeconds(4f);
        CloseDoor();
    }

    private void Update()
    {
        if(enemiesClose.Count == 0)
            return;

        foreach(Tree tree in enemiesClose)
        {
            float distance = Vector2.Distance(transform.position, tree.transform.position);
            if(distance > 1.5f)
                continue;
            if(toggleableOptions["isClosed"])
                OpenDoor();
            StartCoroutine(CloseDoorCoroutine());
            enemiesClose.Remove(tree);
            break;
        }
    }
}
