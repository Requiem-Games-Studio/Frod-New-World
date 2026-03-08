using UnityEngine;
using UnityEngine.UI;

public class WorldMapManager : MonoBehaviour
{
    public GameObject[] map0;
    public Transform calabozoMaps;

    private void Start()
    {
        CheckStatusMap();
    }

    public void CheckStatusMap()
    {
        if (SaveManager.Instance.currentData.takenCollectables.Contains("Map0"))
        {
            for (int i = 0; i < map0.Length; i++)
            {
                map0[i].SetActive(true);
            }
        }
    }

    public void RevealChunk(Vector2Int coord)
    {
        string name = coord.x + "," + coord.y;

        Transform chunk = calabozoMaps.Find(name);

        if (chunk != null)
        {
            chunk.gameObject.SetActive(true);
            chunk.gameObject.GetComponent<Image>().color = Color.white;
        }
    }
}
