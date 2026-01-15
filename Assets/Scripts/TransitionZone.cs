using UnityEngine;
using System.Collections;

public class TransitionZone : MonoBehaviour
{
    [Header("Camera Bounds en esta zona")]
    public Collider2D newCameraBounds;   // Los límites de cámara de esta zona

    [Header("Opcional: Transición de cámara")]
    public float transitionTime = 0.5f;    // Tiempo para suavizar el cambio de límites

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            CameraFollow camFollow = Camera.main.GetComponent<CameraFollow>();
            if (camFollow != null && newCameraBounds != null)
            {
                StartCoroutine(ChangeBoundsSmooth(camFollow, newCameraBounds));
            }
        }
    }

    private System.Collections.IEnumerator ChangeBoundsSmooth(CameraFollow camFollow, Collider2D targetBounds)
    {
        isTransitioning = true;

        // Guardamos los bounds actuales
        Collider2D oldBounds = camFollow.cameraBounds;
        camFollow.cameraBounds = targetBounds;

        // Si quieres interpolar entre bounds (opcional)
        float elapsed = 0f;
        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        isTransitioning = false;
    }
}
