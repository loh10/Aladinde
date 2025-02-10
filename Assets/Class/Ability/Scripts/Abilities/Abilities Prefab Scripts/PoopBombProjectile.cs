using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class PoopBombProjectile : NetworkBehaviour
{
    private Vector2 targetPosition;
    private float speed;
    private bool exploded = false;
    private GameObject owner;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 2f;      // 2 meters radius.
    [SerializeField] private float slowDuration = 5f;         // Slow lasts 5 seconds.
    [SerializeField] private float slowMultiplier = 0.8f;     // 80% of original speed.
    [SerializeField] private GameObject explosionEffectPrefab;
    
    public void Initialize(Vector2 targetPos, float projectileSpeed)
    {
        targetPosition = targetPos;
        speed = projectileSpeed;
    }
    
    public void SetOwner(GameObject ownerGameObject)
    {
        owner = ownerGameObject;
        Collider2D projCollider = GetComponent<Collider2D>();
        Collider2D ownerCollider = owner.GetComponent<Collider2D>();
        if (projCollider != null && ownerCollider != null)
            Physics2D.IgnoreCollision(projCollider, ownerCollider);
    }
    
    private void Update()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (exploded) return;
        
        Vector2 currentPos = transform.position;
        Vector2 direction = (targetPosition - currentPos).normalized;
        float step = speed * Time.deltaTime;
        float remainingDistance = Vector2.Distance(currentPos, targetPosition);
        float threshold = 0.05f;
        
        if (remainingDistance <= threshold || step >= remainingDistance)
        {
            transform.position = targetPosition;
            Explode();
        }
        else
        {
            transform.position = currentPos + direction * step;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == owner) return;
        if (!exploded && NetworkManager.Singleton.IsServer)
            Explode();
    }
    
    private void Explode()
    {
        if (exploded) return;
        exploded = true;
        Debug.Log("PoopBombProjectile exploded at " + transform.position);
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;
        
        TriggerExplosionEffectsClientRpc(transform.position);
        ApplySlowToEnemies();
        Invoke(nameof(DespawnSelf), 1f);
    }
    
    [ClientRpc]
    private void TriggerExplosionEffectsClientRpc(Vector2 explosionPos)
    {
        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, explosionPos, Quaternion.identity);
    }
    
    private void ApplySlowToEnemies()
    {
        Debug.Log("PoopBombProjectile applying slow effect");
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        Debug.Log("PoopBombProjectile found " + hitColliders.Length + " colliders");
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Player") && collider.gameObject != owner)
            {
                Debug.Log("Applying slow to " + collider.name);
                PlayerMovement movement = collider.GetComponent<PlayerMovement>();
                if (movement != null)
                    movement.ApplyStaggerEffect(slowDuration, slowMultiplier);
                else
                    Debug.LogWarning("PoopBombProjectile: No PlayerMovement on " + collider.name);
            }
        }
    }
    
    private void DespawnSelf()
    {
        if (TryGetComponent<NetworkObject>(out NetworkObject netObj))
            netObj.Despawn();
        else
            Destroy(gameObject);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
