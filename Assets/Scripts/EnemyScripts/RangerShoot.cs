using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class RangerShoot : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip shootSound;

    private SpriteRenderer sr;
    private AudioSource audioSource;

    [Header("Shot Settings")]
    [SerializeField] private Vector2 initShotVelocity = new Vector2(10f, 0f);

    [Header("Spawn Points (children)")]
    [SerializeField] private Transform leftSpawn;
    [SerializeField] private Transform rightSpawn;

    [Header("Projectile Prefab")]
    [SerializeField] private GameObject projectilePrefab; 

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (!sr) sr = GetComponentInParent<SpriteRenderer>();

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; 
        if (GameManager.Instance && GameManager.Instance.sfxMixerGroup)
        {
            audioSource.outputAudioMixerGroup = GameManager.Instance.sfxMixerGroup;
        }
    }

    private void Start()
    {
        if (!leftSpawn || !rightSpawn || !projectilePrefab)
        {
            Debug.LogError($"[{name}] RangerShoot: Assign Left/Right spawn and projectile prefab in the Inspector.");
        }
    }

    public void Fire()
    {
        if (!projectilePrefab || !leftSpawn || !rightSpawn || !sr) return;

        bool facingRight = !sr.flipX;
        Transform spawn = facingRight ? rightSpawn : leftSpawn;

        if (shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        var go = Instantiate(projectilePrefab, spawn.position, Quaternion.identity);

        var rp = go.GetComponent<RangerProjectile>();
        if (rp)
        {
            rp.SetVelocity(facingRight ? initShotVelocity
                                       : new Vector2(-initShotVelocity.x, initShotVelocity.y));
            return;
        }

        var vp = go.GetComponent<VickyulaProjectile>();
        if (vp)
        {
            vp.SetVelocity(facingRight ? initShotVelocity
                                       : new Vector2(-initShotVelocity.x, initShotVelocity.y));
            return;
        }

        Debug.LogError($"[{name}] RangerShoot: Projectile prefab missing RangerProjectile/VickyulaProjectile component.");
    }
}