using UnityEngine;

public class FollowWithDelay : MonoBehaviour
{
    Transform target;     // El jugador
    public float smoothSpeed = 5f;  // Qué tanto delay
    public bool follow = true;      // Si debe seguir o quedarse quieto

    void Start()
    {
        target = transform.parent;
        // Dejar de ser hijo del jugador
        transform.SetParent(null);
    }

    void Update()
    {
        if (!follow) return; // Si no debe seguir, no hacemos nada

        // Movimiento suave hacia el target
        transform.position = Vector3.Lerp(
            transform.position,
            target.position,
            smoothSpeed * Time.deltaTime
        );
    }

    public void EqualsXTarget()
    {
        transform.position = new Vector2(target.position.x,transform.position.y);
    }
}
