using UnityEngine;

public class HandleScript : MonoBehaviour
{
    [Header("Limits")]
    [SerializeField] private float maxUpDistance = 1f;
    [SerializeField] private float maxDownDistance = 1f;

    [Header("Door")]
    [SerializeField] private Animator objectToActive; // Asignar en Inspector

    [Header("State")]
    [SerializeField] private bool estaAbierta;
    [SerializeField] private bool isAobject=true;

    private float lockedX;
    private float startY;
    private float minY;
    private float maxY;

    private bool lastState;
    private float tolerance = 0.05f;
    public AudioSource audioSource;

    private void Awake()
    {
        startY = transform.position.y;
        lockedX = transform.position.x;

        minY = startY - maxDownDistance;
        maxY = startY + maxUpDistance;

        lastState = false;
        estaAbierta = false;
    }

    private void OnEnable()
    {
        Invoke("FindObject",2);
    }

    private void LateUpdate()
    {
        ClampY();

        Vector3 pos = transform.position;
        pos.x = lockedX;
        transform.position = pos;

        CheckState();
    }

    private void ClampY()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    private void CheckState()
    {
        float currentY = transform.position.y;
        bool newState;

        // Si está abajo
        if (Mathf.Abs(currentY - minY) < tolerance)
        {
            newState = true; // abierta
        }
        // Si está arriba
        else if (Mathf.Abs(currentY - maxY) < tolerance)
        {
            newState = false; // cerrada
        }
        else
        {
            return; // si está en medio no cambia nada
        }

        if (newState != lastState)
        {
            lastState = newState;
            estaAbierta = newState;

            ActivarPuerta();
        }
    }

    void FindObject()
    {
        if (isAobject)
        {
            objectToActive = GameObject.FindWithTag("AObject").GetComponent<Animator>();
        }
        else
        {
            objectToActive = GameObject.FindWithTag("BObject").GetComponent<Animator>();
        }
    }

    private void ActivarPuerta()
    {
        if (objectToActive != null)
        {
            // Si la palanca está abierta, la puerta se desactiva (se abre)
            audioSource.Play(); 
            objectToActive.SetBool("isActive", estaAbierta);
        }
    }
}
