using UnityEngine;
using Gameplay.Simulation.Runtime;
using Company.Utilities.Runtime;
using Company.Utilities.Runtime.Unity;
using Logger = Company.Utilities.Runtime.Logger;

namespace Gameplay.Simulation.Runtime.Unity
{
    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Destroyable)), RequireComponent(typeof(Team))]
    public class Bullet : MonoBehaviour, IBullet, IPoolable
    {
        [SerializeField] new Rigidbody2D rigidbody;
        [SerializeField] new Collider2D collider;
        [SerializeField] ScoreWorth scoreWorth;
        [SerializeField] Destroyable destroyable;
        [SerializeField] Team team;

        public System.Numerics.Vector2 Position { get => transform.position.To2D().ToNumerics(); set => transform.position = value.To3D().ToUnity(); }
        public System.Numerics.Vector2 LinearVelocity { get => rigidbody.linearVelocity.ToNumerics(); set => rigidbody.linearVelocity = value.ToUnity(); }
        public bool IsDestroyed { get => destroyable.IsDestroyed; set => destroyable.IsDestroyed = value; }
        public bool IsPlayerBullet { get => team.IsTeamPlayer; set => team.IsTeamPlayer = value; }
        public int Score { get; set; }
        public float TotalTraveledDistance { get; set; }

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
            // Note: Unity doesn't guarantee the execution order of OnTriggerEnter2D callbacks, so it's safer to allow
            // for duplicate scoring instead of trying to track the order of the callbacks on both ends(this and the
            // other object).
            // Example: Consider 2 asteroids (B and C) colliding with this bullet A. All of them implement OnTriggerEnter. In the same frame, we have a trigger event between A and B, and A and C.
            Logger.Log("Bullet collided with {0}", other.attachedRigidbody.name);
            if (other.attachedRigidbody.TryGetComponent<ScoreWorth>(out var scoreWorth))
            {
                Score += scoreWorth.Amount;
            }
            IsDestroyed = true;
        }

        void OnValidate()
        {
            if (rigidbody == null)
            {
                rigidbody = GetComponent<Rigidbody2D>();
            }

            if (destroyable == null)
            {
                destroyable = GetComponent<Destroyable>();
            }

            if (scoreWorth == null)
            {
                scoreWorth = GetComponent<ScoreWorth>();
            }

            if (team == null)
            {
                team = GetComponent<Team>();
            }
        }
    }
}