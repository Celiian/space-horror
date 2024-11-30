using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] private GameObject lightPrefab;
    private Dictionary<Vector2, Tuple<Coroutine, GameObject>> soundPositions = new Dictionary<Vector2, Tuple<Coroutine, GameObject>>();
    public static LightManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    public void AddSoundPosition(Vector2 position)
    {
        if (soundPositions.ContainsKey(position))
        {
            StopCoroutine(soundPositions[position].Item1);
        }

        Coroutine removeCoroutine = StartCoroutine(RemoveSoundPositionAfterDelay(position, 10f));
        GameObject light = AddLightToPosition(position);
        soundPositions[position] = new Tuple<Coroutine, GameObject>(removeCoroutine, light);
    }

    private IEnumerator RemoveSoundPositionAfterDelay(Vector2 position, float delay)
    {
        yield return new WaitForSeconds(delay);
        RemoveSoundPosition(position);
        }

    public void RemoveSoundPosition(Vector2 position)
    {
        if (soundPositions.TryGetValue(position, out Tuple<Coroutine, GameObject> tuple))
        {
            StopCoroutine(tuple.Item1);
            Destroy(tuple.Item2);
            soundPositions.Remove(position);
        }
    }


    private GameObject AddLightToPosition(Vector2 position)
    {
        GameObject light = Instantiate(lightPrefab, position, Quaternion.identity);
        light.transform.parent = this.transform;
        return light;
    }

}
