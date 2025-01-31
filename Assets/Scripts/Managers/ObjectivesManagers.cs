using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using System.Collections;
using System.Linq;
using DG.Tweening;
using System;
using UnityEngine.Analytics;

public class ObjectivesManager : MonoBehaviour
{
    public static ObjectivesManager Instance;

    [FoldoutGroup("UI Elements")]
    [SerializeField, Required]
    private GameObject objectivesUI;

    [FoldoutGroup("UI Elements")]
    [SerializeField, Required]
    private GameObject objectivePrefab;


    [FoldoutGroup("Audio")]
    [SerializeField, Required]
    private AudioClip minorObjectiveCompleteSound;

    [FoldoutGroup("Audio")]
    [SerializeField, Required]
    private AudioClip majorObjectiveCompleteSound;


    [Title("Objectives")]
    [SerializeField, Required, ListDrawerSettings()]
    private List<MajorObjective> objectives = new List<MajorObjective>();

    
    [ShowInInspector, ReadOnly]
    private Dictionary<MinorObjective, GameObject> _currentObjectives = new Dictionary<MinorObjective, GameObject>();

    [ShowInInspector, ReadOnly]
    private int _currentMajorObjectiveIndex = 0;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start() {
        if(objectives.Count > 0)
        {
            if(objectives[0].minorObjectives.Count > 0)
            {
                if(objectives[0].displayAllObjectives)
                {
                    foreach(var minorObjective in objectives[0].minorObjectives)
                    {
                        InstantiateObjective(minorObjective);
                    }
                }
                else {
                    MinorObjective nextObjective = GetNextObjective(objectives[0]);
                    if(nextObjective != null)
                    {
                        InstantiateObjective(nextObjective);
                    }
                }
            }
        }
    }

    private MinorObjective GetNextObjective(MajorObjective majorObjective)
    {
        return majorObjective.minorObjectives.Find(o => !o.isCompleted);
    }

    public void CompleteObjective(string minorObjectiveName)
    {

        MinorObjective minorObjective = _currentObjectives.Keys.First(o => o.name == minorObjectiveName);
        if(minorObjective == null) return;

        minorObjective.isCompleted = true;

        GameObject minorObjectiveText = _currentObjectives[minorObjective];
        minorObjectiveText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
        MinorObjective nextObjective = GetNextObjective(objectives[_currentMajorObjectiveIndex]);

        if(nextObjective != null)
        {
            SoundManager.Instance.PlaySoundClip(minorObjectiveCompleteSound, PlayerMovement.Instance.transform, SoundManager.SoundType.FX, SoundManager.SoundFXType.FX);
            
            if(objectives[_currentMajorObjectiveIndex].displayAllObjectives)
                return;

            // If there is a next minor objective, display it in the list.
            InstantiateObjective(nextObjective, false);
        }
        else 
        {
            // If there is no next minor objective, complete the major objective and show the next major objective.
            GoToNextMajorObjective(false);
            SoundManager.Instance.PlaySoundClip(majorObjectiveCompleteSound, PlayerMovement.Instance.transform, SoundManager.SoundType.FX, SoundManager.SoundFXType.FX);
        }   
    }


    // I had no choice because i can't have two arguments in an Action so i cannot add a skip option to the CompleteObjective function
    public void SkipCurrentObjective()
    {
        MajorObjective majorObjective = objectives[_currentMajorObjectiveIndex];
        if(majorObjective == null) return;

        foreach(var minorObjective in majorObjective.minorObjectives)
        {
            minorObjective.isCompleted = true;
        }

        GoToNextMajorObjective(true);
    }


    private void GoToNextMajorObjective(bool skip = false)
    {
        _currentMajorObjectiveIndex++;
        
        Action callback = null;

        if(_currentMajorObjectiveIndex < objectives.Count)
        {
            if(objectives[_currentMajorObjectiveIndex].displayAllObjectives)
            {
                foreach(var minorObjective in objectives[_currentMajorObjectiveIndex].minorObjectives)
                {
                   callback += () => InstantiateObjective(minorObjective, skip);
                }
            }
            else {
                MinorObjective nextObjective = GetNextObjective(objectives[_currentMajorObjectiveIndex]);
                if(nextObjective != null)
                {
                    callback = () => InstantiateObjective(nextObjective, skip);
                }
                else {
                    callback = () => GoToNextMajorObjective();
                }
            }
        }

        StartCoroutine(RemoveCurrentObjectives(callback, skip));

    }

    private IEnumerator RemoveCurrentObjectives(Action callback, bool skip = false)
    {
        foreach(var objective in _currentObjectives.Reverse())
        {
            if(skip)
            {
                Destroy(objective.Value);
            }
            else
            {
                objective.Value.GetComponent<TextMeshProUGUI>().DOFade(0, 1f).OnComplete(() => Destroy(objective.Value));
                yield return new WaitForSeconds(1f);
            }
        }
        _currentObjectives.Clear();
        callback?.Invoke();
    }

    private void InstantiateObjective(MinorObjective minorObjective, bool skip = false)
    {

        GameObject objectiveInstance = Instantiate(objectivePrefab, objectivesUI.transform);

        TextMeshProUGUI objectiveText = objectiveInstance.GetComponent<TextMeshProUGUI>();
        objectiveText.text = minorObjective.description;
        objectiveText.color = new Color(objectiveText.color.r, objectiveText.color.g, objectiveText.color.b, 0);
        
        if(skip)
            objectiveText.color = new Color(objectiveText.color.r, objectiveText.color.g, objectiveText.color.b, 1);
        else
            objectiveText.DOFade(1f, 2f);
        

        _currentObjectives.Add(minorObjective, objectiveInstance);

        if(minorObjective.position != null)
        {
            Player.Instance.SetObjectiveIndicatorTarget(minorObjective.position);
        }
        else {
            Player.Instance.RemoveObjectiveIndicatorTarget();

        }
    }
}

[System.Serializable]
public class MinorObjective
{
    public string name;
    public string description;
    public bool isCompleted = false;
    public GameObject position;
}

[System.Serializable]
public class MajorObjective {
    public string name;
    public bool displayAllObjectives = false;
    public List<MinorObjective> minorObjectives = new List<MinorObjective>();
}