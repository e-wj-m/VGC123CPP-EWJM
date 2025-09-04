using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class RangerEnemy : Enemy
{
    [Header("Patrol")]
    [SerializeField, Range(0.5f, 12f)] private float xVelocity = 2.5f;
    [SerializeField] private string flipTag = "Enemy Barriers"; // optional: trigger tag support

    [Header("Turn Sensor (Layer-based)")]
    [SerializeField] private LayerMask turnMask;                       
    [SerializeField] private Vector2 sensorSize = new Vector2(0.15f, 0.9f);
    [SerializeField] private Vector2 sensorOffset = new Vector2(0.25f, 0f);

    [Header("Targeting / LOS")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRadius = 7f;
    [SerializeField] private LayerMask losBlockers;                    // walls/ground that block LOS

    [Header("Ranged Attack")]
    [SerializeField] private float fireRate = 2f;                      // seconds between shots
    [SerializeField] private Transform firePoint;                      // ray origin for LOS
    [SerializeField] private RangerShoot shooter;                       // <— assign this (on Ranger/Bow)

    [Header("Animator Params")]
    [SerializeField] private string pSpeed = "Speed";
    [SerializeField] private string pFire = "Fire";

    private Rigidbody2D rb;
    // NOTE: Enemy base already provides: protected SpriteRenderer sr; protected Animator anim;

    private float shootCooldown;
    private bool spotted;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();

        // Physics QoL
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        if (!player)
        {
            var p = GameObject.FindWithTag("Player");
            if (p) player = p.transform;
        }

        if (fireRate <= 0f) fireRate = 2f;
        if (xVelocity <= 0f) xVelocity = 2.5f;
    }

    private void FixedUpdate()
    {
        // Patrol when not spotted; stop to aim when spotted
        float vx = (!spotted) ? (sr.flipX ? -xVelocity : xVelocity) : 0f;

        var v = rb.linearVelocity;
        v.x = vx;
        rb.linearVelocity = v;

        // Flip if the front sensor hits a turn barrier (layer-based, robust)
        if (!spotted && HitTurnSensor())
        {
            sr.flipX = !sr.flipX;

            // tiny nudge so we don’t sit in the sensor and double-flip
            var v2 = rb.linearVelocity;
            v2.x = (sr.flipX ? -1f : 1f) * Mathf.Max(0.1f, Mathf.Abs(v2.x));
            rb.linearVelocity = v2;
        }

        // Drive Idle<->Walk blend
        anim.SetFloat(pSpeed, Mathf.Abs(v.x));
    }

    private void Update()
    {
        spotted = SeesPlayer();
        if (spotted) FacePlayerX();

        // Idle/Attack cadence
        shootCooldown -= Time.deltaTime;
        if (spotted && shootCooldown <= 0f)
        {
            anim.SetTrigger(pFire);      // Attack clip should return to Idle via Exit Time
            shootCooldown = fireRate;
        }
    }

    public void AE_Shoot() { if (shooter) shooter.Fire(); }
    public void Fire() { AE_Shoot(); }  // wrapper for existing event named "Fire"

    // ---------- Sensors & Collisions ----------
    private bool HitTurnSensor()
    {
        Vector2 origin = (Vector2)transform.position
                       + new Vector2((sr.flipX ? -1f : 1f) * sensorOffset.x, sensorOffset.y);
        return Physics2D.OverlapBox(origin, sensorSize, 0f, turnMask);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(flipTag) && !spotted)
        {
            sr.flipX = !sr.flipX;

            var v = rb.linearVelocity;
            v.x = (sr.flipX ? -1f : 1f) * Mathf.Max(0.1f, Mathf.Abs(v.x));
            rb.linearVelocity = v;
            return;
        }

        var playerProj = other.GetComponent<VickyulaProjectile>();
        if (playerProj)
        {
            // pass huge damage so we definitely die, regardless of current HP
            TakeDamage(int.MaxValue, DamageType.Default);

            // prevent double-hits if multiple colliders fire
            var projGO = playerProj.gameObject;
            if (projGO) Destroy(projGO);
        }
    }

    // ---------- LOS / Facing ----------
    private bool SeesPlayer()
    {
        if (!player) return false;

        Vector2 delta = player.position - transform.position;
        if (delta.sqrMagnitude > detectionRadius * detectionRadius) return false;

        Vector2 from = firePoint ? (Vector2)firePoint.position : (Vector2)transform.position;
        var hit = Physics2D.Raycast(from, delta.normalized, delta.magnitude, losBlockers);
        return hit.collider == null; 
    }

    private void FacePlayerX()
    {
        if (!player) return;
        float lookX = Mathf.Sign(player.position.x - transform.position.x);
        if (lookX != 0f) sr.flipX = (lookX < 0f);
    }

    // ---------- Gizmos (tuning helpers) ----------
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.green;
        var sprite = GetComponent<SpriteRenderer>();
        bool flip = sprite ? sprite.flipX : false;
        Vector2 origin = (Vector2)transform.position
                       + new Vector2((flip ? -1f : 1f) * sensorOffset.x, sensorOffset.y);
        Gizmos.DrawWireCube(origin, sensorSize);
    }
}