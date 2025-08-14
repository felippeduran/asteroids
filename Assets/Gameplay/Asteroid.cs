using System;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] new Rigidbody2D rigidbody;

    public Vector2 Position { get => transform.position; set => transform.position = value; }
    public Vector2 LinearVelocity { get => rigidbody.linearVelocity; set => rigidbody.linearVelocity = value; }
    public bool IsDestroyed { get; private set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IsDestroyed = true;
    }
}