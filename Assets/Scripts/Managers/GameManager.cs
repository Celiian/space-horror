using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Light2D globalLight;

    [SerializeField] private int importantPieceCount;

    public Player player;

    public static GameManager Instance { get; private set; }
    private void Awake() => Instance = this;

    public bool turnOffLight = true;
    public bool canEscape { get; set; }

    private void Start() {
        player = FindObjectOfType<Player>();
        canEscape = false;
        if(turnOffLight){
            globalLight.intensity = 0f;
        }
    }


    public void ImportantPieceFound(){
        importantPieceCount--;
        if(importantPieceCount == 0){
            canEscape = true;
        }
    }
}
