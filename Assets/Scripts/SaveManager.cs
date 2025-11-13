using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public int currentSlot;        // Slot actual en uso
    public SaveData currentData;   // Datos cargados del slot

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Guardar partida en un slot
    public void SaveGame(int slotIndex, SaveData data)
    {
        string path = GetSlotPath(slotIndex);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    //  Cargar partida de un slot
    public SaveData LoadGame(int slotIndex)
    {
        string path = GetSlotPath(slotIndex);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            Debug.LogWarning("No hay datos en el slot " + slotIndex);
            return null;
        }
    }

    //  Verificar si existe save en un slot
    public bool SaveExists(int slotIndex)
    {
        return File.Exists(GetSlotPath(slotIndex));
    }

    //  Obtener la ruta del slot
    private string GetSlotPath(int slotIndex)
    {
        return Application.persistentDataPath + "/saveSlot" + slotIndex + ".json";
    }
}
