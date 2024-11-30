using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerEnvironmentInteractionCheck : MonoBehaviour
{
	private IInteractable currentInteractable;

	[SerializeField]
	private GameObject _interactUI;

	private PlayerInteract _interactableUI;


	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.TryGetComponent<IInteractable>(out var interactable))
		{
			if (interactable.canInteract)
			{
				currentInteractable = interactable;
				_interactableUI = Instantiate(_interactUI, transform).GetComponent<PlayerInteract>();
				_interactableUI.currentInteractable = interactable;
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.GetComponent<IInteractable>() != null)
		{
			if (_interactableUI != null)
			{
				Destroy(_interactableUI.gameObject);
			}
		}
	}
}
