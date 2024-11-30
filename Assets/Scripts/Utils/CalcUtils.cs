using UnityEngine;

public static class CalcUtils
{
    public static float DistanceToTarget(Transform transform, Transform target){
        return Vector2.Distance(transform.position, target.position);
    }
}
