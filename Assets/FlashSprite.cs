using UnityEngine;
using System.Collections;

public class FlashSprite : MonoBehaviour
{
    [Header("Flash Settings")]
    public Material flashMaterial;      // El material blanco

    private Material originalMaterial;  // Material original del sprite
    public SpriteRenderer sr;
    private Coroutine flashRoutine;

    void Awake()
    {
        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }
        originalMaterial = sr.material;
    }

    public void Flash(float duration = 0.05f)
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine(duration));
    }

    private IEnumerator FlashRoutine(float duration)
    {
        sr.material = flashMaterial;
        yield return new WaitForSecondsRealtime(duration);
        sr.material = originalMaterial;
    }
}
