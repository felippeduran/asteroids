using UnityEngine;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Destroyable))]
public class Saucer : MonoBehaviour, IPoolable
{
    [SerializeField] SaucerType type;
    [SerializeField] new Rigidbody2D rigidbody;
    [SerializeField] Destroyable destroyable;

    public SaucerType Type { get => type; set => type = value; }
    public Vector2 Position { get => transform.position; set => transform.position = value; }
    public bool IsDestroyed { get => destroyable.IsDestroyed; set => destroyable.IsDestroyed = value; }
    public Vector2 LinearVelocity { get => rigidbody.linearVelocity; set => rigidbody.linearVelocity = value; }
    public float TurnCooldown { get; set; }
    public float ShootCooldown { get; set; }

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

        if (destroyable == null)
        {
            destroyable = GetComponent<Destroyable>();
        }
    }
}