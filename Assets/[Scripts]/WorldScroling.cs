using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldScroling : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap backgroundTilemap;
    [SerializeField] private Tilemap middlegroundTilemap;

    [Header("Background Tiles")]
    [Tooltip("All grass tiles that can appear in the background.")]
    [SerializeField] private TileBase[] grassTiles;

    [Tooltip("Main plain grass tile that should appear most often.")]
    [SerializeField] private TileBase plainGrassTile;

    [Range(0f, 1f)]
    [Tooltip("Chance to use plain grass instead of a random grass variant.")]
    [SerializeField] private float plainGrassChance = 0.75f;

    [Header("Water Tiles")]
    [SerializeField] private bool generateWater = true;
    [SerializeField] private TileBase[] waterTiles;

    [Header("Prop Tiles")]
    [SerializeField] private bool generateProps = false;
    [SerializeField] private TileBase[] propTiles;

    [Header("Generation Area")]
    [Tooltip("How many cells around the player should be generated.")]
    [SerializeField] private int generateRadius = 30;

    [Tooltip("Extra distance before old cells are cleared.")]
    [SerializeField] private int clearBuffer = 12;

    [Tooltip("Do not place water or props too close to the player.")]
    [SerializeField] private int noFeatureRadius = 8;

    [Header("Generation Speed")]
    [SerializeField] private int tilesPerFrame = 800;
    [SerializeField] private float refreshInterval = 0.15f;

    [Header("Feature Chances")]
    [Range(0f, 1f)]
    [SerializeField] private float waterChance = 0.03f;

    [Range(0f, 1f)]
    [SerializeField] private float propChance = 0.01f;

    [Header("World Seed")]
    [Tooltip("Change this if you want a different world layout.")]
    [SerializeField] private int worldSeed = 12345;

    private readonly HashSet<Vector3Int> placedCells = new HashSet<Vector3Int>();
    private readonly HashSet<Vector3Int> queuedCells = new HashSet<Vector3Int>();
    private readonly Queue<Vector3Int> generationQueue = new Queue<Vector3Int>();

    private float timer;
    private Vector3Int lastPlayerCell;

    private void Start()
    {
        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        lastPlayerCell = backgroundTilemap.WorldToCell(player.position);

        EnqueueCellsAround(lastPlayerCell);
        GenerateFromQueue(tilesPerFrame * 2, lastPlayerCell);
    }

    private void Update()
    {
        Vector3Int playerCell = backgroundTilemap.WorldToCell(player.position);

        timer += Time.deltaTime;
        if (timer >= refreshInterval)
        {
            timer = 0f;

            if (playerCell != lastPlayerCell)
            {
                lastPlayerCell = playerCell;
                EnqueueCellsAround(playerCell);
                ClearFarCells(playerCell);
            }
        }

        GenerateFromQueue(tilesPerFrame, playerCell);
    }

    private bool ValidateReferences()
    {
        if (player == null || backgroundTilemap == null || middlegroundTilemap == null)
        {
            Debug.LogError("WorldScroling: Missing player or tilemap reference.");
            return false;
        }

        if (grassTiles == null || grassTiles.Length == 0)
        {
            Debug.LogError("WorldScroling: Add at least one grass tile.");
            return false;
        }

        if (waterTiles == null || waterTiles.Length == 0)
        {
            generateWater = false;
        }

        if (propTiles == null || propTiles.Length == 0)
        {
            generateProps = false;
        }

        return true;
    }

    private void EnqueueCellsAround(Vector3Int centerCell)
    {
        int minX = centerCell.x - generateRadius;
        int maxX = centerCell.x + generateRadius;
        int minY = centerCell.y - generateRadius;
        int maxY = centerCell.y + generateRadius;

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);

                if (placedCells.Contains(cell)) continue;
                if (queuedCells.Contains(cell)) continue;

                queuedCells.Add(cell);
                generationQueue.Enqueue(cell);
            }
        }
    }

    private void GenerateFromQueue(int budget, Vector3Int playerCell)
    {
        int generatedCount = 0;

        while (generatedCount < budget && generationQueue.Count > 0)
        {
            Vector3Int cell = generationQueue.Dequeue();
            queuedCells.Remove(cell);

            if (placedCells.Contains(cell))
            {
                generatedCount++;
                continue;
            }

            PlaceCell(cell, playerCell);
            placedCells.Add(cell);

            generatedCount++;
        }
    }

    private void PlaceCell(Vector3Int cell, Vector3Int playerCell)
    {
        PlaceBackground(cell);
        PlaceMiddleground(cell, playerCell);
    }

    private void PlaceBackground(Vector3Int cell)
    {
        // Do not overwrite manually painted background tiles.
        if (backgroundTilemap.HasTile(cell)) return;

        TileBase grassTile = GetGrassTile(cell);
        backgroundTilemap.SetTile(cell, grassTile);
    }

    private void PlaceMiddleground(Vector3Int cell, Vector3Int playerCell)
    {
        // Do not overwrite manually painted middleground tiles.
        if (middlegroundTilemap.HasTile(cell)) return;

        if (IsNearPlayer(cell, playerCell)) return;

        TileBase featureTile = GetFeatureTile(cell);
        if (featureTile != null)
        {
            middlegroundTilemap.SetTile(cell, featureTile);
        }
    }

    private TileBase GetGrassTile(Vector3Int cell)
    {
        // Use plain grass most of the time if assigned.
        if (plainGrassTile != null)
        {
            float grassRoll = GetCellRandom01(cell.x, cell.y, 10);

            if (grassRoll < plainGrassChance)
            {
                return plainGrassTile;
            }
        }

        int index = GetCellRandomIndex(cell.x, cell.y, 11, grassTiles.Length);
        return grassTiles[index];
    }

    private TileBase GetFeatureTile(Vector3Int cell)
    {
        float featureRoll = GetCellRandom01(cell.x, cell.y, 20);

        // Water gets first priority.
        if (generateWater && featureRoll < waterChance)
        {
            int waterIndex = GetCellRandomIndex(cell.x, cell.y, 21, waterTiles.Length);
            return waterTiles[waterIndex];
        }

        // Props use a second range after water.
        if (generateProps && featureRoll < waterChance + propChance)
        {
            int propIndex = GetCellRandomIndex(cell.x, cell.y, 22, propTiles.Length);
            return propTiles[propIndex];
        }

        return null;
    }

    private bool IsNearPlayer(Vector3Int cell, Vector3Int playerCell)
    {
        int dx = cell.x - playerCell.x;
        int dy = cell.y - playerCell.y;

        // Circular safe zone around the player.
        return (dx * dx + dy * dy) <= (noFeatureRadius * noFeatureRadius);
    }

    private void ClearFarCells(Vector3Int centerCell)
    {
        int keepRadius = generateRadius + clearBuffer;
        int keepRadiusSqr = keepRadius * keepRadius;

        List<Vector3Int> cellsToRemove = null;

        foreach (Vector3Int cell in placedCells)
        {
            int dx = cell.x - centerCell.x;
            int dy = cell.y - centerCell.y;

            if ((dx * dx + dy * dy) > keepRadiusSqr)
            {
                cellsToRemove ??= new List<Vector3Int>();
                cellsToRemove.Add(cell);
            }
        }

        if (cellsToRemove == null) return;

        for (int i = 0; i < cellsToRemove.Count; i++)
        {
            Vector3Int cell = cellsToRemove[i];

            backgroundTilemap.SetTile(cell, null);
            middlegroundTilemap.SetTile(cell, null);
            placedCells.Remove(cell);
        }
    }

    private float GetCellRandom01(int x, int y, int salt)
    {
        int hash = x;
        hash ^= y * 374761393;
        hash ^= salt * 668265263;
        hash ^= worldSeed * 1442695041;
        hash = (hash ^ (hash >> 13)) * 1274126177;
        hash ^= hash >> 16;

        uint unsignedHash = (uint)hash;
        return unsignedHash / (float)uint.MaxValue;
    }

    private int GetCellRandomIndex(int x, int y, int salt, int length)
    {
        if (length <= 1) return 0;

        float value = GetCellRandom01(x, y, salt);
        int index = Mathf.FloorToInt(value * length);

        if (index >= length)
        {
            index = length - 1;
        }

        return index;
    }
}