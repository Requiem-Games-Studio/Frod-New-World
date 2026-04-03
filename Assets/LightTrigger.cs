using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightTrigger : MonoBehaviour
{

    public Light2D globalLight;

    [Header("Nuevo estado")]
    public float targetIntensity = 0.2f;
    public Color targetColor = Color.blue;

    [Header("Estado original")]
    private float originalIntensity;
    private Color originalColor;
    public float speed = 2f;
    private bool inside = false;


    private void Start()
    {
        if (globalLight != null)
        {
            originalIntensity = globalLight.intensity;
            originalColor = globalLight.color;
        }
    }

    private void Update()
    {
        if (inside)
        {
            globalLight.intensity = Mathf.Lerp(globalLight.intensity, targetIntensity, Time.deltaTime * speed);
            globalLight.color = Color.Lerp(globalLight.color, targetColor, Time.deltaTime * speed);
        }
        else
        {
            globalLight.intensity = Mathf.Lerp(globalLight.intensity, originalIntensity, Time.deltaTime * speed);
            globalLight.color = Color.Lerp(globalLight.color, originalColor, Time.deltaTime * speed);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            inside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            inside = false;
    }

}
