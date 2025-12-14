using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    public int damage, postureDamage;
    bool downAttack;
    public PlayerController controller;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyStats stats = collision.GetComponent<EnemyStats>();
            if (stats != null)
            {
                stats.Damage(damage, postureDamage);
            }

            if (downAttack)
            {                
                controller.PogoJump();
            }
        }
    }

    public void EnterDownAttack()
    {
        downAttack = true;
    }
    public void ExitDownAttack()
    {
        downAttack = false;
    }

}
