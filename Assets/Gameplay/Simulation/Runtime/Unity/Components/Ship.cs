using UnityEngine;
using Gameplay.Simulation.Runtime;
using Company.Utilities.Runtime;
using Company.Utilities.Runtime.Unity;

namespace Gameplay.Simulation.Runtime.Unity
{
    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Destroyable)), RequireComponent(typeof(Team))]
    public class Ship : MonoBehaviour, IShip, IPoolable
    {
        [SerializeField] new Rigidbody2D rigidbody;
        [SerializeField] Destroyable destroyable;
        [SerializeField] Team team;
        [SerializeField] Transform bulletSpawnPoint;

        public System.Numerics.Vector2 Position { get => transform.position.To2D().ToNumerics(); set => transform.position = value.To3D().ToUnity(); }
        public System.Numerics.Vector2 Forward { get => transform.up.To2D().ToNumerics(); set => transform.up = value.To3D().ToUnity(); }
        public bool IsDestroyed { get => destroyable.IsDestroyed; set => destroyable.IsDestroyed = value; }
        public bool IsTeamPlayer { get => team.IsTeamPlayer; set => team.IsTeamPlayer = value; }
        public System.Numerics.Vector2 ThrustForce { get => rigidbody.totalForce.ToNumerics(); set => rigidbody.totalForce = value.ToUnity(); }
        public float AngularVelocity { get => rigidbody.angularVelocity; set => rigidbody.angularVelocity = value; }
        public int Ammo { get; set; }
        public float AmmoReloadCooldown { get; set; }
        public float FireCooldown { get; set; }
        public bool IsTeleporting { get; set; }
        public float TeleportCooldown { get; set; }

        public bool IsThrusting { get => rigidbody.totalForce.magnitude > Mathf.Epsilon; }
        public System.Numerics.Vector2 BulletSpawnPosition { get => bulletSpawnPoint.position.To2D().ToNumerics(); }

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