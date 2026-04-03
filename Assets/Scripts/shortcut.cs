using UnityEngine;

public class shortcut : MonoBehaviour
{
    public Animator animator;
    [Header("ID único")]
    public string collectableID;

    public bool open = false;

    void Start()
    {
        // Si este collectable ya fue tomado antes no aparece
        if (SaveManager.Instance.currentData.takenCollectables.Contains(collectableID) || open)
        {
            animator.SetBool("isActive", true);
        }
    }

    public void SaveShortCut()
    {
        if (SaveManager.Instance.currentData.takenCollectables.Contains(collectableID))
        {
            return;
        }

        SaveManager.Instance.currentData.takenCollectables.Add(collectableID);
    }

    public void TriggerAction()
    {
        if (!animator.GetBool("isActive"))
        {
            animator.SetBool("isActive", true);
        }
        else
        {
            animator.SetBool("isActive", false);
        }
    }

}
