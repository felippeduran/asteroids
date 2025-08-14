using System;
using System.Runtime.Serialization;
using UnityEngine;

public interface IShip
{
    Vector2 Position { get; set; }

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

public class Ship : MonoBehaviour, IShip
{
    [SerializeField] ShipConfig config;
    [SerializeField] new Rigidbody2D rigidbody;

    public Vector2 Position { get => transform.position; set => transform.position = value; }

    public void Setup(ShipConfig config)
    {
        this.config = config;
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
}