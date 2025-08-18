using UnityEngine;
using Company.Utilities.Runtime;

namespace Gameplay.Simulation.Runtime
{
    public interface IShip : IPoolable
    {
        Vector2 Position { get; set; }
        Vector2 Forward { get; set; }
        bool IsDestroyed { get; set; }
        bool IsTeamPlayer { get; set; }
        Vector2 ThrustForce { get; set; }
        float AngularVelocity { get; set; }
        bool IsThrusting { get; }
        Vector2 BulletSpawnPosition { get; }
        int Ammo { get; set; }
        float AmmoReloadCooldown { get; set; }
        float FireCooldown { get; set; }
        bool IsTeleporting { get; set; }
        float TeleportCooldown { get; set; }
    }

    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Destroyable)), RequireComponent(typeof(Team))]
    public class Ship : MonoBehaviour, IShip, IPoolable
    {
        [SerializeField] new Rigidbody2D rigidbody;
        [SerializeField] Destroyable destroyable;
        [SerializeField] Team team;
        [SerializeField] Transform bulletSpawnPoint;

        public Vector2 Position { get => transform.position; set => transform.position = value; }
        public Vector2 Forward { get => transform.up; set => transform.up = value; }
        public bool IsDestroyed { get => destroyable.IsDestroyed; set => destroyable.IsDestroyed = value; }
        public bool IsTeamPlayer { get => team.IsTeamPlayer; set => team.IsTeamPlayer = value; }
        public Vector2 ThrustForce { get => rigidbody.totalForce; set => rigidbody.totalForce = value; }
        public float AngularVelocity { get => rigidbody.angularVelocity; set => rigidbody.angularVelocity = value; }
        public int Ammo { get; set; }
        public float AmmoReloadCooldown { get; set; }
        public float FireCooldown { get; set; }
        public bool IsTeleporting { get; set; }
        public float TeleportCooldown { get; set; }

        public bool IsThrusting { get => rigidbody.totalForce.magnitude > Mathf.Epsilon; }
        public Vector2 BulletSpawnPosition { get => bulletSpawnPoint.position; }

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
            if (destroyable == null)
            {
                destroyable = GetComponent<Destroyable>();
            }

            if (rigidbody == null)
            {
                rigidbody = GetComponent<Rigidbody2D>();
            }

            if (team == null)
            {
                team = GetComponent<Team>();
            }
        }
    }
}