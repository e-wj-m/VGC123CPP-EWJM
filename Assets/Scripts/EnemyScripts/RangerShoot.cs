using UnityEngine;

[DisallowMultipleComponent]
public class RangerShoot : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("Shot Settings")]
    [SerializeField] private Vector2 initShotVelocity = new Vector2(10f, 0f);

    [Header("Spawn Points (children)")]
    [SerializeField] private Transform leftSpawn;
    [SerializeField] private Transform rightSpawn;

    [Header("Projectile Prefab")]
    [SerializeField] private GameObject projectilePrefab; // must have RangerProjectile 

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (!sr) sr = GetComponentInParent<SpriteRenderer>();
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
        var go = Instantiate(projectilePrefab, spawn.position, Quaternion.identity);

        // Prefer new RangerProjectile
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

        Debug.LogError($"[{name}] RangerCast: Projectile prefab missing RangerProjectile/VickyulaProjectile component.");
    }
}