using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldScroling : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Tilemap groundTilemap;

    [Header("Ground Tile Variants (drag your grass tiles here)")]
    [SerializeField] private TileBase[] grassTiles;

    [Header("Optional Water (drag water tiles here, or leave empty)")]
    [SerializeField] private bool generateWater = true;
    [SerializeField] private TileBase[] waterTiles;

    [Header("Generation Area (in CELLS)")]
    [Tooltip("How far from the player we ensure tiles exist. 30 => 61x61 area.")]
    [SerializeField] private int generateRadius = 30;

    [Tooltip("Keep a bit extra before clearing so edges never show.")]
    [SerializeField] private int clearBuffer = 12;

    [Header("Performance")]
    [Tooltip("How many new cells to generate per frame. Increase if you see empty edges, decrease if you see frame drops.")]
    [SerializeField] private int tilesPerFrame = 800;

    [Tooltip("How often to refresh the generation queue (seconds).")]
    [SerializeField] private float refreshInterval = 0.15f;

    [Header("Water Tuning (simple + random)")]
    [Range(0f, 1f)]
    [Tooltip("Chance that a cell becomes water instead of grass (only if generateWater is enabled).")]
    [SerializeField] private float waterChance = 0.03f;

    [Tooltip("Try to avoid water near the player start position (in cells).")]
    [SerializeField] private int noWaterRadius = 8;

    // Tracks which cells currently have something placed by this generator
    private readonly HashSet<Vector3Int> placed = new HashSet<Vector3Int>();

    // Cells already queued for generation (prevents queue spam)
    private readonly HashSet<Vector3Int> queued = new HashSet<Vector3Int>();

    // Queue of cells we still need to generate
    private readonly Queue<Vector3Int> genQueue = new Queue<Vector3Int>();

    private float timer;
    private Vector3Int lastPlayerCell;

    private void Start()
    {
        // Basic validation
        if (player == null)
        {
            Debug.LogError("InfiniteTilemapGround: Player reference is missing.");
            enabled = false;
            return;
        }
        if (groundTilemap == null)
        {
            Debug.LogError("InfiniteTilemapGround: Ground Tilemap reference is missing.");
            enabled = false;
            return;
        }
        if (grassTiles == null || grassTiles.Length == 0)
        {
            Debug.LogError("InfiniteTilemapGround: Add at least 1 grass tile into Grass Tiles.");
            enabled = false;
            return;
        }

        // If water arrays are empty, disable water generation automatically
        if (waterTiles == null || waterTiles.Length == 0) generateWater = false;

        lastPlayerCell = groundTilemap.WorldToCell(player.position);

        // Kickstart generation
        EnqueueCellsAround(lastPlayerCell);
        GenerateFromQueue(tilesPerFrame * 2); // burst on start so you don't see emptiness
    }

    private void Update()
    {
        Vector3Int playerCell = groundTilemap.WorldToCell(player.position);

        timer += Time.deltaTime;
        if (timer >= refreshInterval)
        {
            timer = 0f;

            // Only refresh generation area if the player moved to a new cell
            if (playerCell != lastPlayerCell)
            {
                lastPlayerCell = playerCell;
                EnqueueCellsAround(playerCell);
                ClearFarCells(playerCell);
            }
        }

        // Generate a limited amount per frame for smoothness
        GenerateFromQueue(tilesPerFrame);
    }

    private void EnqueueCellsAround(Vector3Int centerCell)
    {
        int minX = centerCell.x - generateRadius;
        int maxX = centerCell.x + generateRadius;
        int minY = centerCell.y - generateRadius;
        int maxY = centerCell.y + generateRadius;

        // Add missing cells into queue (we don't place immediately)
        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);

                if (placed.Contains(cell)) continue;
                if (queued.Contains(cell)) continue;

                queued.Add(cell);
                // Avoid enqueuing duplicates endlessly
                // (This also keeps the queue from ballooning too hard)
                genQueue.Enqueue(cell);
            }
        }
    }

    private void GenerateFromQueue(int budget)
    {
        int count = 0;

        while (count < budget && genQueue.Count > 0)
        {
            Vector3Int cell = genQueue.Dequeue();
            queued.Remove(cell);

            // Another safety check: it might already have been placed by earlier queued work
            if (placed.Contains(cell)) continue;

            PlaceOneCell(cell);
            placed.Add(cell);
            count++;
        }
    }

    private void PlaceOneCell(Vector3Int cell)
    {
        // Don't overwrite anything you already painted by hand
        if (groundTilemap.HasTile(cell)) return;

        // Optional: avoid water near the player so you don't spawn in a lake
        Vector3Int playerCell = groundTilemap.WorldToCell(player.position);
        int dx = Mathf.Abs(cell.x - playerCell.x);
        int dy = Mathf.Abs(cell.y - playerCell.y);
        bool nearPlayer = (dx <= noWaterRadius && dy <= noWaterRadius);

        if (generateWater && !nearPlayer && Random.value < waterChance)
        {
            TileBase water = waterTiles[Random.Range(0, waterTiles.Length)];
            groundTilemap.SetTile(cell, water);
        }
        else
        {
            TileBase grass = grassTiles[Random.Range(0, grassTiles.Length)];
            groundTilemap.SetTile(cell, grass);
        }
    }

    private void ClearFarCells(Vector3Int centerCell)
    {
        int keepRadius = generateRadius + clearBuffer;

        // We can’t remove from HashSet while iterating directly, so collect first
        List<Vector3Int> toRemove = null;

        foreach (var cell in placed)
        {
            int dx = Mathf.Abs(cell.x - centerCell.x);
            int dy = Mathf.Abs(cell.y - centerCell.y);

            // Outside keep square => clear it
            if (dx > keepRadius || dy > keepRadius)
            {
                toRemove ??= new List<Vector3Int>();
                toRemove.Add(cell);
            }
        }

        if (toRemove == null) return;

        foreach (var cell in toRemove)
        {
            groundTilemap.SetTile(cell, null);
            placed.Remove(cell);
        }
    }
}
