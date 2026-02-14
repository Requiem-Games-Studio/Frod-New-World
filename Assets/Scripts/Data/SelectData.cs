using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectData : MonoBehaviour
{
    public TMPro.TextMeshProUGUI[] collectables;
    public TMPro.TextMeshProUGUI[] time;
    public TMPro.TextMeshProUGUI[] progress;
    public TMPro.TextMeshProUGUI[] place;
    public Image[] image;

    public Sprite defaultImage;

    private void Start()
    {
        CheckSlot();
    }

    public void CheckSlot()
    {
        for (int i = 0; i < 4; i++) // 4 slots
        {
            if (SaveManager.Instance.SaveExists(i))
            {
                SaveData data = SaveManager.Instance.LoadGame(i);

                Debug.Log("Slot " + i + " ocupado");
                place[i].text = "" + data.playerName;
                collectables[i].text = "Collect:" + data.collectables;
                time[i].text = "Time: " + data.playTime.ToString("F1") + "s";
                progress[i].text = "" + data.playProgress + "%";
            }
            else
            {
                Debug.Log("Slot " + i + " vacío");

                place[i].text = "";
                collectables[i].text = "Vacío";
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
            newData.collectables = 0;
            newData.playTime = 0f;
            newData.playProgress = 0f;
            newData.playerPosition = new Vector2(-547, -48.4f);

            SaveManager.Instance.SaveGame(slotIndex, newData);

            SaveManager.Instance.currentSlot = slotIndex;
            SaveManager.Instance.currentData = newData;
        }

        SceneManager.LoadScene("Game");
    }

    public void DeletSlot(int slotID)
    {
        SaveManager.Instance.DeleteSlot(slotID);
        place[slotID].text = "";
        collectables[slotID].text = "Vacío";
        time[slotID].text = "";
        progress[slotID].text = "";

    }

}
