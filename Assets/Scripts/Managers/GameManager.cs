using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [SerializeField] private List<GameObject> objectives;
    [SerializeField] private StorySoundManager storySound;

    private int objectivesLeft = 0;

    private void Start() {
        objectivesLeft = objectives.Count;
    }

    public void ObjectiveCompleted(GameObject objective){
        objectivesLeft--;
        if(objectivesLeft == 0)
            AllObjectivesCompleted();
        else
            OneObjectiveCompleted();
    }

    public void AllObjectivesCompleted(){
        storySound.TriggerCustomEvent("AllObjectivesCompleted");
    }


    public void OneObjectiveCompleted(){
        storySound.TriggerCustomEvent("OneObjectiveCompleted");
    }
}
