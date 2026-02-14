using UnityEngine;

public class ObjectStats : MonoBehaviour
{

    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    private bool isAlive = true;

    public AudioSource audioSource;
    public GameObject hitParticle, deadParticle;

    public FlashSprite flashSprite;
    public GameObject[] fragments; 

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Recibir daño
    public void Damage(float damage)
    {
        if (!isAlive) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
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


    void Die()
    {
        if(fragments != null)
        {
            for(int i = 0; i < fragments.Length; i++)
            {
                fragments[i].gameObject.SetActive(true);
            }
        }              
        isAlive = false;
        Debug.Log("Enemigo muerto.");
        Destroy(gameObject);
    }
}
