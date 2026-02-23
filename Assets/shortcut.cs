using UnityEngine;

public class shortcut : MonoBehaviour
{
    public Animator animator;
    [Header("ID único")]
    public string collectableID;

    void Start()
    {
        // Si este collectable ya fue tomado antes no aparece
        if (SaveManager.Instance.currentData.takenCollectables.Contains("ShortCut" + collectableID))
        {
            animator.SetBool("isActive", true);
        }
    }

    public void SaveShortCut()
    {
        if (SaveManager.Instance.currentData.takenCollectables.Contains("ShortCut" + collectableID))
        {
            return;
        }

        SaveManager.Instance.currentData.takenCollectables.Add("ShortCut"+collectableID);
    }

}
