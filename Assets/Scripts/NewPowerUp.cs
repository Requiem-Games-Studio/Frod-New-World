using UnityEngine;

public class NewPowerUp : MonoBehaviour
{
    [Header("ID único")]
    public string collectableID;

    public GameObject tutorial;

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
            if(tutorial != null)
            {
                Canvas canvas = FindFirstObjectByType<Canvas>();
                Instantiate(tutorial, canvas.transform, false);
            }


            WeaponInventory inventory = other.GetComponent<WeaponInventory>();
            if (inventory != null)
            {
                // Guardamos este ID en el save
                SaveManager.Instance.currentData.takenCollectables.Add(collectableID);
                Destroy(gameObject); // Eliminar el pickup del suelo
            }
        }
    }
}
