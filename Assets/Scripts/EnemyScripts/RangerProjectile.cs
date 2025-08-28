using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public class RangerProjectile : MonoBehaviour
{
    [SerializeField, Range(0.1f, 20f)] private float lifetime = 1.0f;
    [SerializeField] private bool zeroGravity = true;    // straight shot by default

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (!sr) sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (zeroGravity) rb.gravityScale = 0f;
        if (lifetime > 0f) Destroy(gameObject, lifetime);
    }

    public void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
        if (sr) sr.flipX = velocity.x < 0f;
    }
}