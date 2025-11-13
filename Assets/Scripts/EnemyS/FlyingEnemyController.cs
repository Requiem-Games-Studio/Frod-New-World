using UnityEngine;

public class FlyingEnemyController : MonoBehaviour
{
    public enum State { Patrol, Dive, Recover }

    [Header("References")]
    public Rigidbody2D rb;
    public Transform player;
    public LayerMask groundLayer;
    public Collider2D hitbox; // collider used to hit the player (set IsTrigger if using OnTriggerEnter2D)

    [Header("Hover / Ground")]
    public float hoverHeight = 2.5f;      // altura deseada sobre el suelo
    public float groundCheckDistance = 10f;
    public float verticalSmoothTime = 0.12f;

    [Header("Patrol (aleatorio)")]
    public float patrolSpeed = 1.8f;
    public float patrolChangeIntervalMin = 1.0f;
    public float patrolChangeIntervalMax = 3.0f;
    public float patrolHorizontalRange = 3f; // distancia máxima del objetivo horizontal

    [Header("Detección y picado")]
    public float detectRangeVertical = 6f;     // cuanto tiene que estar el jugador bajo el enemigo para considerar tirarse
    public float detectRangeHorizontal = 1.5f; // margen horizontal para ver si el jugador está "debajo"
    public float diveSpeed = 12f;
    public float diveDuration = 0.5f;
    public float recoverUpImpulse = 6f;

    [Header("Misc")]
    public float maxHorizontalSpeed = 4f;

    // estado interno
    private State state = State.Patrol;
    private float patrolTimer = 0f;
    private Vector2 patrolTargetOffset = Vector2.zero;
    private float verticalVelocity = 0f;
    private float diveTimer = 0f;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        ChooseNewPatrolTarget();
        patrolTimer = Random.Range(patrolChangeIntervalMin, patrolChangeIntervalMax);
    }

    private void Update()
    {
        // lógica de detección solo en Update (no provoca cambios de física)
        switch (state)
        {
            case State.Patrol:
                TryDetectPlayerBelow();
                break;
            case State.Dive:
                // temporizador para terminar el dive si no colisionó
                break;
            case State.Recover:
                // nada especial en Update
                break;
        }

        // temporizador de cambio de objetivo de patrulla
        if (state == State.Patrol)
        {
            patrolTimer -= Time.deltaTime;
            if (patrolTimer <= 0f)
            {
                ChooseNewPatrolTarget();
                patrolTimer = Random.Range(patrolChangeIntervalMin, patrolChangeIntervalMax);
            }
        }
    }

    private void FixedUpdate()
    {
        // mantén la altura sobre el suelo (usa raycast hacia abajo para encontrar Y del suelo)
        float groundY = GetGroundYUnder(transform.position);
        float targetY = groundY + hoverHeight;

        // control vertical suave
        float newY = Mathf.SmoothDamp(transform.position.y, targetY, ref verticalVelocity, verticalSmoothTime, Mathf.Infinity, Time.fixedDeltaTime);
        float verticalMove = (newY - transform.position.y) / Time.fixedDeltaTime;

        // control horizontal según estado
        float horizontalMove = 0f;
        switch (state)
        {
            case State.Patrol:
                // target x = posición actual + offset
                float targetX = transform.position.x + patrolTargetOffset.x;
                float desiredDeltaX = targetX - transform.position.x;
                // movimiento suave hacia objetivo horizontal
                horizontalMove = Mathf.Clamp(desiredDeltaX, -maxHorizontalSpeed * Time.fixedDeltaTime, maxHorizontalSpeed * Time.fixedDeltaTime) / Time.fixedDeltaTime;
                // aplicar velocidad combinada (mantener gravedad controlada por el script)
                rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, horizontalMove, 0.2f), verticalMove);
                break;

            case State.Dive:
                // en dive establecemos una velocidad hacia abajo y hacia X del jugador
                diveTimer += Time.fixedDeltaTime;
                if (player != null)
                {
                    Vector2 dir = (player.position - transform.position).normalized;
                    // mayor componente vertical hacia abajo (asegurar que se mueve rápido hacia el jugador)
                    Vector2 diveVel = new Vector2(dir.x * diveSpeed, dir.y * diveSpeed);
                    rb.linearVelocity = diveVel;
                }
                else
                {
                    rb.linearVelocity = new Vector2(0f, -diveSpeed);
                }
                // si el dive excede su duración, pasa a recover
                if (diveTimer >= diveDuration)
                {
                    EnterRecover();
                }
                break;

            case State.Recover:
                // recuperamos: fijar velocidad vertical hacia arriba hasta alcanzar hoverHeight
                // usamos AddForce para dar impulso si hace falta, o lerp la velocidad
                float toTargetY = (targetY - transform.position.y);
                float upVel = Mathf.Clamp(toTargetY * 5f, -maxHorizontalSpeed, Mathf.Abs(recoverUpImpulse));
                rb.linearVelocity = new Vector2(0f, upVel);
                // cuando ya esté cerca de la altura, volver a patrullar
                if (Mathf.Abs(transform.position.y - targetY) < 0.15f)
                {
                    ExitRecover();
                }
                break;
        }
    }

    private float GetGroundYUnder(Vector2 origin)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
        if (hit.collider != null)
        {
            return hit.point.y;
        }
        // fallback: si no detecta suelo, toma origin.y - hoverHeight (evita errores)
        return origin.y - hoverHeight;
    }

    private void ChooseNewPatrolTarget()
    {
        float offsetX = Random.Range(-patrolHorizontalRange, patrolHorizontalRange);
        patrolTargetOffset = new Vector2(offsetX, 0f);
    }

    private void TryDetectPlayerBelow()
    {
        if (player == null) return;

        // Está el jugador debajo lo suficiente (vertical) y dentro del margen horizontal?
        float dy = transform.position.y - player.position.y;
        float dx = Mathf.Abs(transform.position.x - player.position.x);

        if (dy > 0f && dy <= detectRangeVertical && dx <= detectRangeHorizontal)
        {
            EnterDive();
        }
    }

    private void EnterDive()
    {
        state = State.Dive;
        diveTimer = 0f;
        // opcional: desactivar gravedad si quieres controlar todo por velocidad
        rb.gravityScale = 0f;
        // opcional: cambiar animador aquí
        // animator.SetTrigger("Dive");
    }

    private void EnterRecover()
    {
        state = State.Recover;
        rb.gravityScale = 0f;
        // dar un pequeño impulso hacia arriba para la recuperación si quieres
        rb.linearVelocity = new Vector2(0f, recoverUpImpulse * 0.5f);
    }

    private void ExitRecover()
    {
        state = State.Patrol;
        rb.gravityScale = 0f; // seguimos controlando la Y por SmoothDamp
        ChooseNewPatrolTarget();
        patrolTimer = Random.Range(patrolChangeIntervalMin, patrolChangeIntervalMax);
    }

    // Manejo de colisiones con el jugador: asume que el hitbox está configurado como Trigger y el player tiene tag "Player"
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Aquí aplicar daño al jugador (llama su script de salud)
            // var health = other.GetComponent<PlayerHealth>(); if (health) health.TakeDamage(damageAmount);

            // una vez golpeó, pasa a recover
            EnterRecover();
        }
    }

    // Método público para forzar volver a patrullar (útil desde Animator Events)
    public void ForcePatrol()
    {
        state = State.Patrol;
        rb.gravityScale = 0f;
        ChooseNewPatrolTarget();
    }
}
