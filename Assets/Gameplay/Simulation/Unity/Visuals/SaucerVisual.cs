using System;
using UnityEngine;
using Gameplay.Simulation.Runtime;

namespace Gameplay.Simulation.Unity
{
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
        [SerializeField] SpriteRenderer[] spriteRenderers;
        [SerializeField] ParticleSystem particleSystemPrefab;

        SaucerType currentActiveType;

        void Update()
        {
            if (saucer.IsDestroyed && gameObject.activeSelf)
            {
                var particleSystem = Instantiate(particleSystemPrefab, transform.position, Quaternion.identity);

                var main = particleSystem.main;
                main.startColor = spriteRenderers[0].color;
                particleSystem.GetComponent<ParticleSystemRenderer>().sortingOrder = spriteRenderers[0].sortingOrder;
            }
        }

        void LateUpdate()
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
}