using UnityEngine;
using Gameplay.Simulation.Runtime.Unity;

namespace Gameplay.Visuals.Runtime
{
    public class ShipVisual : MonoBehaviour
    {
        [SerializeField] GameObject propulsionVisual;
        [SerializeField] Ship ship;
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] ParticleSystem particleSystemPrefab;

        public void Update()
        {
            propulsionVisual.SetActive(ship.IsThrusting ? Random.value < 0.7f : false);

            if (ship.IsDestroyed && gameObject.activeSelf)
            {
                var particleSystem = Instantiate(particleSystemPrefab, transform.position, Quaternion.identity);

                var main = particleSystem.main;
                main.startColor = spriteRenderer.color;
                particleSystem.GetComponent<ParticleSystemRenderer>().sortingOrder = spriteRenderer.sortingOrder;
            }
        }
    }
}