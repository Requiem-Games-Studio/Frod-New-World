using UnityEngine;

public class Bat : MonoBehaviour
{
    public enum BatState { Sleeping, Awakening, Chasing }
    public BatState currentState = BatState.Sleeping;

    [Header("Detection")]
    public float detectionRadius = 5f;
    public float yDistancte;
    public LayerMask playerLayer;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float chaoticStrength = 1.5f;
    public float flapSpeed = 8f;

    [Header("References")]
    public Transform player;
    public Animator animator;
    public WeakEnemyStats enemyStats;

    private Rigidbody2D rb;
    private float flapTimer;
    private Vector2 randomOffset;

    [Header("Attack")]
    public float attackDistance = 1.5f;
    private bool isAttacking,dead;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Que no caiga
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        switch (currentState)
        {
            case BatState.Sleeping:
                DetectPlayer();
                break;

            case BatState.Awakening:
                // Pequeño delay si quieres animación
                currentState = BatState.Chasing;
                break;

            case BatState.Chasing:
                ChasePlayer();
                break;
        }
    }

    void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(new Vector3(transform.position.x,transform.position.y + yDistancte,0), detectionRadius, playerLayer);

        if (hit != null)
        {
            player = hit.transform;
            WakeUp();
        }
    }

    void WakeUp()
    {
        currentState = BatState.Awakening;

        if (animator != null)
            animator.SetTrigger("Wake");

        Invoke(nameof(StartChasing), 0.5f);
    }

    void StartChasing()
    {
        currentState = BatState.Chasing;
    }

    void ChasePlayer()
    {
        if (player == null || enemyStats.confused || dead) return;

        FlipTowardsPlayer();

        Vector2 direction = (player.position - transform.position).normalized;

        // Movimiento base hacia el jugador
        Vector2 baseMovement = direction * moveSpeed;

        // Movimiento caótico tipo aleteo
        flapTimer += Time.deltaTime * flapSpeed;
        float verticalFlap = Mathf.Sin(flapTimer) * chaoticStrength;

        // Pequeña variación aleatoria
        randomOffset = Random.insideUnitCircle * 0.1f;

        Vector2 chaoticMovement = new Vector2(0, verticalFlap) + randomOffset;

        rb.linearVelocity = baseMovement + chaoticMovement;

        float distance = Vector2.Distance(transform.position, player.position);

        // Activar ataque si está cerca
        if (distance <= attackDistance)
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }

        if (animator != null)
            animator.SetBool("isAttacking", isAttacking);
    }

    void FlipTowardsPlayer()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y + yDistancte, 0), detectionRadius);
    }

    public void Dead()
    {
        dead = true;
        rb.gravityScale = 1;
        Destroy(gameObject,1f);
    }
}
