using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ScaleChanger : MonoBehaviour
{
    [SerializeField]
    public float xLength = 1.0f;
    [SerializeField]
    public float yLength = 1.0f;

    [SerializeField]
    private Color color = Color.black;

    [SerializeField]
    private int orderInLayer = 1;

    [SerializeField]
    private bool castShadows = true;

    private GameObject child;
    private PointClouds pointClouds;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private ShadowCaster2D shadowCaster;


    // This method is called whenever the script is loaded or a value is changed in the inspector
    private void OnValidate()
    {
        child = transform.GetChild(0).gameObject;
        shadowCaster = child.GetComponent<ShadowCaster2D>();
        spriteRenderer = child.GetComponent<SpriteRenderer>();
        boxCollider = transform.TryGetComponent<BoxCollider2D>(out var bc) ? bc : null;
        pointClouds = transform.TryGetComponent<PointClouds>(out var pc) ? pc : null;
        UpdateScale();
    }

    private void UpdateScale()
    {
        // Update the localScale based on the length
        child.transform.localScale = new Vector3(xLength, yLength, 0);
        if(pointClouds != null)
        {
            pointClouds.ObjectX = xLength;
            pointClouds.ObjectY = yLength;
        }
        if(boxCollider != null)
        {
            boxCollider.size = new Vector2(xLength + 0.1f, yLength + 0.1f);
        }
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = orderInLayer;
        if(shadowCaster != null)
        {
            shadowCaster.enabled = castShadows;
        }
    }
}
