using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyStats : MonoBehaviour
{
    [Header("Health")]
    public float maxHp = 100f;
    public float currentHp;

    [Header("Posture / Resistance")]
    public float maxPosture = 100f;
    public float currentPosture;
    public float postureRecoveryRate = 8f; // Recuperación por segundo
    public float postureBreakTime = 2f;
    public bool isStaggered = false;

    public Animator anim;
    private bool isAlive = true;
    public GameObject enemyBehavior;

    public Slider healthBar;
    public Slider delayedBar;
    public float delayBeforeDrop = 0.3f; // Tiempo que espera antes de bajar
    public float dropSpeed = 0.5f;       // Velocidad de bajada
    private Coroutine delayedRoutine;
    public Slider postureBar;
    public GameObject canvas;
    public GameObject iconAlert, iconStun, iconConfused;
    public bool alert, stun, confused;

    public bool isBoss;
    bool isBlocking;
    public AudioSource audioSource;
    public AudioClip blockClip;
    public GameObject parryParticle,hitParticle,deadParticle;

    public FlashSprite flashSprite;


    void Start()
    {
        canvas.GetComponent<Canvas>().worldCamera = Camera.main;
        currentHp = maxHp;
        currentPosture = maxPosture;
        canvas.SetActive(false);
    }

    void Update()
    {
        RecoverPosture();
    }

    // Recibir daño
    public void Damage(float damage, float postureDamage, bool isHeavyAttack = false)
    {
        canvas.SetActive(true);
        
        if (!isAlive) return;

        if (isBlocking)
        {
            ReducePosture(10f);
            if (currentPosture >= 10 && !isStaggered)
            {
                anim.Play("ParryAttack");
                Instantiate(parryParticle, gameObject.transform);
                GameFeelManager.Instance.DoParryImpact();
            }
            else
            {
                audioSource.clip = blockClip;
                audioSource.Play();
                anim.Play("BlockBreak");
            }
            return;
        }

        if (isStaggered)
        {
            PlayTargetAnimation("Damage",true);
            currentHp -= damage * 2;
            UpdateHealthBar();
        }
        else
        {
            if (!isBoss)
            {
                PlayTargetAnimation("Damage", true);
            }
            currentHp -= damage;
            UpdateHealthBar();
            currentPosture -= postureDamage * (isHeavyAttack ? 2f : 1f);
            CheckPostureBreak();
        }

        if (currentHp <= 0)
        {
            GameFeelManager.Instance.DoImpactToKill();
            Instantiate(deadParticle, transform.position, transform.rotation);
            flashSprite.Flash();
            Die();
            return;
        }
        GameFeelManager.Instance.DoImpact();
        Instantiate(hitParticle, transform.position, transform.rotation);
        flashSprite.Flash();
    }

    public void BreakObject(float postureDamage)
    {
        currentPosture -= postureDamage;
        CheckPostureBreak();
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            float newValue = (float)currentHp / maxHp;
            healthBar.value = newValue;
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
    public void ActivateBossBar()
    {
        healthBar.enabled = true;
    }

    // Reducir solo postura (por ejemplo, al recibir un parry)
    public void ReducePosture(float amount)
    {
        if (!isAlive || isStaggered) return;

        currentPosture -= amount;
        UpdatePostureBar();
        CheckPostureBreak();
    }

    // Recuperación de postura
    void RecoverPosture()
    {
        if (!isStaggered && currentPosture < maxPosture)
        {
            currentPosture += postureRecoveryRate * Time.deltaTime;
            currentPosture = Mathf.Min(currentPosture, maxPosture);
            UpdatePostureBar();
        }
    }
    private void UpdatePostureBar()
    {
        currentPosture = Mathf.Min(currentPosture, maxPosture);
        postureBar.value = currentPosture / maxPosture;
    }

    // Llamado desde la aniamcion
    public void SetBlock()
    {
        isBlocking = true;
    }

    public void StopBlock()
    {
        isBlocking = false;
    }

    // Revisar si se rompe la postura
    void CheckPostureBreak()
    {
        if (currentPosture <= 0 && !isStaggered)
        {
            StartCoroutine(Stagger());
        }
    }
    public void ConfusedState()
    {
        if (isAlive)
        {
            confused = true;
            ActivateStatus(iconConfused,4);
            anim.SetBool("confused", true);
            anim.Play("Confused");
        }
    }

    public void NormalState()
    {
        confused = false;
        anim.SetBool("confused", false);
    }

    public void ActivateStatus(GameObject icon, float duration)
    {
        canvas.SetActive(true);
        StartCoroutine(StatusRoutine(icon, duration));
    }

    IEnumerator StatusRoutine(GameObject icon, float duration)
    {
        icon.SetActive(true);
        yield return new WaitForSeconds(duration);
        icon.SetActive(false);
    }

    // Tambaleo
    System.Collections.IEnumerator Stagger()
    {
        isStaggered = true;
        PlayTargetAnimation("Stagger",true);
        ActivateStatus(iconStun, 2f);
        Debug.Log("¡Enemigo tambaleado!");
        yield return new WaitForSeconds(postureBreakTime);
        currentPosture = maxPosture * 0.6f;
        isStaggered = false;
    }

    // Muerte
    void Die()
    {
        isAlive = false;
        enemyBehavior.SendMessage("DieEvent");
        anim.SetBool("dead", true);
        canvas.SetActive(false);
        // Aquí puedes desactivar IA, colisiones, etc.
    }

    public void PlayTargetAnimation(string targetAnim, bool isInteracting = false)
    {
        anim.CrossFade(targetAnim, 0.2f);
        anim.SetBool("isInteracting", isInteracting);
    }
}