using System.Collections.Generic;
using UnityEngine;

public class PickupsSpawner : MonoBehaviour
{
    [Header("RandPickupSlots - Locations!")]
    [SerializeField] private List<Transform> slots = new();

    [Header("Collectible Spawn Options:")]
    [SerializeField] private GameObject pickupA;
    [SerializeField] private GameObject pickupB;
    [Range(0f, 1f)]
    [SerializeField] private float rareChance = 0.25f;   

    [Header("Spawner's Lifecycle")]
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private Transform spawnedParent;     
    [SerializeField] private bool clearPrevious = true;   

    void Start()
    {
        if (spawnOnStart) Spawn();
    }

    public void Spawn()
    {
        if (clearPrevious && spawnedParent)
        {
            for (int i = spawnedParent.childCount - 1; i >= 0; i--)
                Destroy(spawnedParent.GetChild(i).gameObject);
        }

        foreach (var slot in slots)
        {
            if (!slot) continue;

            GameObject prefab = (Random.value < rareChance) ? pickupB : pickupA;
            if (!prefab) { Debug.LogWarning("[RandPickupsSpawner] Missing collectible prefab!!"); continue; }

            Instantiate(prefab, slot.position, slot.rotation, spawnedParent);
        }
    }

    [ContextMenu("Auto-Fill the slots from the first game object's Child Parent!")]
    private void AutofillSlots()
    {
        slots.Clear();
        if (transform.childCount > 0)
            foreach (Transform t in transform.GetChild(0)) slots.Add(t);
    }
}