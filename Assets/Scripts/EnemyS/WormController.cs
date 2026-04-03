using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormController : MonoBehaviour
{
    public float visionRadius;
    public float attackRadius;
    public float speed,forceUp;
    float normalSpeed,ySpeed;

    public GameObject player;
    PlayerController playerController;
    Vector3 initialPosition;

    public Animator anim;
    public Rigidbody2D rb2d;
    public CapsuleCollider2D cC2d;

    public LayerMask suelo;
    public Transform pie;
    public float radio = 0.1f;
    public bool ground = false;
    public bool sucking = false;
    bool isInteracting;
    bool full;
    public bool dead;
    //public GameObject myPrefab;

    public SpriteRenderer sprite;
    public float timer;

    public bool posibleSuck;
    public AudioSource idleWorm;

    // Variables nuevas
    private bool isAttacking = false;
    private Vector2 attackDirection;
    Vector3 dir;
    public CircleCollider2D fullBody;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        normalSpeed = speed;
        ySpeed = 0;
    }

    void Update()
    {       
        Grounded();
        Suck();

        initialPosition = transform.position;
        Vector3 target = initialPosition;
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            player.transform.position - transform.position,
            visionRadius,
            1 << LayerMask.NameToLayer("Player"));

        Vector3 forward = transform.TransformDirection(player.transform.position - transform.position);
        Debug.DrawRay(transform.position, forward, Color.red);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player") && !playerController.isHidden)
            {
                target = player.transform.position;
            }
        }

        float distance = Vector3.Distance(target, transform.position);
        dir = (target - transform.position);

        if (!full && !dead)
        {
            isInteracting = anim.GetBool("isInteracting");

            if (target != initialPosition && distance < attackRadius && !sucking && !isInteracting)
            {
                //anim.Play("PreAttack");
                if (ground)
                {
                    PlayTargetAnimation("Attack", true);
                }
                Vector2 movement = new Vector2(attackDirection.x, attackDirection.y * ySpeed);
                rb2d.MovePosition(rb2d.position + movement * speed * Time.deltaTime);
                anim.SetFloat("movX", attackDirection.x);
            }
            else
            {
                if (isAttacking)
                {
                    Vector2 movement = new Vector2(attackDirection.x, attackDirection.y * ySpeed);
                    rb2d.MovePosition(rb2d.position + movement * speed * Time.deltaTime);
                    anim.SetFloat("movX", attackDirection.x);
                }
                else
                {
                    Vector2 movement = new Vector2(dir.x, -6);
                    rb2d.MovePosition(rb2d.position + movement * speed * Time.deltaTime);
                    anim.SetFloat("movX", dir.x);
                }                             
                anim.SetBool("seguir", true);
            }

            if (target == initialPosition && distance < 0.5f)
            {
                transform.position = initialPosition;
                anim.SetBool("seguir", false);
            }
        }               

        Debug.DrawLine(transform.position, target, Color.green);

        // direccion 

        if (dir.x < 0 && !sucking && !full && !dead)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void OnAttack()
    {
        Debug.Log("ON Attack");
        isAttacking = true;
        attackDirection = dir;
        posibleSuck = true;
        ySpeed = forceUp;
        speed *= 4;
    }
    public void OffAttack()
    {
        Debug.Log("Off Attack");
        isAttacking = false;
        posibleSuck = false;
        ySpeed = 0;
        speed = normalSpeed;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, visionRadius);
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    void Grounded()
    {
        ground = Physics2D.OverlapCircle(pie.position, radio, suelo);

        if (ground)
        {
            anim.SetBool("Ground", true);
        }
        else
        {
            anim.SetBool("Ground", false);
        }
    }

    void Suck()
    {
        if (sucking && !dead)
        {
            cC2d.isTrigger = true;
            rb2d.bodyType = RigidbodyType2D.Kinematic;
            this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, 0);
            transform.parent = player.transform;
            timer += Time.deltaTime;

            if (timer > 8)
            {
                idleWorm.Stop();
                full = true;
                PlayTargetAnimation("Full", true);
                fullBody.enabled = full;

                StopSuck();
            }
        }
    }

    public void Dead()
    {
        dead = true;
        idleWorm.Stop();
        StopSuck();
    }

    public void StopSuck()
    {
        Debug.Log("StopSuck");
        OffAttack();
        posibleSuck = false;
        transform.parent = null;
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        cC2d.isTrigger = false;
        timer = 0;
        anim.SetBool("suck", false);
        sucking = false;
    }

    void MakeDamageToPlayer()
    {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        playerStats.Damage(7, 5, gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Debug.Log("collision");
            if (posibleSuck)
            {
                //player.SendMessage("SlowDeath");
                anim.SetBool("suck", true);
                sucking = true;
                //sprite.sortingOrder = sortingOrder;
            }
        }
    }

    public void PlayTargetAnimation(string targetAnim, bool isInteracting = false)
    {
        anim.CrossFade(targetAnim, 0.2f);
        anim.SetBool("isInteracting", isInteracting);
    }
}
