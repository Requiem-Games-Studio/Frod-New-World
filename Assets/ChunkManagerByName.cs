using System.Collections.Generic;
using UnityEngine;

public class ChunkManagerByName : MonoBehaviour
{
    public GameObject[] chunkPrefabs; // Prefabs de los chunks
    public Vector2Int[] chunkPositions; // Coordenadas 
    public int chunkWidth = 34;  // Ancho del chunk
    public int chunkHeight = 24; // Alto del chunk
    public int loadRadius = 1; // Radio de carga en chunks

    private Transform player;
    private Dictionary<Vector2Int, GameObject> loadedChunks = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, GameObject> chunkPrefabMap = new Dictionary<Vector2Int, GameObject>();

    private void Awake()
    {
        chunkPositions = new Vector2Int[chunkPrefabs.Length];

        for (int i = 0; i < chunkPrefabs.Length; i++)
        {
            if (TryGetCoords(chunkPrefabs[i].name, out Vector2Int coords))
            {
                chunkPositions[i] = coords;
            }
            else
            {
                Debug.LogError("Nombre inválido: " + chunkPrefabs[i].name);
            }
        }
    }

    bool TryGetCoords(string name, out Vector2Int coords)
    {
        coords = Vector2Int.zero;

        string[] split = name.Split(',');
        if (split.Length != 2) return false;

        if (!int.TryParse(split[0], out int x)) return false;
        if (!int.TryParse(split[1], out int y)) return false;

        coords = new Vector2Int(x, y);
        return true;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Asignar prefabs a cada coordenada al inicio
        InitializeChunkMap();

        // Cargar los chunks iniciales
        UpdateChunks();
    }

    void Update()
    {
        UpdateChunks();
    }

    void InitializeChunkMap()
    {
        for (int i = 0; i < chunkPositions.Length; i++)
        {
            Vector2Int chunkCoord = chunkPositions[i];
            GameObject prefab = chunkPrefabs[i % chunkPrefabs.Length]; // Ciclo de prefabs para evitar errores
            chunkPrefabMap[chunkCoord] = prefab;
        }
    }

    void UpdateChunks()
    {
        Vector2Int playerChunk = GetPlayerChunk();

        // Cargar nuevos chunks dentro del radio
        for (int x = -loadRadius; x <= loadRadius; x++)
        {
            for (int y = -loadRadius; y <= loadRadius; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(playerChunk.x + x, playerChunk.y + y);
                if (!loadedChunks.ContainsKey(chunkCoord) && chunkPrefabMap.ContainsKey(chunkCoord))
                {
                    LoadChunk(chunkCoord);
                }
            }
        }

        // Descargar chunks fuera del radio
        List<Vector2Int> chunksToUnload = new List<Vector2Int>();
        foreach (var chunk in loadedChunks.Keys)
        {
            if (Vector2Int.Distance(chunk, playerChunk) > loadRadius)
            {
                chunksToUnload.Add(chunk);
            }
        }

        foreach (var chunkCoord in chunksToUnload)
        {
            UnloadChunk(chunkCoord);
        }
    }

    Vector2Int GetPlayerChunk()
    {
        int chunkX = Mathf.FloorToInt(player.position.x / chunkWidth);
        int chunkY = Mathf.FloorToInt(player.position.y / chunkHeight);
        return new Vector2Int(chunkX, chunkY);
    }

    void LoadChunk(Vector2Int chunkCoord)
    {
        Vector3 chunkPosition = new Vector3(chunkCoord.x * chunkWidth, chunkCoord.y * chunkHeight, 0);
        GameObject chunkPrefab = chunkPrefabMap[chunkCoord];

        if (chunkPrefab != null)
        {
            GameObject newChunk = Instantiate(chunkPrefab, chunkPosition, Quaternion.identity);
            newChunk.name = $"Chunk_{chunkCoord.x}_{chunkCoord.y}";
            loadedChunks.Add(chunkCoord, newChunk);
        }
    }

    void UnloadChunk(Vector2Int chunkCoord)
    {
        if (loadedChunks.TryGetValue(chunkCoord, out GameObject chunk))
        {
            Destroy(chunk);
            loadedChunks.Remove(chunkCoord);
        }
    }
}
