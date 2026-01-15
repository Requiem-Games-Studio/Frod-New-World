using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;

public class SaveController : MonoBehaviour
{

    public GameObject player,mainCamera;
    SaveData data;

    void Start()
    {
        data = SaveManager.Instance.currentData;
        player.transform.position = data.playerPosition;
        mainCamera.transform.position = new Vector3(data.playerPosition.x, data.playerPosition.y, -10);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SavePlayerData();
        }
    }


    void SavePlayerData()
    {       
        // Llenar datos
        data.playerName = "Roper";
        data.playTime += Time.deltaTime;
        data.playProgress = GetProgress();
        data.playerPosition = player.transform.position;
        data.health = GetHealth();
        data.coins = GetCoins();
        data.unlockedPowers = GetUnlockedPowers();

        // Guardar en disco
        SaveManager.Instance.SaveGame(
            SaveManager.Instance.currentSlot,
            data
        );

        Debug.Log("Juego guardado correctamente");
    }

    float GetProgress() => 0.5f;
    int GetHealth() => 100;
    int GetCoins() => 250;
    List<PowerType> GetUnlockedPowers() => new List<PowerType> { PowerType.Dash };
}
