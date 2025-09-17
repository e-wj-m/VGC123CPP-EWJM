using UnityEngine;

public class VJumpAttackBehaviour : StateMachineBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip diveSound;     

    public GameObject explosionPrefab;

    [Header("Dive")]
    [SerializeField] private float diveForce = 5f;

    [Header("Trail")]
    [SerializeField] private bool spawnTrail = true;
    [SerializeField] private float trailInterval = 0.05f;
    [SerializeField] private float trailLifetime = 0.4f;

    [Header("Impact")]
    [SerializeField] private bool spawnImpact = true;
    [SerializeField] private float impactLifetime = 0.6f;

    private Rigidbody2D rb;
    private PlayerController player;
    private AudioSource audioSource;

    private bool isDiving = false;
    private bool impactSpawned = false;
    private float lastTrailTime = 0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponent<Rigidbody2D>();
        player = animator.GetComponent<PlayerController>();
        audioSource = animator.GetComponent<AudioSource>();  

        isDiving = false;
        impactSpawned = false;
        lastTrailTime = 0f;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null || rb == null) return;

        bool holdingDown = Input.GetKeyUp(KeyCode.DownArrow) || Input.GetAxisRaw("Vertical") < 0f;

        if (!isDiving && player.IsFalling() && holdingDown)
        {
            isDiving = true;
            rb.AddForce(Vector2.down * diveForce, ForceMode2D.Impulse);
            lastTrailTime = Time.time;
        }

        if (spawnTrail && isDiving && holdingDown && player.IsFalling())
        {
            if (Time.time - lastTrailTime >= trailInterval)
            {
                SpawnFX(animator.transform.position, trailLifetime);
                lastTrailTime = Time.time;
            }
        }

        if (spawnImpact && isDiving && !player.IsFalling() && !impactSpawned)
        {
            SpawnFX(animator.transform.position, impactLifetime);

            if (diveSound && audioSource)
                audioSource.PlayOneShot(diveSound);

            impactSpawned = true;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isDiving = false;
        impactSpawned = false;
    }

    private void SpawnFX(Vector3 pos, float lifetime)
    {
        if (!explosionPrefab) return;
        var go = Object.Instantiate(explosionPrefab, pos, Quaternion.identity);
        Object.Destroy(go, lifetime);
    }
}