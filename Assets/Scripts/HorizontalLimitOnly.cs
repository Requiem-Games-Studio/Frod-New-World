using UnityEngine;

public class HorizontalLimitOnly : MonoBehaviour
{
    [Header("Limits")]
    [SerializeField] private float maxRightDistance = 2f;
    [SerializeField] private float maxLeftDistance = 2f;

    private float lockedY;

    private float startX;

    private void Awake()
    {
        startX = transform.position.x;
        lockedY = transform.position.y;
    }

    private void LateUpdate()
    {
        ClampY();
        Vector3 pos = transform.position;
        pos.y = lockedY;
        transform.position = pos;
    }

    private void ClampY()
    {
        float minX = startX - maxLeftDistance;
        float maxX = startX + maxRightDistance;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;
    }
}
