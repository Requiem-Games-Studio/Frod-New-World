using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponPrefab; // El arma que representa este pickup

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            WeaponInventory inventory = other.GetComponent<WeaponInventory>();
            if (inventory != null)
            {
                PowerupManager.Instance.UnlockPower(PowerType.Weapon);
                inventory.AddWeapon(weaponPrefab);
                Destroy(gameObject); // Eliminar el pickup del suelo
            }
        }
    }
}
