using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))] 
public class RangerEnemy : Enemy
{
    [Header("Patrol")]
    [SerializeField, Range(0.5f, 12f)] private float xVelocity = 2.5f;
    [SerializeField] private string flipTag = "Enemy Barriers";

    [Header("Turn Sensor (Layer-based)")]
    [SerializeField] private LayerMask turnMask;
    [SerializeField] private Vector2 sensorSize = new Vector2(0.15f, 0.9f);
    [SerializeField] private Vector2 sensorOffset = new Vector2(0.25f, 0f);

    [Header("Targeting / LOS")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRadius = 7f;
    [SerializeField] private LayerMask losBlockers;

    [Header("Ranged Attack")]
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private RangerShoot shooter;

    [Header("Animator Params")]
    [SerializeField] private string pSpeed = "Speed";
    [SerializeField] private string pFire = "Fire";
    [SerializeField] private string pDie = "Die";

    [Header("Audio")]
    [SerializeField] private AudioClip deathSound;
    private AudioSource audioSource;

    private Rigidbody2D rb;
    private float shootCooldown;
    private bool spotted;
    private bool isDead;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; 
        if (GameManager.Instance && GameManager.Instance.sfxMixerGroup)
            audioSource.outputAudioMixerGroup = GameManager.Instance.sfxMixerGroup;

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
        if (isDead) return;

        float vx = (!spotted) ? (sr.flipX ? -xVelocity : xVelocity) : 0f;

        var v = rb.linearVelocity;
        v.x = vx;
        rb.linearVelocity = v;

        if (!spotted && HitTurnSensor())
        {
            sr.flipX = !sr.flipX;

            var v2 = rb.linearVelocity;
            v2.x = (sr.flipX ? -1f : 1f) * Mathf.Max(0.1f, Mathf.Abs(v2.x));
            rb.linearVelocity = v2;
        }

        anim.SetFloat(pSpeed, Mathf.Abs(v.x));
    }

    private void Update()
    {
        if (isDead) return;

        spotted = SeesPlayer();
        if (spotted) FacePlayerX();

        shootCooldown -= Time.deltaTime;
        if (spotted && shootCooldown <= 0f)
        {
            anim.SetTrigger(pFire);
            shootCooldown = fireRate;
        }
    }

    public void AE_Shoot() { if (shooter) shooter.Fire(); }
    public void Fire() { AE_Shoot(); }

    public void PlayRangerDeath()
    {
        if (isDead) return;
        isDead = true;

        if (deathSound) audioSource.PlayOneShot(deathSound);

        // stop behaviour
        spotted = false;
        shootCooldown = float.PositiveInfinity;

        // stop movement/physics
        if (rb)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        // stop shooting
        if (shooter) shooter.enabled = false;

        // prevent further hits
        foreach (var c in GetComponentsInChildren<Collider2D>(includeInactive: true))
            c.enabled = false;

        // reset animator params
        anim.ResetTrigger(pFire);
        anim.SetFloat(pSpeed, 0f);

        // play death animation
        if (!string.IsNullOrEmpty(pDie))
            anim.SetTrigger(pDie);
        else
            anim.Play("RangerDeath", 0, 0f);
    }

    public void AE_Despawn()
    {
        Destroy(gameObject);
    }

    private bool HitTurnSensor()
    {
        Vector2 origin = (Vector2)transform.position
                       + new Vector2((sr.flipX ? -1f : 1f) * sensorOffset.x, sensorOffset.y);
        return Physics2D.OverlapBox(origin, sensorSize, 0f, turnMask);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag(flipTag) && !spotted)
        {
            sr.flipX = !sr.flipX;

            var v = rb.linearVelocity;
            v.x = (sr.flipX ? -1f : 1f) * Mathf.Max(0.1f, Mathf.Abs(v.x));
            rb.linearVelocity = v;
        }
    }

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