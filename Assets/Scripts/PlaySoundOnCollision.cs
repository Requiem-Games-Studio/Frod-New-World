using UnityEngine;

public class PlaySoundOnCollision : MonoBehaviour
{
    public AudioSource audioSource;   // Arrastras aquí el AudioSource
    public AudioClip collisionSound;  // Arrastras aquí el sonido

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            audioSource.PlayOneShot(collisionSound);
        }
    }
}
