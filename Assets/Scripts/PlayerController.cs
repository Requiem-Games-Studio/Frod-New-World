using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    public Animator animator,weaponAnim;
    [HideInInspector]
    public Weapon weapon;
    public PlayerStats stats;
    private SpriteRenderer spriteRenderer;
    public Transform espadaPivot; // arrastra aquí tu EspadaPivot en el inspector

    public Collider2D normalCollider, balloonCollider, ballCollider, wormCollider;
    public PhysicsMaterial2D ballMaterial, normalMaterial;
    public Transform groundCheck; // Punto en los pies para detectar el suelo
    public LayerMask groundLayer; // Capa del suelo

    
    public float currentSpeed = 5f;
    public float LegsSpeed = 5f;
    public float downSpeed = 1f;
    public float ballSpeed = 6f;
    public float jumpForce;
    public float jumpForceB;
    public float maxJumpTime = 0.2f; // Tiempo máximo de salto variable
    public float coyoteTime = 0.15f; // Tiempo extra después de dejar el suelo
                                     // SuperJump
    public float superJumpForce = 20f;  // más grande que jumpForce
    public float superJumpDuration = 0.4f; // opcional, para que dure más tiempo
    public float smashStunTime = 0.3f; // segundos detenido


    public float dodgeSpeed = 5f;  // Velocidad del dodge
    public float dodgeDuration = 0.5f;
    public float dodgeCooldown = 1f;

    public float normalGravity = 3.5f;
    //public float balloonGravity = 0.8f;
    //public float wormGravity = 1.5f;

    private bool isGrounded;
    private bool canDodge = true;
    public bool isDown;
    private bool isJumping, isInteracting, isBallForm, isAttacking;
    private float jumpTimeCounter;
    private float coyoteTimeCounter;

    //Forms
    bool walk = true;
    private bool isBall = true;
    private enum Form { Normal, Balloon, Ball,Worm }
    private Form currentForm = Form.Ball;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        ChangeForm(Form.Ball);
    }

    void Update()
    {
        isInteracting = animator.GetBool("isInteracting");
        isBallForm = animator.GetBool("isBallForm");
        isDown = animator.GetBool("Down");
        animator.SetBool("isJumping", isJumping);

        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (walk)
        {
            rb.linearVelocity = new Vector2(horizontal * currentSpeed, rb.linearVelocity.y);
        }        

        if (weaponAnim!=null && !stats.isStaggered)
        {
            if (Input.GetMouseButtonDown(0)) // Click izquierdo
            {
                if (vertical > 0)
                {
                    weaponAnim.Play("UpAttack");
                }
                else if (vertical < 0 && !isGrounded)
                {
                    rb.linearVelocity = Vector2.zero;
                    weaponAnim.Play("DownAttack");
                }
                else
                {
                    weaponAnim.Play("Attack");
                }

                if (!isInteracting && !isDown && isGrounded)
                {
                    if (isBallForm)
                    {
                        animator.Play("ActionB");
                    }
                    else
                    {
                        DisableWalk(0.2f);
                        animator.Play("Attack");
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))// Click derecho
            {
                if (!isAttacking && !isInteracting)
                {
                    weaponAnim.SetBool("blocking", true);
                    animator.SetBool("blocking", true);
                    weaponAnim.Play("StartBlock");

                    if (isBallForm)
                    {
                        animator.Play("StartBlockB");
                    }
                    else
                    {
                        animator.Play("StartBlock");
                    }
                }
            }
            if (Input.GetMouseButtonUp(1))
            {
                weaponAnim.SetBool("blocking", false);
                animator.SetBool("blocking", false);
            }
        }
      

        // **Rotar sprite según dirección**
        if (horizontal != 0)
        {
            spriteRenderer.flipX = horizontal < 0;

            // **Flip del pivote de la espada**
            if (weapon != null)
            {
                Vector3 scale = espadaPivot.localScale;
                scale.x = horizontal < 0 ? -1 : 1;
                if (weapon.canFlip)
                {
                    espadaPivot.localScale = scale;
                }
            }
        }

        // **Control de animación Walk**
        animator.SetBool("Walk", horizontal != 0);

        // **Coyote Time**
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // --- Salto Variable ---
        if (Input.GetKeyDown(KeyCode.Space) && coyoteTimeCounter > 0f)
        {
            isJumping = true;
            jumpTimeCounter = maxJumpTime;

            if (isDown && PowerupManager.Instance.HasPower(PowerType.SuperJump))
            {
                animator.Play("SuperJump");
                jumpTimeCounter = superJumpDuration; // opcional: que dure más
            }
            else
            {
                animator.Play(isBallForm ? "JumpB" : "Jump");
            }
        }

        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                if (isDown && PowerupManager.Instance.HasPower(PowerType.SuperJump))
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, superJumpForce);
                }
                else if (isBallForm)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForceB);
                }
                else
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                }

                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }

        // Detectar doble tap abajo Y arriba
        if (PowerupManager.Instance.HasPower(PowerType.Leg))
        {
            if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && !isBall && isGrounded)
            {
                ToggleBallForm();
            }
            else if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && isBall)
            {
                ToggleLegForm();
            }
        }

        // Detectar mantener presionado "DownArrow"
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            currentSpeed = 0;
            animator.SetBool("Down", true);
            animator.Play("StartDown");
            ChangeForm(Form.Worm);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
        {
            animator.SetBool("Down", false);
            animator.Play("FinishDown");
            ToggleCurrentForm();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeForm(Form.Balloon);

        if (PowerupManager.Instance.HasPower(PowerType.Dash) && Input.GetKeyDown(KeyCode.LeftShift) && canDodge)
        {
            StartCoroutine(Dodge());
        }
    }
    void ToggleCurrentForm()
    {
        if (isBall)
        {
            // Entrar en forma Ball
            ChangeForm(Form.Ball);
            animator.SetBool("isBallForm", true);
            currentSpeed = ballSpeed;
            isBall = true;
        }
        else
        {
            // Volver a Normal
            ChangeForm(Form.Normal);
            animator.Play("FromBall"); // animación de levantarse
            animator.SetBool("isBallForm", false);
            currentSpeed = LegsSpeed;
            isBall = false;
        }
    }
    void ToggleBallForm()
    {
        // Entrar en forma Ball
        ChangeForm(Form.Ball);
        //animator.Play("ToBall"); // animación de transformarse
        animator.SetBool("isBallForm", true);
        currentSpeed = ballSpeed;
        isBall = true;
    }
    void ToggleLegForm()
    {
        // Volver a Normal
        ChangeForm(Form.Normal);
        animator.Play("FromBall"); // animación de levantarse
        animator.SetBool("isBallForm", false);
        currentSpeed = LegsSpeed;
        isBall = false;
    }

    IEnumerator Dodge()
    {
        canDodge = false;

        // Reproducir la animación
        animator.SetBool("isInteracting", true);
        if (isBallForm)
        {
            animator.Play("DodgeB");
        }
        else
        {
            animator.Play("Dodge");
        }
        // Desactivar colisión con enemigos
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);

        // Determinar dirección del dodge
        float dodgeDirection = spriteRenderer.flipX ? -1 : 1;

        // Desactivar gravedad para evitar caída
        rb.gravityScale = 0;

        // Aplicar movimiento durante el dodge
        float startTime = Time.time;
        while (Time.time < startTime + dodgeDuration)
        {
            rb.linearVelocity = new Vector2(dodgeDirection * dodgeSpeed, 0); // Velocidad en Y se mantiene en 0
            yield return null;
        }

        rb.linearVelocity = Vector2.zero; // Detener el movimiento después del dodge

        // Reactivar la gravedad
        rb.gravityScale = normalGravity;

        // Reactivar colisión con enemigos después del dodge
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), false);

        // Esperar el cooldown antes de permitir otro dodge
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }

    void ChangeForm(Form newForm)
    {
        //rb.linearVelocity =Vector2.zero;
        walk = false;
        currentForm = newForm;
        rb.sharedMaterial = normalCollider.sharedMaterial;

        normalCollider.enabled = false;
        balloonCollider.enabled = false;
        ballCollider.enabled = false;
        wormCollider.enabled = false;

        switch (newForm)
        {
            case Form.Normal:
                rb.linearVelocity = Vector2.zero;
                normalCollider.enabled = true;
                rb.gravityScale = normalGravity;
                StartCoroutine(EnableWalkDelay(0.3f));
                break;
            case Form.Balloon:
                rb.linearVelocity = Vector2.zero;
                balloonCollider.enabled = true;
                //rb.gravityScale = balloonGravity;
                StartCoroutine(EnableWalkDelay(0.3f));
                break;
            case Form.Worm:
                wormCollider.enabled = true;
                //rb.gravityScale = wormGravity;
                break;
            case Form.Ball:
                //rb.linearVelocity = Vector2.zero;
                ballCollider.enabled = true;
                rb.gravityScale = normalGravity;
                ballCollider.sharedMaterial = ballMaterial;
                StartCoroutine(EnableWalkDelay(0.2f));
                break;
        }
    }
    public void DieEvent()
    {
        rb.linearVelocity = Vector2.zero;
        walk = false;
    }
    public void DisableWalk(float time)
    {
        rb.linearVelocity = Vector2.zero;
        walk = false;
        StartCoroutine(EnableWalkDelay(time));
    }

    IEnumerator EnableWalkDelay(float time)
    {
        yield return new WaitForSeconds(time); // ⏳ medio segundo (ajusta si quieres)
        walk = true;
    }

    private void FixedUpdate()
    {
        if (!animator.GetBool("dodge"))
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
            animator.SetBool("Ground", isGrounded);
        }
    }

    //Animacion de choque
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y < -0.5f)
        {
            if (isDown && PowerupManager.Instance.HasPower(PowerType.SuperJump))
            {
                StartCoroutine(SuperSmashStun());
            }

            if (isBallForm && isJumping)
            {
                StartCoroutine(SmashStun());
            }
        }
    }

    IEnumerator SmashStun()
    {
        // Animación de aplastarse
        animator.Play("Smash");

        // Guardar velocidad y congelar
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(smashStunTime);
        
    }
    IEnumerator SuperSmashStun()
    {
        // Animación de aplastarse
        animator.Play("SuperSmash");

        // Guardar velocidad y congelar
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(smashStunTime);

    }

    public void PlayTargetAnimation(string targetAnim, bool isInteracting = false)
    {
        animator.CrossFade(targetAnim, 0.2f);
        animator.SetBool("isInteracting", isInteracting);
    }
}
