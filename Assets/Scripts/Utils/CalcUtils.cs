using UnityEngine;

public static class CalcUtils
{
    public static float DistanceToTarget(Vector3 position, Vector3 target){
        return Vector2.Distance(position, target);
    }
}