using UnityEngine;


public class BossActivator : MonoBehaviour
{
    public string collectableID;

    public GameObject boss;
    public GameObject bossController;
    public EnemyStats bossHealth;
    bool activated;

    public GameObject barrera;
    public AudioSource endCombat;
    GameObject manager;
    ChunkManagerByName chunkManager;
    MusicManager musicManager;

    void Start()
    {
        // Si este collectable ya fue tomado antes no aparece
        if (SaveManager.Instance.currentData.takenCollectables.Contains(collectableID))
        {
            Destroy(boss);
            Destroy(gameObject);
        }
    }

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
        manager = GameObject.FindGameObjectWithTag("Manager");
        chunkManager = manager.GetComponent<ChunkManagerByName>();
        musicManager = manager.GetComponent<MusicManager>();
        musicManager.PlayBossFight();
        chunkManager.enabled = false;
        bossController.SendMessage("ActivateBoss");
        bossHealth.ActivateBossBar();
        barrera.SetActive(true);
    }

    public void Desactivate()
    {
        SaveManager.Instance.currentData.takenCollectables.Add(collectableID);
        chunkManager.enabled = true;
        barrera.SetActive(false);
        endCombat.Play();
        musicManager.PlayCalabozo();
    }
}
