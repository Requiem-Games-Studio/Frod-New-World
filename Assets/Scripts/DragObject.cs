using UnityEngine;
using UnityEngine.InputSystem;

public class DragObject : MonoBehaviour
{
    private bool isDragging = false;

    private void OnMouseDown()
    {
        if (!PowerupManager.Instance.HasPower(PowerType.Hand))
        {
            return;
        }

        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Mantiene el objeto en el mismo plano
            transform.position = mousePosition;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

}
