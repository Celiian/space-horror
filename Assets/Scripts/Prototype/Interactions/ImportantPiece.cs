using UnityEngine;

public class ImportantPiece : MonoBehaviour, IInteractable
{
    public bool longPress { get; set; }
    public bool canInteract { get; set; }
    public GameObject _playerInteractUI { get; set; }

    void Awake()
    {
        canInteract = true;
    }

    public void Interact(Player player){
        GameManager.Instance.ImportantPieceFound();
        Destroy(gameObject);
    }

    public string GetInteractionPrompt(){
        return "Pick up";
    }
}