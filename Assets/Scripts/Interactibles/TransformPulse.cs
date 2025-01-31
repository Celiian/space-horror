using DG.Tweening;
using UnityEngine; // Make sure to include the DOTween namespace

public class TransformPulse : MonoBehaviour
{
    public float pulseDuration = 0.95f;
    public float pulseMaxScale = 1.05f;
    public float pulseMinScale = 0.95f;
    void Start()
    {
        // Start the pulsing animation
        Pulse();
    }

    void Pulse()
    {
        // Animate the scale to 1.1 over 0.5 seconds, then back to 0.9 over 0.5 seconds, and loop
        transform.DOScale(pulseMaxScale, pulseDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}
