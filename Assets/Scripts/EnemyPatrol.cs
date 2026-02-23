using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public enum MovementType { Horizontal, Vertical }

    [Header("Movement Settings")]
    [SerializeField] private MovementType movementType = MovementType.Horizontal;
    [SerializeField] private float speed = 2f;

    [Header("Wall Detection")]
    [SerializeField] private float detectionDistance = 0.5f;
    [SerializeField] private LayerMask wallLayer;

    private Vector2 direction;
    private SpriteRenderer spriteRenderer;

    bool dead;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (movementType == MovementType.Horizontal)
            direction = Vector2.right;
        else
            direction = Vector2.up;

        UpdateSpriteDirection();
    }

    private void Update()
    {
        if(!dead)
        {
            Move();
            DetectWall();
        }
    }

    private void Move()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void DetectWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionDistance, wallLayer);

        if (hit.collider != null)
        {
            ChangeDirection();
        }
    }

    private void ChangeDirection()
    {
        direction *= -1;
        UpdateSpriteDirection();
    }

    private void UpdateSpriteDirection()
    {
        if (movementType == MovementType.Horizontal)
        {
            spriteRenderer.flipX = direction.x < 0;
        }
        else
        {
            spriteRenderer.flipX = direction.y < 0;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 dir;

        if (Application.isPlaying)
            dir = direction;
        else
            dir = movementType == MovementType.Horizontal ? Vector3.right : Vector3.up;

        Gizmos.DrawLine(transform.position, transform.position + dir * detectionDistance);
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
