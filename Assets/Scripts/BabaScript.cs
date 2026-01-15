using UnityEngine;

public class BabaScript : MonoBehaviour
{
    [Header("References")]
    private Transform player;
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Chase Settings")]
    [SerializeField] private float chaseDistance = 6f;
    [SerializeField] private float loseDistance = 8f;
    [SerializeField] private float stopDistance = 0.5f;

    [Header("States")]
    public bool dead;
    public StatsEnemy stats; // asumo que aquí está confused

    private bool chasing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (dead || stats.confused)
        {
            StopMovement();
            return;
        }

        HandleChaseState();
        HandleMovement();
    }

    private void HandleChaseState()
    {
        float sqrDistance =
            (player.position - transform.position).sqrMagnitude;

        if (!chasing && sqrDistance <= chaseDistance * chaseDistance)
        {
            chasing = true;
        }
        else if (chasing && sqrDistance > loseDistance * loseDistance)
        {
            chasing = false;
        }
    }

    private void HandleMovement()
    {
        if (!chasing)
        {
            StopMovement();
            return;
        }

        float xDiff = player.position.x - transform.position.x;

        // Si está muy cerca, no se mueve
        if (Mathf.Abs(xDiff) <= stopDistance)
        {
            StopMovement();
            return;
        }

        float directionX = Mathf.Sign(xDiff);
        rb.linearVelocity = new Vector2(directionX * moveSpeed, rb.linearVelocity.y);
    }

    private void StopMovement()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public void Dead()
    {
        dead = true;
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
