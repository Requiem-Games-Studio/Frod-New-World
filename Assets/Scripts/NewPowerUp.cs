using UnityEngine;

public class NewPowerUp : MonoBehaviour
{
    public PowerType power;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PowerupManager.Instance.UnlockPower(power);
            Destroy(gameObject); // destruir el ítem
        }
    }
}
