using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class SkellyEnemy : Enemy
{
    private Rigidbody2D rb;

    [SerializeField, Range(1f, 10f)] private float xVelocity = 2f;
    [SerializeField] private string barrierTag = "Enemy Barriers";

    // Direction of travel: -1 = left, +1 = right
    private int moveDir = -1; // start moving left by default

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        // Make sprite face starting direction
        sr.flipX = (moveDir > 0);  // true if facing right
    }

    void FixedUpdate()
    {
        // Apply velocity based on moveDir
        rb.linearVelocityX = moveDir * xVelocity;
    }

    public override void TakeDamage(int damageValue, DamageType damageType = DamageType.Default)
    {
        if (damageType == DamageType.JumpedOn)
        {
            anim.SetTrigger("Death");
            Destroy(gameObject, 0.5f);
            return;
        }

        base.TakeDamage(damageValue, damageType);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(barrierTag)) return;

        // Flip movement direction
        moveDir *= -1;

        // Flip the sprite so it matches new direction
        sr.flipX = (moveDir > 0);

        // Optional nudge away from wall so we don't stick
        rb.position += new Vector2(moveDir * 0.06f, 0f);
    }
}