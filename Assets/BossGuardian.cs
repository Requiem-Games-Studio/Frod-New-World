using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossGuardian : MonoBehaviour
{
    public Animator anim;
    public EnemyStats stats;
    GameObject player;
    public bool facePlayer = true;

    float timer;
    public float timeAttack;
    [HideInInspector]
    public bool isInteracting;

    public bool active;
    public bool chase;

    bool impaled;

    Vector2 moveDirection;
    Rigidbody2D rgb2D;
    public float moveSpeed;
    public SpriteRenderer spriteRenderer;

    public bool rightView;
    public string startCombatAnim;
    public string[] closeAction;
    public float closeDistance = 2.5f;
    public string[] actionAnimations;
    public string[] actionAnimations2;
    int randomAnimation;

    public LayerMask Suelo;
    public Transform foot;
    public float Radio = 0.1f;
    public bool ground = false;

    [Header("Impulse Settings")]
    public float dashForce = 10f;
    public float jumpForce = 30f;
    public float speedBoost = 2f; // multiplicador de velocidad temporal

    float originalSpeed, originalGravity;
    public AudioSource audioSource;
    public AudioClip audioClip;

    public Transform swordAttackPoint;
    public GameObject ataque2Right, ataque2Left;

    public GameObject barreras;

    float startScale;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rgb2D = GetComponent<Rigidbody2D>();

        startScale = transform.localScale.x;

        originalSpeed = moveSpeed;
        originalGravity = rgb2D.gravityScale;
    }

    private void Update()
    {
        if (active)
        {
            ground = Physics2D.OverlapCircle(foot.position, Radio, Suelo);

            timer += Time.deltaTime;
            isInteracting = anim.GetBool("isInteracting");
            anim.SetBool("ground", ground);
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (chase)
            {
                Vector3 direction = (player.transform.position - transform.position).normalized;
                moveDirection = direction;
                rgb2D.linearVelocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y);
            }



            //Accion del jefe
            if (timer >= timeAttack && ground)
            {
                timer = 0;
                DirectionBoss();
                if (!isInteracting)
                {                  
                    if(stats.currentHp <= stats.maxHp * 0.5f)
                    {

                        //Ejecutar una de las animaciones asignadas desde el inspector
                        randomAnimation = Random.Range(0, actionAnimations2.Length);
                        PlayTargetAnimation(actionAnimations2[randomAnimation], true);
                        return;
                    }
                    else
                    {
                        if (distanceToPlayer < closeDistance)
                        {
                            randomAnimation = Random.Range(0, closeAction.Length);
                            PlayTargetAnimation(closeAction[randomAnimation], true);
                            return;
                        }

                        //Ejecutar una de las animaciones asignadas desde el inspector
                        randomAnimation = Random.Range(0, actionAnimations.Length);
                        PlayTargetAnimation(actionAnimations[randomAnimation], true);
                    }
                }
            }

            //Vista al jugador
            if (facePlayer)
            {
                if (player.transform.position.x > transform.position.x)
                {
                    transform.localScale = new Vector3(-startScale, startScale, 1);  // Mira a la derecha
                    rightView = false;
                }
                if (player.transform.position.x < transform.position.x)
                {
                    transform.localScale = new Vector3(startScale, startScale, 1); // Mira a la izquierda
                    rightView = true;
                }
            }
        }
    }

    //Checar direccion y posicion del jefe antes de actuar/atacar
    void DirectionBoss()
    {
        if (rightView)
        {

        }
        else
        {

        }
    }

    public void Attack2Efect()
    {
        if (rightView)
        {
            Instantiate(ataque2Right, swordAttackPoint.position, Quaternion.identity);
        }
        else
        {
            Instantiate(ataque2Left, swordAttackPoint.position, Quaternion.identity);
        }
    }

    public void DamageEvent()
    {
        if (!isInteracting)
        {
            PlayTargetAnimation("Damage", false);
        }
    }

    //Activa Al jefe
    public void ActivateBoss()
    {
        if (!active)
        {
            PlayTargetAnimation(startCombatAnim, true);
            active = true;
            anim.SetBool("isWalking", true);
        }
    }

    public void DieEvent()
    {
        anim.SetBool("Diying", true);
        active = false;
        chase = false;
        barreras.SetActive(false);
        if (impaled)
        {
            PlayTargetAnimation("Impaled", true);
        }
        else
        {
            PlayTargetAnimation("Die", true);
        }
    }

    public void ImpaledEvent()
    {
        impaled = true;
        rgb2D.linearVelocity = Vector2.zero;
        rgb2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }

    //Funcion para ejecutar animacion
    public void PlayTargetAnimation(string targetAnim, bool isInteracting = false)
    {
        anim.CrossFade(targetAnim, 0.2f);
        anim.SetBool("isInteracting", isInteracting);
    }

    // Congelar enemigo (que no se mueva)
    public void FreezePosition()
    {
        rgb2D.linearVelocity = Vector2.zero;
        rgb2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }

    // Soltar congelamiento
    public void UnfreezePosition()
    {
        rgb2D.constraints = RigidbodyConstraints2D.FreezeRotation; // vuelve a normal
    }

    // Impulso hacia el lado contrario del que está mirando
    public void BackDash()
    {
        facePlayer = false;
        rgb2D.linearVelocity = Vector2.zero;
        if (rightView)
        {
            rgb2D.AddForce(new Vector2(1 * dashForce, 0), ForceMode2D.Impulse);
        }
        else
        {
            rgb2D.AddForce(new Vector2(-1 * dashForce, 0), ForceMode2D.Impulse);
        }
    }

    // Impulso hacia el frente (en la dirección que está mirando)
    public void ForwardDash()
    {
        facePlayer = false;
        rgb2D.linearVelocity = Vector2.zero;
        if (rightView)
        {
            rgb2D.AddForce(new Vector2(-1 * dashForce, 0), ForceMode2D.Impulse);
        }
        else
        {
            rgb2D.AddForce(new Vector2(1 * dashForce, 0), ForceMode2D.Impulse);
        }
    }

    //Salto
    public void Jump()
    {
        //rgb2D.linearVelocity = Vector2.zero;
        rgb2D.gravityScale = -10;
        rgb2D.AddForce(new Vector2(0, 2 * jumpForce), ForceMode2D.Impulse);
    }

    // Aumentar velocidad del boss temporalmente
    public void BoostSpeed()
    {
        moveSpeed *= speedBoost;
    }

    // Resetear fisica y velocidad
    public void ResetPhysics()
    {
        Debug.Log("ResetPhysics");
        rgb2D.gravityScale = originalGravity;
        facePlayer = true;
        //rgb2D.linearVelocity = Vector2.zero;  
        rgb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        moveSpeed = originalSpeed;
    }

    public void GravityZero()
    {
        rgb2D.gravityScale = 0;
    }

    public void DiePhysics()
    {
        rgb2D.linearVelocity = Vector2.zero;
        rgb2D.gravityScale = originalGravity;
    }

    public void ChaseOn()
    {
        facePlayer = true;
        //rgb2D.linearVelocity = Vector2.zero;
        chase = true;
    }
    public void ChaseOff()
    {
        //rgb2D.linearVelocity = Vector2.zero;
        chase = false;
    }

    public void PlayClip()
    {
        audioSource.pitch = Random.Range(0.95f, 1.08f);
        audioSource.PlayOneShot(audioClip);
    }
}
