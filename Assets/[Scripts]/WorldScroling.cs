using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldScroling : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap backgroundTilemap;   // Grass Background
    [SerializeField] private Tilemap middlegroundTilemap; // Middleground (water/props)

    [Header("Background: Grass Variants")]
    [Tooltip("Put your plain grass + grass detail variants here (Tile assets).")]
    [SerializeField] private TileBase[] grassTiles;

    [Header("Middleground: Water Tiles")]
    [SerializeField] private bool generateWater = true;
    [SerializeField] private TileBase[] waterTiles;

    [Header("Middleground: Prop Tiles (optional)")]
    [SerializeField] private bool generateProps = false;
    [SerializeField] private TileBase[] propTiles; // trees, bushes, rocks, etc.

    [Header("Generation Area (in CELLS)")]
    [SerializeField] private int generateRadius = 30;
    [SerializeField] private int clearBuffer = 12;

    [Header("Performance")]
    [SerializeField] private int tilesPerFrame = 800;
    [SerializeField] private float refreshInterval = 0.15f;

    [Header("Tuning (simple + random)")]
    [Range(0f, 1f)][SerializeField] private float waterChance = 0.03f;
    [Range(0f, 1f)][SerializeField] private float propChance = 0.01f;

    [Tooltip("Don't generate water/props too close to player (cells).")]
    [SerializeField] private int noFeatureRadius = 8;

    // Weighted grass (simple)
    [Header("Grass Weighting (simple)")]
    [Tooltip("Drag your 'plain grass' tile here so it appears more often.")]
    [SerializeField] private TileBase plainGrassTile;

    [Range(0f, 1f)]
    [Tooltip("Chance to use plain grass instead of a random variant. Example: 0.75 = 75% plain grass.")]
    [SerializeField] private float plainGrassChance = 0.75f;

    private readonly HashSet<Vector3Int> placed = new HashSet<Vector3Int>();
    private readonly HashSet<Vector3Int> queued = new HashSet<Vector3Int>();
    private readonly Queue<Vector3Int> genQueue = new Queue<Vector3Int>();

    private float timer;
    private Vector3Int lastPlayerCell;


    private void Start()
    {
        if (player == null || backgroundTilemap == null || middlegroundTilemap == null)
        {
            Debug.LogError("WorldScrolling: Missing references (player/background/middleground tilemap).");
            enabled = false;
            return;
        }
        if (grassTiles == null || grassTiles.Length == 0)
        {
            Debug.LogError("WorldScrolling: Add at least 1 grass tile into grassTiles.");
            enabled = false;
            return;
        }

        if (waterTiles == null || waterTiles.Length == 0) generateWater = false;
        if (propTiles == null || propTiles.Length == 0) generateProps = false;

        lastPlayerCell = backgroundTilemap.WorldToCell(player.position);

        EnqueueCellsAround(lastPlayerCell);
        GenerateFromQueue(tilesPerFrame * 2);
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

        GenerateFromQueue(tilesPerFrame);
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

                if (placed.Contains(cell)) continue;
                if (queued.Contains(cell)) continue;

                queued.Add(cell);
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

            if (placed.Contains(cell)) continue;

            bool didPlace = PlaceOneCell(cell);
            if (didPlace)
            {
                placed.Add(cell);
            }

            count++;
        }
    }

    private bool PlaceOneCell(Vector3Int cell)
    {
        bool placedAnything = false;

        // Background (grass): only fill if empty (don't overwrite painted)
        if (!backgroundTilemap.HasTile(cell))
        {
            backgroundTilemap.SetTile(cell, PickGrassTile());
            placedAnything = true;
        }

        // Middleground features: only fill if empty (don't overwrite painted)
        Vector3Int playerCell = backgroundTilemap.WorldToCell(player.position);
        int dx = Mathf.Abs(cell.x - playerCell.x);
        int dy = Mathf.Abs(cell.y - playerCell.y);
        bool nearPlayer = (dx <= noFeatureRadius && dy <= noFeatureRadius);

        if (nearPlayer) return placedAnything;

        if (!middlegroundTilemap.HasTile(cell))
        {
            // Water first, then props (simple priority)
            if (generateWater && Random.value < waterChance)
            {
                TileBase water = waterTiles[Random.Range(0, waterTiles.Length)];
                middlegroundTilemap.SetTile(cell, water);
                placedAnything = true;
            }
            else if (generateProps && Random.value < propChance)
            {
                TileBase prop = propTiles[Random.Range(0, propTiles.Length)];
                middlegroundTilemap.SetTile(cell, prop);
                placedAnything = true;
            }
        }

        return placedAnything;
    }

    private TileBase PickGrassTile()
    {
        // Super simple weighting:
        // - If plainGrassTile is assigned and Random < plainGrassChance -> use it
        // - Otherwise pick a random grass variant
        if (plainGrassTile != null && Random.value < plainGrassChance)
            return plainGrassTile;

        return grassTiles[Random.Range(0, grassTiles.Length)];
    }

    private void ClearFarCells(Vector3Int centerCell)
    {
        int keepRadius = generateRadius + clearBuffer;
        List<Vector3Int> toRemove = null;

        foreach (var cell in placed)
        {
            int dx = Mathf.Abs(cell.x - centerCell.x);
            int dy = Mathf.Abs(cell.y - centerCell.y);

            if (dx > keepRadius || dy > keepRadius)
            {
                toRemove ??= new List<Vector3Int>();
                toRemove.Add(cell);
            }
        }

        if (toRemove == null) return;

        foreach (var cell in toRemove)
        {
            // Only clear what this script likely placed.
            // If you manually painted far away, it could be cleared; easiest rule for now is:
            // - Only paint your handcrafted area near spawn OR disable clearing while testing.
            backgroundTilemap.SetTile(cell, null);
            middlegroundTilemap.SetTile(cell, null);
            placed.Remove(cell);
        }
    }
}
