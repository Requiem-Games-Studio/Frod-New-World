using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SelectData : MonoBehaviour
{
    public TMPro.TextMeshProUGUI[] coins;
    public TMPro.TextMeshProUGUI[] time;
    public TMPro.TextMeshProUGUI[] progress;

    private void Start()
    {
        CheckSlot();
    }

    public void CheckSlot()
    {
        for (int i = 0; i < 3; i++) // 4 slots
        {
            if (SaveManager.Instance.SaveExists(i))
            {
                SaveData data = SaveManager.Instance.LoadGame(i);

                Debug.Log("Slot " + i + " ocupado");

                coins[i].text = "Monedas: " + data.coins;
                time[i].text = "Tiempo: " + data.playTime.ToString("F1") + "s";
                progress[i].text = "Progreso: " + data.playProgress + "%";
            }
            else
            {
                Debug.Log("Slot " + i + " vacío");

                coins[i].text = "Vacío";
                time[i].text = "";
                progress[i].text = "";
            }
        }
    }

    public void SelectSlot(int slotIndex)
    {
        if (SaveManager.Instance.SaveExists(slotIndex))
        {
            Debug.Log("Cargando partida del slot " + slotIndex);
            SaveManager.Instance.currentSlot = slotIndex;
            SaveManager.Instance.currentData = SaveManager.Instance.LoadGame(slotIndex);
        }
        else
        {
            Debug.Log("Slot vacío, creando nueva partida...");
            SaveData newData = new SaveData();
            newData.coins = 0;
            newData.playTime = 0f;
            newData.playProgress = 0f;

            SaveManager.Instance.SaveGame(slotIndex, newData);

            SaveManager.Instance.currentSlot = slotIndex;
            SaveManager.Instance.currentData = newData;
        }

        SceneManager.LoadScene("Game");
    }
}
