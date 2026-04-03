using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveController : MonoBehaviour
{

    public GameObject player,mainCamera;
    public SaveData data;
    public WorldMapManager chunkManager;

    void Start()
    {
        data = SaveManager.Instance.currentData;
        LoadData();
    }

    void LoadData()
    {
        player.transform.position = data.playerPosition;
        Debug.Log("posicion del jugador" + data.playerPosition);
        mainCamera.transform.position = new Vector3(data.playerPosition.x, data.playerPosition.y, -10);
        Debug.Log("Load Chunks" + data.exploredChunks);
        chunkManager.exploredChunks = new HashSet<Vector2Int>(data.exploredChunks);
        chunkManager.CheckStatusMap();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SavePlayerData();
        }
    }


    public void SavePlayerData()
    {       
        // Llenar datos
        data.playerName = "Roper";
        data.playTime += Time.deltaTime;
        data.playProgress = GetProgress();
        data.playerPosition = player.transform.position;
        data.health = GetHealth();
        Debug.Log("Chunks guardados: " + chunkManager.exploredChunks.Count);
        data.exploredChunks = chunkManager.exploredChunks.ToList();
        data.collectables = GetCollectables();



    // Guardar en disco
    SaveManager.Instance.SaveGame(
            SaveManager.Instance.currentSlot,
            data
        );

        Debug.Log("Juego guardado correctamente");
    }

    float GetProgress() => 0.5f;
    int GetHealth() => 100;
    int GetCollectables() => 250;
}
