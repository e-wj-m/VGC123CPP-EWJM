using UnityEngine;

public class Projectile : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] private Vector2 initShotVelocity = Vector2.zero;
    [SerializeField] private Transform leftSpawn;
    [SerializeField] private Transform rightSpawn;
    [SerializeField] private GameObject projectilePrefab = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (initShotVelocity == Vector2.zero)
        {
            initShotVelocity = new Vector2(10f, 0f);
            Debug.LogWarning("Initial Shot Velocity not set. Using default value: " + initShotVelocity);
        }

        if (leftSpawn == null || rightSpawn == null || projectilePrefab == null)
        {
            Debug.LogError("Spawn Points or Projectile Prefab not set. Please assign in the inspector.");
        }
    }
    public void Fire()
    {
        VickyulaProjectile curProjectile;
        if (!sr.flipX)
        {
            curProjectile = Instantiate(projectilePrefab, rightSpawn.position,
             Quaternion.identity).GetComponent<VickyulaProjectile>();
            curProjectile.SetVelocity(initShotVelocity);
        }
        else
        {
            curProjectile = Instantiate(projectilePrefab, leftSpawn.position,
             Quaternion.identity).GetComponent<VickyulaProjectile>();
            curProjectile.SetVelocity(new Vector2(-initShotVelocity.x, initShotVelocity.y));
        }
    }

}
