using UnityEngine;

public class HidingCabinet : MonoBehaviour, IInteractable
{
    public bool longPress { get; set; }
    public bool canInteract { get; set; }
    public GameObject _playerInteractUI { get; set; }

    [SerializeField]
    private Transform _revealPosition;

    private bool PlayerHiding { get; set; }

    private float coolDownTime = 0.5f;
    private float coolDownTimer = 1f;


	private void Awake()
	{
        PlayerHiding = false;
		longPress = true;
		canInteract = true;
	}
    
    private void Update(){
        if(coolDownTimer < coolDownTime){
            coolDownTimer += Time.deltaTime;
        }
    }


    public void Interact(Player player){
        if(coolDownTimer < coolDownTime){return;}
        coolDownTimer = 0f;
        PlayerInteract playerInteract = player.GetComponentInChildren<PlayerInteract>();
        if(PlayerHiding){
            player.controlsEnabled = true;
            PlayerHiding = false;
            longPress = true;
            player.isHiding = false;
            player.transform.position = _revealPosition.position;
            player.GetComponent<SpriteRenderer>().enabled = true;
            player.GetComponentInChildren<Light>().enabled = true;
            playerInteract.ShowUI();
        }
        else{
            playerInteract.HideUI();
            player.controlsEnabled = false;
            PlayerHiding = true;
            longPress = false;
            player.isHiding = true;
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.transform.position = transform.position;
            player.GetComponent<SpriteRenderer>().enabled = false;
            player.GetComponentInChildren<Light>().enabled = false;
        }
    }

    public string GetInteractionPrompt(){
        return "Hide";
    }
}