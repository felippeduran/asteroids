using System;
using System.Runtime.Serialization;
using UnityEngine;

public interface IShip
{
    Vector2 Position { get; set; }
    Vector2 Forward { get; set; }

    void Setup(ShipConfig config);
    void Reset();
    void TurnLeft();
    void TurnRight();
    void Thrust();
}

[Serializable]
public struct ShipConfig
{
    public float ThrustForce;
    public float TurnSpeed;
}

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Destroyable))]
public class Ship : MonoBehaviour, IShip, IPoolable
{
    [SerializeField] ShipConfig config;
    [SerializeField] new Rigidbody2D rigidbody;
    [SerializeField] Destroyable destroyable;

    public Vector2 Position { get => transform.position; set => transform.position = value; }
    public Vector2 Forward { get => transform.up; set => transform.up = value; }
    public bool IsDestroyed { get => destroyable.IsDestroyed; set => destroyable.IsDestroyed = value; }
    public bool IsThrusting { get => rigidbody.totalForce.magnitude > Mathf.Epsilon; }

    public void Setup(ShipConfig config)
    {
        this.config = config;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Reset()
    {
        rigidbody.angularVelocity = 0;
    }

    public void TurnLeft()
    {
        rigidbody.angularVelocity = config.TurnSpeed;
    }

    public void TurnRight()
    {
        rigidbody.angularVelocity = -config.TurnSpeed;
    }

    public void Thrust()
    {
        rigidbody.AddForce(transform.up * config.ThrustForce);
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
    }
}