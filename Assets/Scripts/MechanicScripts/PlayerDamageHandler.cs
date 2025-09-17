using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]  
public class PlayerDamageHandler : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float invulnSeconds = 0.5f;
    [SerializeField] private bool destroyArrowOnHit = true;

    [Header("Audio")]
    [SerializeField] private AudioClip deathSound;     
    private AudioSource audioSource;

    private Animator anim;
    private bool isDead;
    private bool _invulnerable;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; 
        if (GameManager.Instance && GameManager.Instance.sfxMixerGroup)
            audioSource.outputAudioMixerGroup = GameManager.Instance.sfxMixerGroup;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (_invulnerable || isDead) return;
        if (col.collider.GetComponentInParent<Enemy>() != null)
            TakeHit();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_invulnerable || isDead) return;

        if (other.TryGetComponent<RangerProjectile>(out var arrow) ||
            other.GetComponentInParent<RangerProjectile>() != null)
        {
            TakeHit();
            if (destroyArrowOnHit)
            {
                var root = other.attachedRigidbody ? other.attachedRigidbody.gameObject
                                                   : other.transform.root.gameObject;
                Destroy(root);
            }
            return;
        }

        if (other.GetComponentInParent<Enemy>() != null)
            TakeHit();
    }

    private void TakeHit()
    {
        var gm = GameManager.Instance;
        if (gm == null) { Debug.LogWarning("No GameManager instance."); return; }

        gm.lives -= 1;
        Debug.Log($"You've taken damage and lost a life! ({gm.lives}) Lives Left!");

        if (gm.lives <= 0 && !isDead)
            HandleDeath();

        StartCoroutine(InvulnWindow());
    }

    private void HandleDeath()
    {
        isDead = true;
        _invulnerable = true;

        if (deathSound) audioSource.PlayOneShot(deathSound);

        var rb = GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = Vector2.zero;

        var pc = GetComponent<PlayerController>();
        if (pc) pc.enabled = false;

        if (anim)
            anim.SetTrigger("LivesGone"); 
    }

    private IEnumerator InvulnWindow()
    {
        _invulnerable = true;
        yield return new WaitForSeconds(invulnSeconds);
        if (!isDead) _invulnerable = false;
    }

    public void OnDeathAnimationComplete()
    {
        GameManager.Instance?.LoadGameOverScene();
    }
}