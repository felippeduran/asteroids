using System;
using UnityEngine;

public class SaucerVisual : MonoBehaviour
{
    [Serializable]
    public struct SaucerData
    {
        public SaucerType Type;
        public GameObject Root;
    }

    [SerializeField] Saucer saucer;
    [SerializeField] SaucerData[] data;

    SaucerType currentActiveType;

    void Update()
    {
        if (currentActiveType != saucer.Type)
        {
            currentActiveType = saucer.Type;
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
            d.Root.SetActive(d.Type == saucer.Type);
        }
    }
}