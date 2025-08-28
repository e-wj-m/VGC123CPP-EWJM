using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VickyulaProjectile : MonoBehaviour
{
    [SerializeField, Range(1, 20)] private float lifetime = 1.0f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = rb.GetComponent<SpriteRenderer>();
    }

    private void Start() => Destroy(gameObject, lifetime);
    public void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;

        sr.flipX = velocity.x < 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.GetComponentInParent<Enemy>();
        if (!enemy) return;

        enemy.TakeDamage(int.MaxValue, DamageType.Default); // insta-kill for now
        Destroy(gameObject);
    }

}