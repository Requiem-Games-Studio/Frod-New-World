using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [Header("Break Settings")]
    [SerializeField] private GameObject breakParticles; // Prefab de partículas

    private bool isBroken = false;
    public float postureDamage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isBroken) return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            BreakEvent();
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.SendMessage("BreakObject", postureDamage);
            BreakEvent();
        }
    }

    public void BreakEvent()
    {
        isBroken = true;

        // Instancia partículas
        if (breakParticles != null)
        {
            Instantiate(breakParticles, transform.position, Quaternion.identity);
        }

        // Destruye el objeto
        Destroy(gameObject);
    }
}
