using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VickyulaProjectile : MonoBehaviour
{
    [SerializeField, Range(1, 20)] private float lifetime = 1.0f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool hasHit; // guard against double-trigger on composite colliders

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
        if (sr) sr.flipX = velocity.x < 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

       
        var enemy = other.GetComponentInParent<Enemy>();
        if (!enemy) return;

        hasHit = true;

        // Apply damage (insta-kill for now)
        enemy.TakeDamage(int.MaxValue, DamageType.Default);

        var ranger = other.GetComponentInParent<RangerEnemy>();
        if (ranger) ranger.PlayRangerDeath();

        // Remove the projectile on hit
        Destroy(gameObject);
    }
}