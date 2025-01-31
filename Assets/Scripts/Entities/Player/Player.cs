using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public PlayerAnimator playerAnimator;

    public GameObject objectiveIndicatorPivot;
    public GameObject objectiveIndicator;
    public GameObject _objectiveIndicatorTarget;

    public bool isDead = false;

    private void Awake() {
        Instance = this;
    }

    [SerializeField] private AudioClip deathSound;

    public void TakeDamage() {
        if(isDead) return;
        isDead = true;
        SoundManager.Instance.PlaySoundClip(deathSound, transform, SoundManager.SoundType.LOUD_FX, SoundManager.SoundFXType.FX);
        PlayerMovement.Instance.canMove = false;
    }

    public void RemoveObjectiveIndicatorTarget() {
        objectiveIndicatorPivot.SetActive(false);
        _objectiveIndicatorTarget = null;
    }

    public void SetObjectiveIndicatorTarget(GameObject target) {
        _objectiveIndicatorTarget = target; 
        if(IntroManager.Instance.isInIntro) return;
        objectiveIndicatorPivot.SetActive(true);
    }

    void Update() {
        if(_objectiveIndicatorTarget != null) {
            Vector2 direction = (_objectiveIndicatorTarget.transform.position - transform.position).normalized;
            if(direction.magnitude > 0.1f) {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                objectiveIndicatorPivot.transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}   