using System;
using UnityEngine;

public class AsteroidVisual : MonoBehaviour
{
    [Serializable]
    public struct AsteroidData
    {
        public AsteroidType Type;
        public GameObject Root;
    }

    [SerializeField] Asteroid asteroid;
    [SerializeField] Gradient colorRange;
    [SerializeField] SpriteRenderer[] spriteRenderers;
    [SerializeField] AsteroidData[] data;

    AsteroidType currentActiveType;

    void LateUpdate()
    {
        if (currentActiveType != asteroid.Type)
        {
            currentActiveType = asteroid.Type;
            UpdateVisual();
        }
    }

    void OnEnable()
    {
        UpdateVisual();
    }

    void UpdateVisual()
    {
        foreach (var d in data)
        {
            d.Root.SetActive(d.Type == asteroid.Type);
            d.Root.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360));
        }

        var color = colorRange.Evaluate(UnityEngine.Random.value);
        var sortingOrder = (int)(UnityEngine.Random.value * 1000);
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = color;
            spriteRenderer.sortingOrder = sortingOrder;
        }
    }
}