using UnityEngine;


public class BossActivator : MonoBehaviour
{
    public BossController bossController;
    public EnemyStats bossHealth;
    bool activated;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            activated = true;
            bossController.ActivateBoss();
            bossHealth.ActivateBossBar();
        }
    }
}
