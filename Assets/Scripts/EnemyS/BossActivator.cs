using UnityEngine;


public class BossActivator : MonoBehaviour
{
    public GameObject bossController;
    public EnemyStats bossHealth;
    bool activated;

    public GameObject barrera;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            activated = true;
            bossController.SendMessage("ActivateBoss");
            bossHealth.ActivateBossBar();
            barrera.SetActive(true);
        }
    }
}
