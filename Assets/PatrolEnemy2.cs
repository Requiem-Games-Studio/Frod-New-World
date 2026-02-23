using UnityEngine;
using System.Collections;

public class PatrolEnemy2 : MonoBehaviour
{
    // =========================
    // MOVIMIENTO
    // =========================
    [Header("Movimiento")]
    public float speed = 2f;
    public bool startFacingRight = true;
    public bool patrol = true;
    bool dead;

    Rigidbody2D rb;
    float currentSpeed;
    int direction = 1;
    bool flipping = false;

    public Animator anim;
    public EnemyStats enemyStats;

    // =========================
    // DETECCIÓN SUELO / MURO
    // =========================
    [Header("Detección entorno")]
    public Transform groundCheck;
    public float groundCheckDistance = 0.2f;

    public Transform wallCheck;
    public float wallCheckDistance = 0.2f;

    public LayerMask groundLayer;

    [Header("Comportamiento")]
    public float flipDelay = 0.15f;

    // =========================
    // VISIÓN DEL JUGADOR
    // =========================
    [Header("Vision")]
    [SerializeField] private float viewDistance = 5f;
    [SerializeField] private LayerMask visionMask; // Player + Walls + Ground

    [SerializeField] private Transform visionPoint1;
    [SerializeField] private Transform visionPoint2;

    private Transform player;
    private bool isSeeingPlayer;

    private float flipCooldown = 0.3f;
    private float flipTimer;

    // =========================
    // DISPARO
    // =========================
    [Header("Disparo")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint, firePoint2;

    public AudioSource AudioSource;
    public AudioClip disparo;

    // =========================
    // UNITY
    // =========================

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = speed;
        direction = startFacingRight ? 1 : -1;
    }

    void Start()
    {
        anim.SetBool("walk", true);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null || enemyStats.confused || dead) return;

        HandleVision();

        if (flipTimer > 0)
            flipTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (!patrol || enemyStats.confused || dead) return;

        rb.linearVelocity = new Vector2(direction * currentSpeed, rb.linearVelocity.y);

        if (!flipping)
        {
            bool groundAhead = false;
            if (groundCheck != null)
            {
                RaycastHit2D hitGround = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
                groundAhead = hitGround.collider != null;
            }

            bool wallAhead = false;
            if (wallCheck != null)
            {
                Vector2 dir = transform.right * direction;
                RaycastHit2D hitWall = Physics2D.Raycast(wallCheck.position, dir, wallCheckDistance, groundLayer);
                wallAhead = hitWall.collider != null;
            }

            if (!groundAhead || wallAhead)
            {
                StartCoroutine(FlipRoutine());
            }
        }
    }

    // =========================
    // VISIÓN
    // =========================

    void HandleVision()
    {
        int visionIndex = CheckVision();
        Debug.Log("CheckVision = " + visionIndex);
        bool canSee = visionIndex != 0;

        if (canSee && !isSeeingPlayer)
        {
            Debug.Log("Enemigo Attack");
            patrol = false;
            rb.linearVelocity = Vector2.zero;

            anim.SetBool("walk", false);
            anim.SetBool("Shooting", true);

            if (visionIndex == 1)
                anim.Play("Shoot");
            else
                anim.Play("Shoot2");
        }

        if (!canSee && isSeeingPlayer)
        {
            Debug.Log("Enemigo Patrol");
            anim.SetBool("Shooting", false);
            //patrol = true;
            anim.SetBool("walk", true);
        }

        isSeeingPlayer = canSee;
    }

    int CheckVision()
    {
        if (CanSeeFromPoint(visionPoint1))
            return 1;

        if (CanSeeFromPoint(visionPoint2))
            return 2;

        return 0;
    }

    public void ReCheckVision()
    {
        if(CheckVision() == 0)
        {
            Debug.Log("Enemigo Patrol");
            anim.SetBool("Shooting", false);
            patrol = true;
            anim.SetBool("walk", true);
        }
    }

    bool CanSeeFromPoint(Transform point)
    {
        Vector2 dir = direction == 1 ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(point.position, dir, viewDistance, visionMask);

        Debug.DrawRay(point.position, dir * viewDistance, Color.red);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            PlayerController pc = hit.collider.GetComponent<PlayerController>();
            if (pc != null && !pc.isHidden)
                return true;
        }

        return false;
    }
 
    // =========================
    // FLIP
    // =========================


    IEnumerator FlipRoutine()
    {
        flipping = true;

        float oldSpeed = currentSpeed;
        currentSpeed = 0f;
        anim.SetBool("walk", false);

        yield return new WaitForSeconds(flipDelay);

        FlipInstant();

        anim.SetBool("walk", true);
        currentSpeed = oldSpeed;
        flipping = false;
    }

    void FlipInstant()
    {
        direction *= -1;

        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * direction;
        transform.localScale = s;
    }

    public void StartPatrol()
    {
        if (!dead)
        {
            patrol = true;
        }
    }

    public void StopPatrol()
    {
        rb.linearVelocity = Vector3.zero;
        patrol = false;
    }

    // =========================
    // DISPARO (Eventos Animación)
    // =========================

    void Shoot()
    {
        AudioSource.clip = disparo;
        AudioSource.Play();
        var proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        proj.transform.localScale = new Vector3(direction, 1, 1);
    }

    void Shoot2()
    {
        AudioSource.clip = disparo;
        AudioSource.Play();
        var proj = Instantiate(projectilePrefab, firePoint2.position, firePoint2.rotation);
        proj.transform.localScale = new Vector3(direction, 1, 1);
    }

    // =========================
    // MUERTE
    // =========================

    public void DieEvent()
    {
        patrol = false;
        dead = true;
        anim.Play("Die");
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
    }

    // =========================
    // DEBUG
    // =========================

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.magenta;
            Vector3 dir = Vector3.right * Mathf.Sign(transform.localScale.x);
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + dir * wallCheckDistance);
        }
    }
}
