using UnityEngine;
using UnityEngine.InputSystem;

public class DragObject : MonoBehaviour
{
    [Header("Layers")]
    [SerializeField] private LayerMask draggableMask;

    private Camera cam;
    private Plane dragPlane;
    private Vector3 grabOffset;
    private bool isDragging;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryStartDrag();

        if (Input.GetMouseButton(0) && isDragging)
            Drag();

        if (Input.GetMouseButtonUp(0))
            StopDrag();
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

        // Plano perpendicular a la cámara que pasa por el objeto
        dragPlane = new Plane(-cam.transform.forward, transform.position);

        // Offset para que no salte al agarrarlo
        if (dragPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            grabOffset = transform.position - hitPoint;
        }
    }

    private void Drag()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            transform.position = hitPoint + grabOffset;
        }
    }

    private void StopDrag()
    {
        isDragging = false;
    }
}
