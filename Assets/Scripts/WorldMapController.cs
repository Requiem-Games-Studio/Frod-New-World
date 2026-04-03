using UnityEngine;
using UnityEngine.UI;

public class WorldMapController : MonoBehaviour
{
    [Header("Referencias")]

    public Transform player;
    public float xOffset, yOffset;
    public RectTransform mapContainer;
    public RectTransform playerIcon;
    public WorldMapManager mapManager;

    [Header("Limites del mundo")]
    public Vector2 worldMin = new Vector2(-935f, -492f);
    public Vector2 worldMax = new Vector2(935f, 492f);

    public float percentX;
    public float percentY;

    private void OnEnable()
    {
        UpdatePlayerIcon();
        CenterOnPlayer();
        mapManager.CheckStatusMap();
    }

    void UpdatePlayerIcon()
    {
        percentX = Mathf.InverseLerp(worldMin.x, worldMax.x, (player.position.x + xOffset));
        percentY = Mathf.InverseLerp(worldMin.y, worldMax.y, (player.position.y + yOffset));

        float mapWidth=mapContainer.rect.width;
        float mapHeight=mapContainer.rect.height;   

        playerIcon.anchoredPosition = new Vector2(percentX * mapWidth - mapWidth/2,percentY * mapHeight - mapHeight/2 );
    }

    void CenterOnPlayer()
    {
        mapContainer.anchoredPosition = -playerIcon.anchoredPosition;
    }



}
