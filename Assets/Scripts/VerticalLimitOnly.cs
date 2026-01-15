using UnityEngine;

public class VerticalLimitOnly : MonoBehaviour
{
    [Header("Limits")]
    [SerializeField] private float maxUpDistance = 2f;
    [SerializeField] private float maxDownDistance = 2f;

    private float lockedX;

    private float startY;

    private void Awake()
    {
        startY = transform.position.y;
        lockedX = transform.position.x;
    }

    private void LateUpdate()
    {
        ClampY();
        Vector3 pos = transform.position;
        pos.x = lockedX;
        transform.position = pos;
    }

    private void ClampY()
    {
        float minY = startY - maxDownDistance;
        float maxY = startY + maxUpDistance;

        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }
}
