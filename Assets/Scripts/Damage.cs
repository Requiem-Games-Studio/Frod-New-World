using UnityEngine;


public class Damage : MonoBehaviour
{
    public int damage, postureDamage;
    public bool damToPlayer, damToEnemy;
    public GameObject enemyObject;

    private void Start()
    {
        if (enemyObject == null)
        {
            enemyObject = this.gameObject;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && damToPlayer)
        {
            PlayerStats stats = collision.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.Damage(damage, postureDamage, enemyObject);
            }
        }

        if (collision.CompareTag("Enemy") && damToEnemy)
        {
            EnemyStats stats = collision.GetComponent<EnemyStats>();
            if (stats != null)
            {
                stats.Damage(damage, postureDamage);
            }
            else
            {
                collision.SendMessage("Damage", damage);
            }
        }
    }
}
