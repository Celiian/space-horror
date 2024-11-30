using UnityEngine;

public interface IInteractable
{
	bool longPress { get; set; }
	bool canInteract { get; set; }
	GameObject _playerInteractUI { get; set; }
	void Interact(Player player);
	string GetInteractionPrompt();
}