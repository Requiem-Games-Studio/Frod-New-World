using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;
    Rigidbody2D rb;
    public GameObject player;
    public PlayerController playerController;

    bool isFacingRight = true;
    public float visionRange;
    public LayerMask playerLayer;

    public float alturaParaSaltar;
    public float fuerzaDeSalto;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Movimiento
        Vector2 direction = (player.transform.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);

        // Vista al jugador
        Vector2 dir = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, visionRange, playerLayer);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            // Player está visible y al frente
        }

        // Salto enemigo
        if (player.transform.position.y > transform.position.y + alturaParaSaltar)
        {
            rb.AddForce(Vector2.up * fuerzaDeSalto, ForceMode2D.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Línea de visión
        Gizmos.color = Color.red;
        Vector3 dir = isFacingRight ? Vector3.right : Vector3.left;
        Gizmos.DrawLine(transform.position, transform.position + dir * visionRange);

        // Rango vertical para saltar
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * alturaParaSaltar);
    }
}
