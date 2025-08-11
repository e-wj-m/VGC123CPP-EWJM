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
                pc.AddScore(relicScoreValue);
                    Debug.Log($"Sanguine Relic Collected! Current Relics Held: {pc.GetScore()} (+{relicScoreValue})");
                    break;

                case PickupType.Powerup:
                    pc.ApplySpeedPowerup(powerupSpeedMultiplier, powerupDuration);
                Debug.Log($"Vampiric Rage! Speed x{powerupSpeedMultiplier} for {powerupDuration:0.#}s");
                    break;
            }

            Destroy(gameObject);

    }
}

