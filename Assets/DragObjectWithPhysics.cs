using UnityEngine;

public class DragObjectWithPhysics : MonoBehaviour
{
    [Header("Layers")]
    [SerializeField] private LayerMask draggableMask;

    [Header("Drag Settings")]
    [SerializeField] private float dragSpeed = 15f;
    [SerializeField] private float dragGravity = 0f;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private AudioSource audioSource;

    private Camera cam;
    private Plane dragPlane;
    private Vector3 grabOffset;

    private Rigidbody2D rb;
    private bool isDragging;
    private bool canDrag;

    private Color originalColor;
    private float originalGravity;

    // ===============================
    // UNITY
    // ===============================

    private void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        if (!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();

        originalColor = spriteRenderer.color;
        originalGravity = rb.gravityScale;

        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        if (particles)
            particles.Stop();
    }

    private void Start()
    {
        canDrag = SaveManager.Instance.currentData.takenCollectables.Contains("Hand");
    }

    private void Update()
    {
        if (!canDrag)
            return;

        if (Input.GetMouseButtonDown(0))
            TryStartDrag();

        if (Input.GetMouseButtonUp(0))
            StopDrag();
    }

    private void FixedUpdate()
    {
        if (isDragging)
            Drag();
    }

    // ===============================
    // DRAG LOGIC
    // ===============================

    private void TryStartDrag()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        RaycastHit2D hit = Physics2D.GetRayIntersection(
            ray,
            Mathf.Infinity,
            draggableMask
        );

        if (!hit || hit.transform != transform)
            return;

        isDragging = true;

        // Feedback visual y audio
        spriteRenderer.color = Color.magenta;
        if (audioSource) audioSource.Play();
        if (particles) particles.Play();

        // Preparar físicas
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = dragGravity;

        // Plano de arrastre
        dragPlane = new Plane(-cam.transform.forward, transform.position);

        if (dragPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            grabOffset = transform.position - hitPoint;
        }
    }

    private void Drag()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!dragPlane.Raycast(ray, out float distance))
            return;

        Vector2 targetPos = ray.GetPoint(distance) + grabOffset;

        rb.MovePosition(
            Vector2.Lerp(rb.position, targetPos, dragSpeed * Time.fixedDeltaTime)
        );
    }

    private void StopDrag()
    {
        if (!isDragging) return;

        isDragging = false;

        // Restaurar físicas
        rb.gravityScale = originalGravity;

        // Feedback visual y audio
        spriteRenderer.color = originalColor;
        if (audioSource) audioSource.Pause();
        if (particles) particles.Stop();
    }
}
