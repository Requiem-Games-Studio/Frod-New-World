using UnityEngine;
using System.Collections;

public class SniperEnemy : MonoBehaviour
{
    // =========================
    // MOVIMIENTO
    // =========================
    [Header("Movimiento")]
    public bool startFacingRight = true;
    bool dead;

    Rigidbody2D rb;
    int direction = 1;

    public Animator anim;
    public EnemyStats enemyStats;

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
        if (player == null || enemyStats.confused || enemyStats.isStaggered || dead) return;

        HandleVision();

    }

    // =========================
    // VISIÓN
    // =========================

    void HandleVision()
    {
        int visionIndex = CheckVision();
        bool canSee = visionIndex != 0;

        if (canSee && !isSeeingPlayer)
        {
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
        if (CheckVision() == 0)
        {
            anim.SetBool("Shooting", false);
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


   

    void FlipInstant()
    {
        direction *= -1;

        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * direction;
        transform.localScale = s;
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
        dead = true;
        anim.Play("Die");
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
    }
}
