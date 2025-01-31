using DG.Tweening;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    public static CameraShake instance;

    void Awake() {
        instance = this;
    }

    public void Shake(float duration, float magnitude) {
        transform.DOComplete();
        transform.DOShakePosition(duration, magnitude);
    }

     public void ShakeAdditive(float duration, float maxMagnitude) {
        transform.DOComplete();
        Sequence shakeSequence = DOTween.Sequence();
        float increment = maxMagnitude / 10;
        for (int i = 0; i < 10; i++) {
            shakeSequence.Append(transform.DOShakePosition(duration / 10, maxMagnitude + (increment * i))
                                 .SetEase(Ease.Linear));
        }
        shakeSequence.Play();
    }

    public void ShakeSubtractive(float duration, float maxMagnitude) {
        Sequence shakeSequence = DOTween.Sequence();
        float increment = maxMagnitude / 10;
        for (int i = 0; i < 10; i++) {
            shakeSequence.Append(transform.DOShakePosition(duration / 10, maxMagnitude - (increment * i))
                                 .SetEase(Ease.Linear));
        }
        shakeSequence.Play();
    }


}