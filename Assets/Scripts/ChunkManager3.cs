using System.Collections.Generic;
using UnityEngine;

public class ChunkManager3 : MonoBehaviour
{
    [Header("Chunk Settings")]
    public GameObject[] chunkPrefabs;
    public int chunkWidth = 34;
    public int chunkHeight = 24;

    private Transform player;
    private Camera mainCamera;

    private Dictionary<Vector2Int, GameObject> loadedChunks = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, GameObject> chunkPrefabMap = new Dictionary<Vector2Int, GameObject>();

    void Awake()
    {
        InitializeChunkMap();
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = Camera.main;

        UpdateChunks();
    }

    void Update()
    {
        UpdateChunks();
    }

    void InitializeChunkMap()
    {
        foreach (GameObject prefab in chunkPrefabs)
        {
            if (TryGetCoords(prefab.name, out Vector2Int coords))
            {
                if (!chunkPrefabMap.ContainsKey(coords))
                    chunkPrefabMap.Add(coords, prefab);
                else
                    Debug.LogWarning("Chunk duplicado en coordenada: " + coords);
            }
            else
            {
                Debug.LogError("Nombre inválido: " + prefab.name + " (Debe ser x,y)");
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

    void UpdateChunks()
    {
        Plane worldPlane = new Plane(Vector3.forward, Vector3.zero);

        Vector3[] screenPoints = new Vector3[]
        {
        new Vector3(0, 0, 0), // bottom left
        new Vector3(Screen.width, 0, 0), // bottom right
        new Vector3(0, Screen.height, 0), // top left
        new Vector3(Screen.width, Screen.height, 0) // top right
        };

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (Vector3 sp in screenPoints)
        {
            Ray ray = mainCamera.ScreenPointToRay(sp);

            if (worldPlane.Raycast(ray, out float enter))
            {
                Vector3 worldPoint = ray.GetPoint(enter);

                minX = Mathf.Min(minX, worldPoint.x);
                maxX = Mathf.Max(maxX, worldPoint.x);
                minY = Mathf.Min(minY, worldPoint.y);
                maxY = Mathf.Max(maxY, worldPoint.y);
            }
        }

        int minChunkX = Mathf.FloorToInt(minX / chunkWidth);
        int maxChunkX = Mathf.FloorToInt(maxX / chunkWidth);
        int minChunkY = Mathf.FloorToInt(minY / chunkHeight);
        int maxChunkY = Mathf.FloorToInt(maxY / chunkHeight);

        HashSet<Vector2Int> neededChunks = new HashSet<Vector2Int>();

        for (int x = minChunkX; x <= maxChunkX; x++)
        {
            for (int y = minChunkY; y <= maxChunkY; y++)
            {
                Vector2Int coord = new Vector2Int(x, y);
                neededChunks.Add(coord);

                if (!loadedChunks.ContainsKey(coord) && chunkPrefabMap.ContainsKey(coord))
                {
                    LoadChunk(coord);
                }
            }
        }

        List<Vector2Int> toUnload = new List<Vector2Int>();

        foreach (var chunk in loadedChunks.Keys)
        {
            if (!neededChunks.Contains(chunk))
                toUnload.Add(chunk);
        }

        foreach (var coord in toUnload)
        {
            UnloadChunk(coord);
        }
    }

    void LoadChunk(Vector2Int coord)
    {
        Vector3 pos = new Vector3(coord.x * chunkWidth, coord.y * chunkHeight, 0);

        GameObject newChunk = Instantiate(chunkPrefabMap[coord], pos, Quaternion.identity);
        newChunk.name = $"Chunk_{coord.x}_{coord.y}";

        loadedChunks.Add(coord, newChunk);
    }

    void UnloadChunk(Vector2Int coord)
    {
        if (loadedChunks.TryGetValue(coord, out GameObject chunk))
        {
            Destroy(chunk);
            loadedChunks.Remove(coord);
        }
    }
}
