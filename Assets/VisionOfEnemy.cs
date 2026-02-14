using UnityEngine;

public class VisionOfEnemy : MonoBehaviour
{
    [SerializeField] private float viewDistance = 5f;
    [SerializeField] private LayerMask playerLayer;

    private bool isSeeingPlayer;
    public PatrolEnemy patrolEnemy;

    private void Update()
    {
        bool canSee = CanSeePlayer();

        // Si antes no lo veía y ahora sí se activó
        if (canSee && !isSeeingPlayer)
        {
            OnPlayerSpotted();
        }

        // Si antes lo veía y ahora no lo perdió
        if (!canSee && isSeeingPlayer)
        {
            OnPlayerLost();
        }

        isSeeingPlayer = canSee;
    }

    private bool CanSeePlayer()
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, viewDistance, playerLayer);

        Debug.DrawRay(transform.position, direction * viewDistance, Color.red);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                PlayerController player = hit.collider.GetComponent<PlayerController>();

                if (!player.isHidden)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnPlayerSpotted()
    {
        Debug.Log("Jugador detectado");

        patrolEnemy.patrol = false;
        // Aquí puedes:
        // Cambiar estado
        // Activar animación
        // Desactivar patrullaje
        // Reproducir sonido
    }

    private void OnPlayerLost()
    {
        Debug.Log("Jugador perdido");
        patrolEnemy.patrol = true;
    }
}
