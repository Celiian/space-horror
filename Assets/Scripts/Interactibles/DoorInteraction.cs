using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : Interactible
{
    [SerializeField] private GameObject openDoor;
    [SerializeField] private GameObject closedDoor;
    [SerializeField] private AudioClip openDoorSound;
    [SerializeField] private AudioClip closeDoorSound;
    [SerializeField] private bool defaultInteractible = true;
    private bool isOpen = false;
    private bool interactible = false;
    private List<Entity> entitiesClose = new List<Entity>();

    private void Start() {
        interactible = defaultInteractible;
        if(isOpen)
            OpenDoor(false);
        else
            CloseDoor(false);
    }
    
    public override void Interact()
    {
        isOpen = !isOpen;
        if(isOpen)
            OpenDoor();
        else {
            CloseDoor();
        }
    }

    public override bool IsInteractible()
    {
        return interactible;
    }

    public override void ToggleInteractible(bool value)
    {
        interactible = value;
    }


    // Return true if an entity is on the door, because the door is still open
    public void CloseDoor(bool playSound = true)
    {
        // Check if an entity is on the door
        if(Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Enemy", "Player"))){
            isOpen = true;
        }
        openDoor.SetActive(false);
        closedDoor.SetActive(true);
        SoundPropagationManager.Instance.addWall(transform.position);
        if(playSound){
            SoundManager.Instance.PlaySoundClip(closeDoorSound, transform, SoundManager.SoundType.FX, SoundManager.SoundFXType.FX);
            SoundPropagationManager.Instance.PropagateSound(transform.position, SoundOrigin.INTERACTIBLE, 0.5f);
        }
        isOpen = false;
    }

    public void OpenDoor(bool playSound = true)
    {
        isOpen = true;
        openDoor.SetActive(true);
        closedDoor.SetActive(false);
        SoundPropagationManager.Instance.removeWall(transform.position);
        if(playSound){
            SoundManager.Instance.PlaySoundClip(openDoorSound, transform, SoundManager.SoundType.FX, SoundManager.SoundFXType.FX);
            SoundPropagationManager.Instance.PropagateSound(transform.position, SoundOrigin.INTERACTIBLE, 0.5f);
        }
    }


    private new void OnTriggerEnter2D(Collider2D other) {
        base.OnTriggerEnter2D(other);
        if(other.CompareTag("Enemy")){
            entitiesClose.Add(other.GetComponent<Entity>());
            OpenDoor();
        }
    }

    private new void OnTriggerExit2D(Collider2D other) {
        base.OnTriggerExit2D(other);
        if(other.CompareTag("Enemy")){
            entitiesClose.Remove(other.GetComponent<Entity>());
            if(entitiesClose.Count == 0){
                CloseDoor();
            }
        }
    }


    private void Update() {
        if(entitiesClose.Count > 0 && !isOpen){
            OpenDoor();
        }
    }



}