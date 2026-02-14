using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponPrefab; // El arma que representa este pickup
    [Header("ID único")]
    public string collectableID;

    void Start()
    {
        // Si este collectable ya fue tomado antes no aparece
        if (SaveManager.Instance.currentData.takenCollectables.Contains(collectableID))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            WeaponInventory inventory = other.GetComponent<WeaponInventory>();
            if (inventory != null)
            {
                // Guardamos este ID en el save
                SaveManager.Instance.currentData.takenCollectables.Add(collectableID);
                inventory.AddWeapon(weaponPrefab);
                Destroy(gameObject); // Eliminar el pickup del suelo
            }
        }
    }
}
