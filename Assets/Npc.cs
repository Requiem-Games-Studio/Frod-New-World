using UnityEngine;
using static System.TimeZoneInfo;

public class Npc : MonoBehaviour
{
    GameObject dialoguePanel;

    public bool setDialogue;

    public float transitionTime = 0.5f;
    public Collider2D newCameraBounds,npcCameraBounds;
    private bool isTransitioning = false;
    bool inDialogue;

    CameraFollow camFollow;

    public float newFieldOfView;

    public GameObject globe;


    private void Update()
    {
        if(inDialogue && Input.GetKeyDown(KeyCode.E))
        {
            NextDialogue();
        }
    }

    void NextDialogue()
    {
        if (!setDialogue)
        {
            setDialogue = true;
            GameObject canvasBoss = GameObject.FindGameObjectWithTag("Canvas");
            dialoguePanel = canvasBoss.transform.Find("DialoguePanel").gameObject;
        }
        if(!dialoguePanel.activeSelf)
        {
            dialoguePanel.SetActive(true);
        }
        globe.SetActive(false);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            if(camFollow == null)
            {
                camFollow = Camera.main.GetComponent<CameraFollow>();               
            }

            camFollow.NewFieldOfView(newFieldOfView);

            if (camFollow != null && npcCameraBounds != null)
            {
                StartCoroutine(ChangeBoundsSmooth(camFollow, npcCameraBounds));
            }
            inDialogue = true;
            globe.SetActive(true);
        }            
    }

    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
            if (camFollow != null && newCameraBounds != null)
            {
                camFollow.ResetFieldOfView();
                StartCoroutine(ChangeBoundsSmooth(camFollow, newCameraBounds));
            }
            inDialogue = false;
            globe.SetActive(false);
        }
    }

    private System.Collections.IEnumerator ChangeBoundsSmooth(CameraFollow camFollow, Collider2D targetBounds)
    {
        isTransitioning = true;

        // Guardamos los bounds actuales
        Collider2D oldBounds = camFollow.cameraBounds;
        camFollow.cameraBounds = targetBounds;

        // Si quieres interpolar entre bounds (opcional)
        float elapsed = 0f;
        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        isTransitioning = false;
    }

}
