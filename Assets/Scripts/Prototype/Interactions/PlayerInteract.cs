using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerInteract : MonoBehaviour
{
	public IInteractable currentInteractable;
	private Animator _interactUIAnimator;
	private Player _player;

	private void Awake()
	{
		_interactUIAnimator = GetComponent<Animator>();
		_player = GetComponentInParent<Player>();
	}

	private void Update()
	{
		if (_player.isInteracting)
		{
			StartInteraction();
		}
		else
		{
			EndInteraction();
		}
	}

	private void StartInteraction()
	{
		_interactUIAnimator.Play("Interacting");
		if (!currentInteractable.longPress)
		{
			currentInteractable?.Interact(_player);
		}
	}

	private void EndInteraction()
	{
		_interactUIAnimator.Play("Idle");
	}

	public void OnInteractAnimationFinished()
	{
		if (currentInteractable.longPress)
		{
			currentInteractable?.Interact(_player);
			EndInteraction();
			if (!currentInteractable.canInteract)
			{
				Destroy(gameObject);
			}
		}
	}

	public void HideUI(){
		transform.GetComponent<SpriteRenderer>().enabled = false;
	}

	public void ShowUI(){
		transform.GetComponent<SpriteRenderer>().enabled = true;
	}

}
