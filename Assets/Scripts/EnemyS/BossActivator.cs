using UnityEngine;


public class BossActivator : MonoBehaviour
{
    public GameObject bossController;
    public EnemyStats bossHealth;
    bool activated;

    public GameObject barrera;
    public AudioSource music;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            Activate();
        }
    }

    public void Activate()
    {
        activated = true;
        bossController.SendMessage("ActivateBoss");
        bossHealth.ActivateBossBar();
        barrera.SetActive(true);
        if(music != null)
        {
            music.Play();
        }
    }

    public void Desactivate()
    {
        barrera.SetActive(false);
        if (music != null)
        {
            music.Stop();
        }
    }
}
