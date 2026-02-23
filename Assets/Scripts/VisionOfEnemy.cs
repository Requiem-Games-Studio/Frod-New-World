using UnityEngine;

public class VisionOfEnemy : MonoBehaviour
{
    [SerializeField] private float viewDistance = 5f;
    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private Transform firePoint1;
    [SerializeField] private Transform firePoint2;

    private bool isSeeingPlayer;
    public GameObject enemyController;

    private void Update()
    {
        int visionIndex = CheckVision();

        bool canSee = visionIndex != 0;

        if (canSee && !isSeeingPlayer)
        {
            if (visionIndex == 1)
                OnPlayerSpotted();
            else if (visionIndex == 2)
                OnPlayerSpotted2();
        }

        if (!canSee && isSeeingPlayer)
        {
            OnPlayerLost();
        }

        isSeeingPlayer = canSee;
    }

    private int CheckVision()
    {
        // 1 = firePoint1 detecta
        if (CanSeeFromPoint(firePoint1))
            return 1;

        // 2 = firePoint2 detecta
        if (CanSeeFromPoint(firePoint2))
            return 2;

        return 0;
    }

    private bool CanSeeFromPoint(Transform point)
    {
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(point.position, direction, viewDistance, playerLayer);

        Debug.DrawRay(point.position, direction * viewDistance, Color.red);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            PlayerController player = hit.collider.GetComponent<PlayerController>();

            if (!player.isHidden)
                return true;
        }

        return false;
    }

    private void OnPlayerSpotted()
    {
        Debug.Log("Jugador detectado por firePoint1");
        enemyController.SendMessage("PlayerSpotted");
    }

    private void OnPlayerSpotted2()
    {
        Debug.Log("Jugador detectado por firePoint2");
        enemyController.SendMessage("PlayerSpotted2");
    }

    private void OnPlayerLost()
    {
        Debug.Log("Jugador perdido");
        enemyController.SendMessage("PlayerLost");
    }
}
