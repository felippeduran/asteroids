using System;
using UnityEngine;
using Gameplay.Simulation.Runtime;
using Gameplay.Simulation.Runtime.Unity;

namespace Gameplay.Visuals.Runtime
{
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
        [SerializeField] ParticleSystem particleSystemPrefab;

        AsteroidType currentActiveType;

        void Update()
        {
            if (asteroid.IsDestroyed && gameObject.activeSelf)
            {
                var particleSystem = Instantiate(particleSystemPrefab, transform.position, Quaternion.identity);

                var main = particleSystem.main;
                main.startColor = spriteRenderers[0].color;
                particleSystem.GetComponent<ParticleSystemRenderer>().sortingOrder = spriteRenderers[0].sortingOrder;
            }
        }

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
}