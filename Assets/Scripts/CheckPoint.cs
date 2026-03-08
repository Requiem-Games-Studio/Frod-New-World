using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    bool saved;

    public SaveController saveController;
    public GameObject particles;

    private void Start()
    {
        saveController = GameObject.FindWithTag("Manager").GetComponent<SaveController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!saved)
            {
                if(saveController != null)
                {
                    saved = true;
                    saveController.SavePlayerData();
                    Instantiate(particles,transform.position,Quaternion.identity);
                    GameFeelManager.Instance.DoSlowMotion(0.2f, 0.6f);
                }                
            }
            collision.SendMessage("StartRegeneration");
        }
    }
}
