using UnityEngine;

public class PickupsScript : MonoBehaviour
{   
    public enum PickupType
    {
        Relic = 0,
        Powerup = 1
    }

    public PickupType pickupType = PickupType.Relic;

    [SerializeField] private int relicScoreValue = 10;
    [SerializeField] private float powerupSpeedMultiplier = 1.5f;
    [SerializeField] private float powerupDuration = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerController pc = collision.GetComponent<PlayerController>();
        if (pc == null) return;

            switch (pickupType)
            {
                case PickupType.Relic:
                {
                    pc.AddScore(relicScoreValue);

                    pc.AddRelic();

                    var gm = GameManager.Instance;
                    if (gm != null)
                    {
                        gm.TryAddLife(1);

                        Debug.Log($"Sanguine Relic Collected! Lives: {gm.lives}/{gm.maxLives} | Relics Held: {pc.GetRelics()} (Score +{relicScoreValue})");
                    }

                    else
                    {
                        Debug.LogWarning("Relic Collected, but no GameManager instance was found.");
                    }

                    break;
                }

                case PickupType.Powerup:
                    pc.ApplySpeedPowerup(powerupSpeedMultiplier, powerupDuration);
                Debug.Log($"Vampiric Rage! Speed x{powerupSpeedMultiplier} for {powerupDuration:0.#}s");
                    break;
            }

            Destroy(gameObject);

    }
}

