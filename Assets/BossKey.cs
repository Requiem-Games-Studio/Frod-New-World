using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;
using System;

public class BossKey : MonoBehaviour
{
    public Animator anim;
    GameObject player;
    Vector3 target;
    public bool facePlayer = true;
    public bool isRightView = true;

    [HideInInspector]
    public bool isInteracting;

    public bool active;
    public bool chase;

    public float moveSpeed;
    public SpriteRenderer spriteRenderer;

    public bool rightView;
    public RectTransform canvasTransform;
    public string startCombatAnim;
    public string[] closeAction;
    public float closeDistance = 2.5f;
    int randomAnimation;

    public BossActivator activator;
    public EnemyStats enemyStats;
    float bScale;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        bScale = transform.localScale.x;
    }

    private void Update()
    {
        if (active)
        {
            isInteracting = anim.GetBool("isInteracting");
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (!isInteracting)
            {
                if (chase)
                {
                    target = player.transform.position;

                    float fixedSpeed = moveSpeed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, target, fixedSpeed);
                }

                if (distanceToPlayer < closeDistance)
                {
                    randomAnimation = UnityEngine.Random.Range(0, closeAction.Length);
                    PlayTargetAnimation(closeAction[randomAnimation], true);
                    return;
                }
            }            

            //Vista al jugador
            if (facePlayer)
            {
                if (isRightView)
                {
                    if (player.transform.position.x > transform.position.x)
                    {
                        transform.localScale = new Vector3(-bScale, bScale, bScale);  // Mira a la derecha
                        canvasTransform.localScale = new Vector3(-1, 1, 1);
                        rightView = true;
                    }
                    if (player.transform.position.x < transform.position.x)
                    {
                        transform.localScale = new Vector3(bScale, bScale, bScale); // Mira a la izquierda
                        canvasTransform.localScale = new Vector3(1, 1, 1);
                        rightView = false;
                    }
                }
                else
                {
                    if (player.transform.position.x > transform.position.x)
                    {
                        transform.localScale = new Vector3(-1, 1, 1);  // Mira a la derecha
                        rightView = false;
                    }
                    if (player.transform.position.x < transform.position.x)
                    {
                        transform.localScale = new Vector3(1, 1, 1); // Mira a la izquierda
                        rightView = true;
                    }
                }
            }
        }
    }


    public void DamageEvent()
    {
        if (!isInteracting)
        {
            PlayTargetAnimation("Damage", true);
        }


        if (enemyStats.currentHp <= enemyStats.maxHp * 0.6f)
        {
            moveSpeed = moveSpeed * 1.2f;
            Debug.Log("Nuevo MoveSpeed" + moveSpeed);
            
            if (enemyStats.currentHp <= enemyStats.maxHp * 0.4f)
            {
                moveSpeed = moveSpeed * 1.4f;
                Debug.Log("Nuevo MoveSpeed" + moveSpeed);
            }
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
            anim.SetBool("isChasing", true);
        }
    }

    public void DieEvent()
    {
        if (active)
        {
            spriteRenderer.sortingLayerName = "Front";
            anim.SetBool("dead", true);
            active = false;
            activator.Desactivate();
            chase = false;
            PlayTargetAnimation("Die", true);
        }
    }

    //Funcion para ejecutar animacion
    public void PlayTargetAnimation(string targetAnim, bool isInteracting = false)
    {
        anim.CrossFade(targetAnim, 0.2f);
        anim.SetBool("isInteracting", isInteracting);
    }
}
