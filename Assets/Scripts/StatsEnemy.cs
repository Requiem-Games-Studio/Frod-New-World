using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsEnemy : MonoBehaviour
{
    public int maxHp;
    public int hp;
    bool dead;
    public GameObject bloodPrefab;
    public SpriteRenderer sprite;
    public Animator anim;
    GameObject controller;

    bool red;
    float timer, timerAlert, timerStun, timerConfused;

    public Canvas canvasEnemy;
    public Slider sliderHealt;
    public GameObject healtBar;
    public GameObject iconAlert, iconStun, iconConfused;
    public bool alert, stun, confused;

    [SerializeField]
    //EnemyAnimatorEvent executionAnimator;

    private void Start()
    {
        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        controller = this.gameObject;

        canvasEnemy.worldCamera = Camera.main;
        hp = maxHp;
        sliderHealt.maxValue = maxHp;
        sliderHealt.value = hp;
        canvasEnemy.enabled = false;
    }

    private void Update()
    {
        if (red)
        {
            timer += Time.deltaTime;
            if (timer >= 0.5f)
            {
                sprite.color = Color.white;
                red = false;
                timer = 0;
            }
        }

        if (alert)
        {
            timerAlert += Time.deltaTime;
            if (timerAlert >= 2f)
            {
                iconAlert.SetActive(false);
                alert = false;
                timerAlert = 0;
            }
        }
        if (stun)
        {
            timerStun += Time.deltaTime;
            if (timerStun >= 2f)
            {
                iconStun.SetActive(false);
                stun = false;
                timerStun = 0;
            }
        }
        if (confused)
        {
            timerConfused += Time.deltaTime;
            if (timerConfused >= 2f)
            {
                iconConfused.SetActive(false);
                confused = false;
                timerConfused = 0;
            }
        }
    }

    public void Damage(int damage)
    {
        if (!dead)
        {
            canvasEnemy.enabled = true;
            sprite.color = Color.red;
            red = true;
            anim.Play("Damage");
            hp -= damage;
            sliderHealt.value = hp;
            Instantiate(bloodPrefab, transform.position, transform.rotation);
            if (hp <= 0)
            {
                dead = true;
                if (anim != null)
                {
                    anim.SetBool("Die", true);
                    anim.Play("Dead");
                    controller.SendMessage("Dead");
                    canvasEnemy.enabled = false;
                }
            }
        }
    }

    public void AlertState()
    {
        if (!stun && !confused && !dead)
        {
            alert = true;
            iconAlert.SetActive(true);
        }
    }
    public void StunState()
    {
        if (!dead)
        {
            stun = true;
            iconStun.SetActive(true);
        }
    }
    public void ConfusedState()
    {
        if (!dead)
        {
            confused = true;
            iconConfused.SetActive(true);
            anim.SetBool("confused", true);
            anim.Play("Confused");
            //if (executionAnimator != null)
            //{
            //executionAnimator.MovePoliStop();
            //}
        }
    }

    public void FoxedState()
    {
        if (!dead)
        {
            confused = true;
            iconConfused.SetActive(true);
            anim.Play("Confused");
        }
    }

    public void NormalState()
    {
        confused = false;
        anim.SetBool("confused", false);
    }
}
