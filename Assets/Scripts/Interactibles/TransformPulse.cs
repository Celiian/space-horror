using DG.Tweening;
using UnityEngine; // Make sure to include the DOTween namespace

public class TransformPulse : MonoBehaviour
{
    void Start()
    {
        // Start the pulsing animation
        Pulse();
    }

    void Pulse()
    {
        // Animate the scale to 1.1 over 0.5 seconds, then back to 0.9 over 0.5 seconds, and loop
        transform.DOScale(1.05f, 0.95f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}
