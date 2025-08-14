using UnityEngine;

public class Saucer : MonoBehaviour
{
    [SerializeField] new Rigidbody2D rigidbody;

    public Vector2 Position { get => transform.position; set => transform.position = value; }
}