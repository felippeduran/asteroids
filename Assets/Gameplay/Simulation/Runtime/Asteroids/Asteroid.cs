using UnityEngine;
using Company.Utilities.Runtime;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Destroyable)), RequireComponent(typeof(ScoreWorth))]
public class Asteroid : MonoBehaviour, IPoolable
{
    [SerializeField] AsteroidType type;
    [SerializeField] new Rigidbody2D rigidbody;
    [SerializeField] Destroyable destroyable;
    [SerializeField] ScoreWorth scoreWorth;

    public AsteroidType Type { get => type; set => type = value; }
    public int ScoreWorth { get => scoreWorth.Amount; set => scoreWorth.Amount = value; }
    public Vector2 Position { get => transform.position; set => transform.position = value; }
    public Vector2 LinearVelocity { get => rigidbody.linearVelocity; set => rigidbody.linearVelocity = value; }
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