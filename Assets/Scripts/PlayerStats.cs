using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private int healPotionCount = 0;
    public int healAmount = 25;
    public Animator animator;

    public Slider healthBar; // Referencia al Slider de vida
    public Slider postureBar;

    bool die;

    public PlayerController playerController;

    [Header("Parry Settings")]
    public bool isBlocking = false;
    public bool isPerfectBlock = false;
    public AudioSource audioBlock;
    public GameObject parryParticle;

    [Header("Posture / Resistance")]
    public float maxPosture = 100f;
    public float currentPosture;
    public float postureRecoveryRate = 10f; // Por segundo
    public float postureBreakTime = 2f; // Tiempo que dura tambaleado
    private bool isStaggered = false;

    void Start()
    {
        currentHealth = maxHealth;
        currentPosture = maxPosture;
        UpdateHealthBar();
        UpdatePostureBar();
    }

    public void Damage(int damage,int postureDamage, GameObject enemyTransform)
    {
        if (!die)
        {
            if (isBlocking)
            {
                if (isPerfectBlock)
                {
                    Debug.Log("Parry perfecto: No se recibe daño y se rompe la postura del enemigo");
                    playerController.DisableWalk(0.4f);
                    //animator.Play("Parry");
                    Instantiate(parryParticle, gameObject.transform);
                    // Aquí deberías llamar algo como enemy.ReducePosture()
                    EnemyStats enemy = enemyTransform.GetComponent<EnemyStats>();
                    if (enemy != null)
                    {
                        enemy.ReducePosture(65f); // Por ejemplo
                        Debug.Log("Reduce postura Enemy");
                    }
                    return;
                }
                else
                {
                    currentPosture -= postureDamage;
                    UpdatePostureBar();
                    CheckPostureBreak();
                    Debug.Log("Bloqueo normal: daño a la resistencia");
                    audioBlock.Play();
                    return;
                }
            }


            currentHealth -= damage;
            playerController.DisableWalk(0.5f);
            animator.SetTrigger("Damage"); // Activa la animación de daño
            UpdateHealthBar();

            if (currentHealth <= 0)
            {
                Die();
            }
        }      
    }

    public void Heal()
    {
        if (healPotionCount > 0)
        {
            currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
            healPotionCount--;
            UpdateHealthBar();
        }
    }

    public void AddPotion()
    {
        if (healPotionCount < 3)
        {
            healPotionCount++;
        }
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        UpdateHealthBar();
    }

    private void Die()
    {        
        playerController.DieEvent();
        die = true;
        playerController.enabled = false;
        animator.SetTrigger("Die"); // Activa la animación de muerte
        animator.SetBool("isInteracting", true);
        animator.SetBool("isDying", true);
        Debug.Log("Jugador ha muerto");
        // Aquí puedes agregar lógica para desactivar controles, reiniciar nivel, etc.
    }
    // Revisar si se rompe la resistencia
    void CheckPostureBreak()
    {
        if (currentPosture <= 0 && !isStaggered)
        {
            StartCoroutine(Stagger());
        }
    }
    // Tambalear al jugador
    System.Collections.IEnumerator Stagger()
    {
        isStaggered = true;
        playerController.DisableWalk(0.6f);
        animator.Play("Stagger"); // Asegúrate de tener esta animación
        Debug.Log("¡Jugador tambaleado!");
        yield return new WaitForSeconds(postureBreakTime);
        currentPosture = maxPosture * 0.5f; // Empieza a la mitad
        isStaggered = false;
    }
    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
    }
    private void UpdatePostureBar()
    {
        currentPosture = Mathf.Min(currentPosture, maxPosture);
        postureBar.value = currentPosture / maxPosture;
    }

    // Llamado desde la aniamcion del arma
    public void SetBlock(bool isPerfect)
    {
        isBlocking = true;
        isPerfectBlock = isPerfect;
    }

    public void StopBlock()
    {
        isBlocking = false;
    }
}
