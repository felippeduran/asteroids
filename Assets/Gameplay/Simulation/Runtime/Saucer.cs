using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Destroyable)), RequireComponent(typeof(Team))]
public class Saucer : MonoBehaviour, IPoolable
{
    [Serializable]
    public struct ShootRingData
    {
        public SaucerType Type;
        public float Radius;
        public Transform Center;
        public Vector2 Position { get => Center.position; }
    }

    [SerializeField] SaucerType type;
    [SerializeField] new Rigidbody2D rigidbody;
    [SerializeField] Destroyable destroyable;
    [SerializeField] Team team;
    [SerializeField] ShootRingData[] shootRings;

    public SaucerType Type { get => type; set => type = value; }
    public Vector2 Position { get => transform.position; set => transform.position = value; }
    public bool IsDestroyed { get => destroyable.IsDestroyed; set => destroyable.IsDestroyed = value; }
    public Vector2 LinearVelocity { get => rigidbody.linearVelocity; set => rigidbody.linearVelocity = value; }
    public bool IsTeamPlayer { get => team.IsTeamPlayer; set => team.IsTeamPlayer = value; }
    public float TurnCooldown { get; set; }
    public float ShootCooldown { get; set; }
    public ShootRingData ShootRing { get => shootRings.First(ring => ring.Type == type); }

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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(ShootRing.Center.position, ShootRing.Radius);
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

        if (team == null)
        {
            team = GetComponent<Team>();
        }
    }
}