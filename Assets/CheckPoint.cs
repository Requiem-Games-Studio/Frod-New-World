using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    bool saved;

    public SaveController saveController;

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
                }                
            }
        }
    }
}
