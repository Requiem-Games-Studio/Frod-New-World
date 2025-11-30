using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossController : MonoBehaviour
{
    public Animator anim;
    GameObject player;
    public bool facePlayer = true;

    float timer;
    public float timeAttack;
    [HideInInspector]
    public bool isInteracting;

    public bool active;
    public bool chase;

    Vector2 moveDirection;
    Rigidbody2D rgb2D;
    public float moveSpeed;
    public SpriteRenderer spriteRenderer;

    public bool rightView;
    public string startCombatAnim;
    public string[] closeAction;
    public float closeDistance = 2.5f;
    public string[] actionAnimations;
    int randomAnimation;
    public string upAttack;

    public LayerMask Suelo;
    public Transform foot;
    public float Radio = 0.1f;
    public bool ground = false;
    public float margenX = 0.2f;
    public bool isFlying;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rgb2D = GetComponent<Rigidbody2D>();
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
                rgb2D.linearVelocity = new Vector2(moveDirection.x, 0) * moveSpeed;
            }



            //Accion del jefe
            if (timer >= timeAttack)
            {
                timer = 0;
                DirectionBoss();
                if (!isInteracting)
                {
                    if (isFlying)
                    {
                        if (Mathf.Abs(player.transform.position.x - transform.position.x) < margenX)
                        {
                            PlayTargetAnimation("Fall", true);
                        }
                        return;
                    }

                    if (player.transform.position.y - transform.position.y > 2f)
                    {
                        PlayTargetAnimation(upAttack, true);
                        return;
                    }

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

            //Vista al jugador
            if (facePlayer)
            {
                if (player.transform.position.x > transform.position.x)
                {
                    transform.localScale = new Vector3(1, 1, 1);  // Mira a la derecha
                    rightView = true;
                }
                if (player.transform.position.x < transform.position.x)
                {
                    transform.localScale = new Vector3(-1, 1, 1); // Mira a la izquierda
                    rightView = false;
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
            //Instantiate(ataque2Right, this.transform.position, Quaternion.identity);
        }
        else
        {
            //Instantiate(ataque2Left, this.transform.position, Quaternion.identity);
        }
    }

    public void DamageEvent()
    {
        if (!isInteracting)
        {
            PlayTargetAnimation("Damage",false);
        }
    }

    //Activa Al jefe
    public void ActivateBoss()
    {
        if (!active)
        {
            PlayTargetAnimation(startCombatAnim, true);
            active = true;
            chase = true;
            anim.SetBool("isWalking",true);
        }
    }

    public void DieEvent()
    {
        spriteRenderer.sortingLayerName = "Front";
        anim.SetBool("Diying",true);
        active = false;
        chase = false;
        PlayTargetAnimation ("Die",true);
    }

    //Funcion para ejecutar animacion
    public void PlayTargetAnimation(string targetAnim, bool isInteracting = false)
    {
        anim.CrossFade(targetAnim, 0.2f);
        anim.SetBool("isInteracting", isInteracting);
    }
}
