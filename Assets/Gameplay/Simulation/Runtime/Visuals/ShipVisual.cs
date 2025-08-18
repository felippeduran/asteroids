using UnityEngine;

namespace Gameplay.Simulation.Runtime
{
    public class ShipVisual : MonoBehaviour
    {
        [SerializeField] GameObject propulsionVisual;
        [SerializeField] Ship ship;

        public void Update()
        {
            propulsionVisual.SetActive(ship.IsThrusting ? UnityEngine.Random.value < 0.7f : false);
        }
    }
}