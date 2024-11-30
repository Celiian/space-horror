using Shapes;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Rendering.Universal;

public class AdaptCollider : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D polygonCollider;
    [SerializeField] private Polygon polygon;

    // This method is called whenever the script is loaded or a value is changed in the inspector
    private void OnValidate()
    {   
        UpdateColliderPoints();
    }

    [Button("Update Collider Points")]
    private void UpdateColliderPoints()
    {
        if(polygon == null) return;
        Vector2[] points = polygon.points.ToArray();
        polygonCollider.points = points;
    }

    [OnValueChanged("polygon")]
    private void OnPolygonChanged()
    {
        UpdateColliderPoints();
    }
}
