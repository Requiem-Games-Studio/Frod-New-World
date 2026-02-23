using UnityEngine;

public class DestroyByFire : MonoBehaviour
{
    [Header("Burn Settings")]
    public float burnDuration = 3f;
    private float burnTimer = 0f;
    private bool isBurning = false;

    [Header("Particles")]
    public GameObject burnParticles;
    public GameObject destroyParticles;

    private GameObject currentBurnParticles;

    private SpriteRenderer sr;
    private Color originalColor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    void Update()
    {
        if (!isBurning) return;

        burnTimer += Time.deltaTime;

        // Oscurecer progresivamente
        float t = burnTimer / burnDuration;
        sr.color = Color.Lerp(originalColor, Color.black, t);

        if (burnTimer >= burnDuration)
        {
            DestroyObject();
        }
    }

    public void StartBurning()
    {
        if (isBurning) return;

        isBurning = true;

        burnParticles.SetActive(true);
    }

    void DestroyObject()
    {
        if (destroyParticles != null)
        {
            Instantiate(destroyParticles, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
