using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Spawner : MonoBehaviour
{
    [Header("Paramètres de spawn")]
    [SerializeField] private Consumable[] _scriptableObjects;

    [SerializeField] private int _spawnCount = 10;

    [Header("Zone de spawn")]
    [SerializeField] private Vector2 _spawnAreaSize = new Vector2(15f, 15f);

    [Header("Collision Settings")]
    [SerializeField] private LayerMask _collisionLayer;

    [SerializeField] private float _checkRadius = 0.5f;
    [SerializeField] private int _maxAttemptsPerObject = 20;

    private void Start()
    {
        SpawnObjects();
    }

    private void SpawnObjects()
    {
        for (int i = 0; i < _spawnCount; i++)
        {
            Vector2 spawnPosition;
            bool foundValidPosition = TryGetValidSpawnPosition(out spawnPosition);
            
            if (!foundValidPosition)
            {
                Debug.LogWarning("Impossible de trouver une position valide après plusieurs tentatives.");
                continue;
            }
            
            Consumable randomData = _scriptableObjects[Random.Range(0, _scriptableObjects.Length)];

            Instantiate(randomData.prefab, spawnPosition, Quaternion.identity);
        }
    }

    private bool TryGetValidSpawnPosition(out Vector2 validPosition)
    {
        for (int attempt = 0; attempt < _maxAttemptsPerObject; attempt++)
        {
            Vector2 randomPosition = GenerateRandomPositionInArea();

            if (!IsPositionColliding(randomPosition))
            {
                validPosition = randomPosition;
                return true;
            }
        }

        validPosition = Vector2.zero;
        return false;
    }
    
    private Vector2 GenerateRandomPositionInArea()
    {
        float randomX = Random.Range(transform.position.x - _spawnAreaSize.x / 2,
            transform.position.x + _spawnAreaSize.x / 2);
        float randomY = Random.Range(transform.position.y - _spawnAreaSize.y / 2,
            transform.position.y + _spawnAreaSize.y / 2);

        return new Vector2(randomX, randomY);
    }

    private bool IsPositionColliding(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, _checkRadius, _collisionLayer);
        return (hit != null);
    }
    
    private void OnDrawGizmos()
    {
        // Définis la couleur 
        Gizmos.color = new Color(1f, 0f, 0f, 0.2f); // Rouge translucide

        Vector3 center = transform.position;
        Vector3 size = new Vector3(_spawnAreaSize.x, _spawnAreaSize.y, 1f);

        Gizmos.DrawCube(center, size);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }
}