using UnityEngine;
using Gameplay.Simulation.Runtime;
using Company.Utilities.Runtime;
using Company.Utilities.Runtime.Unity;

namespace Gameplay.Simulation.Runtime.Unity
{
    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Destroyable)), RequireComponent(typeof(ScoreWorth))]
    public class Asteroid : MonoBehaviour, IAsteroid, IPoolable
    {
        [SerializeField] AsteroidType type;
        [SerializeField] new Rigidbody2D rigidbody;
        [SerializeField] Destroyable destroyable;
        [SerializeField] ScoreWorth scoreWorth;

        public AsteroidType Type { get => type; set => type = value; }
        public int ScoreWorth { get => scoreWorth.Amount; set => scoreWorth.Amount = value; }
        public System.Numerics.Vector2 Position { get => transform.position.To2D().ToNumerics(); set => transform.position = value.To3D().ToUnity(); }
        public System.Numerics.Vector2 LinearVelocity { get => rigidbody.linearVelocity.ToNumerics(); set => rigidbody.linearVelocity = value.ToUnity(); }
        public bool IsDestroyed { get => destroyable.IsDestroyed; set => destroyable.IsDestroyed = value; }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            IsDestroyed = true;
        }

        void OnValidate()
        {
            if (rigidbody == null)
            {
                rigidbody = GetComponent<Rigidbody2D>();
            }

            if (scoreWorth == null)
            {
                scoreWorth = GetComponent<ScoreWorth>();
            }

            if (destroyable == null)
            {
                destroyable = GetComponent<Destroyable>();
            }
        }
    }
}