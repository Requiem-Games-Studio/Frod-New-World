using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeakEnemyStats : MonoBehaviour
{
    public float maxHp;
    public float currentHp;
    bool dead;
    public GameObject bloodPrefab;
    public SpriteRenderer sprite;
    public Animator anim;
    GameObject controller;

    float timerAlert, timerStun, timerConfused;

    public GameObject canvasEnemy;
    public Slider sliderHealt;
    public GameObject healtBar;
    public Slider delayedBar;
    public float delayBeforeDrop = 0.3f; // Tiempo que espera antes de bajar
    public float dropSpeed = 0.5f;       // Velocidad de bajada
    private Coroutine delayedRoutine;

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

        canvasEnemy.GetComponent<Canvas>().worldCamera = Camera.main;
        currentHp = maxHp;
        //sliderHealt.maxValue = maxHp;
        //sliderHealt.value = currentHp;
        //delayedBar.maxValue = currentHp;
        canvasEnemy.SetActive(false);
    }

    public void Damage(float damage)
    {
        if (!dead)
        {
            canvasEnemy.SetActive(true);
            anim.Play("Damage");
            currentHp -= damage;
            UpdateHealthBar();
            Instantiate(bloodPrefab, transform.position, transform.rotation);
            if (currentHp <= 0)
            {
                dead = true;
                if (anim != null)
                {
                    anim.SetBool("Die", true);
                    anim.Play("Dead");
                    controller.SendMessage("Dead");
                    canvasEnemy.SetActive(false);
                }
            }
        }
    }
    private void UpdateHealthBar()
    {
        if (sliderHealt != null)
        {
            float newValue = (float)currentHp / maxHp;
            sliderHealt.value = newValue;
            // Iniciar la corrutina cada vez que cambie la vida
            if (delayedRoutine != null)
                StopCoroutine(delayedRoutine);

            delayedRoutine = StartCoroutine(UpdateDelayedBar(newValue));
        }
    }
    private IEnumerator UpdateDelayedBar(float targetValue)
    {
        // Esperar antes de empezar a bajar
        yield return new WaitForSeconds(delayBeforeDrop);

        // Bajar lentamente hasta el valor real
        while (delayedBar.value > targetValue)
        {
            delayedBar.value -= dropSpeed * Time.deltaTime;
            yield return null;
        }

        delayedBar.value = targetValue; // Asegurar que llegue exacto
    }

    public void ActivateStatus(GameObject icon, float duration)
    {
        canvasEnemy.SetActive(true);
        StartCoroutine(StatusRoutine(icon, duration));
    }

    IEnumerator StatusRoutine(GameObject icon, float duration)
    {
        icon.SetActive(true);
        yield return new WaitForSeconds(duration);
        icon.SetActive(false);
    }

    public void AlertState()
    {
        if (!stun && !confused && !dead)
        {
            ActivateStatus(iconAlert, timerAlert);
        }
    }
    public void StunState()
    {
        if (!dead)
        {
            ActivateStatus(iconStun, timerStun);
        }
    }
    public void ConfusedState()
    {
        if (!dead)
        {
            confused = true;
            ActivateStatus(iconConfused,timerConfused);
            anim.SetBool("confused", true);
            anim.Play("Confused");
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
        iconConfused.SetActive(false);
        anim.SetBool("confused", false);
    }

    public void BreakObject(float postureDamage)
    {
        //currentPosture -= postureDamage;
        //CheckPostureBreak();
        Damage(postureDamage);
    }
}
