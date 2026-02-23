using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
    [SerializeField] private float destroyTime = 2f;

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}
