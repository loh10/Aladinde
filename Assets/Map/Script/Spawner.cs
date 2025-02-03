using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Spawner : MonoBehaviour
{
    [SerializeField] private int _amountOfConsumables;
    [SerializeField] private GameObject[] _consumables;
    [SerializeField] private Vector2 _spawnRange = new Vector2(-5f, 5f);
    [SerializeField] private Tilemap _collisionTilemap;
    [SerializeField] private Grid _grid;
    [SerializeField] private LayerMask _obstacleLayer;

    private HashSet<Vector3Int> _occupiedCells = new HashSet<Vector3Int>();


    void Start()
    {
        ScanObstacles();
        SpawnConsumable();
    }

    /// <summary>
    /// Obstacle Detection by OverlappingBox, if an obstacle with the obstacleLayer and a collider is detected, the cell is marked as occupied
    /// </summary>
    private void ScanObstacles()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, _spawnRange * 2, 0, _obstacleLayer);

        foreach (Collider2D col in colliders)
        {
            Vector3Int cellPosition = _grid.WorldToCell(col.transform.position);
            _occupiedCells.Add(cellPosition);
        }

    }

    /// <summary>
    /// Function to spawn the differents consumables, select a random consumable in the list and make them spawn
    /// </summary>
    public void SpawnConsumable()
    {
        int spawnedCount = 0;
        int maxAttempts = _amountOfConsumables * 5;
        int attempts = 0;

        while (spawnedCount < _amountOfConsumables && attempts < maxAttempts)
        {
            Vector2 spawnPosition = new Vector2(
                Random.Range(transform.position.x - _spawnRange.x, transform.position.x + _spawnRange.x), 
                Random.Range(transform.position.y - _spawnRange.y, transform.position.y + _spawnRange.y)
                );
          
            Vector3Int cellPosition = _grid.WorldToCell(spawnPosition);
            Vector2 cellCenter = _grid.GetCellCenterWorld(cellPosition);

            if (IsValidSpawnPosition(cellPosition, cellCenter))
            {
                GameObject randomPrefab = _consumables[Random.Range(0, _consumables.Length)];

                Instantiate(randomPrefab, cellCenter, Quaternion.identity);
                _occupiedCells.Add(cellPosition);
                spawnedCount++;
            }
            attempts++;
        };

    }

    /// <summary>
    /// Verify if the cells are occupied or not
    /// </summary>
    /// <param name="cellPosition"></param>
    /// <param name="cellCenter"></param>
    /// <returns></returns>
    private bool IsValidSpawnPosition(Vector3Int cellPosition, Vector2 cellCenter)
    {
        if (_collisionTilemap.HasTile(cellPosition) || _occupiedCells.Contains(cellPosition))
        {
            return false;
        }

        Collider2D hit = Physics2D.OverlapBox(cellCenter, _grid.cellSize * 0.9f, 0, _obstacleLayer);
        if (hit != null)
        {
            return false;
        }

        return true;
    }
}
