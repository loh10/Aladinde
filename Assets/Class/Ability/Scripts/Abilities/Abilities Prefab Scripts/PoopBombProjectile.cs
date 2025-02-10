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
    [SerializeField] private float explosionRadius = 2f;      // As per GDD: 2 meters radius.
    [SerializeField] private float slowDuration = 5f;         // Slow lasts 5 seconds.
    [SerializeField] private float slowMultiplier = 0.8f;       // 80% of original speed (20% reduction).
    
    // (Optional) Explosion effect prefab.
    [SerializeField] private GameObject explosionEffectPrefab;
    
    /// <summary>
    /// Initialize the projectile with its target and speed.
    /// </summary>
    public void Initialize(Vector2 targetPos, float projectileSpeed)
    {
        targetPosition = targetPos;
        speed = projectileSpeed;
    }
    
    /// <summary>
    /// Set the owner of this projectile so that it won’t affect its thrower.
    /// </summary>
    public void SetOwner(GameObject ownerGameObject)
    {
        owner = ownerGameObject;
        // Instruct the physics system to ignore collisions between the projectile and its owner.
        Collider2D projCollider = GetComponent<Collider2D>();
        Collider2D ownerCollider = owner.GetComponent<Collider2D>();
        if (projCollider != null && ownerCollider != null)
        {
            Physics2D.IgnoreCollision(projCollider, ownerCollider);
        }
    }
    
    private void Update()
    {
        // Only the server should drive the projectile movement.
        if (!NetworkManager.Singleton.IsServer) return;
        if (exploded) return;
        
        Vector2 currentPos = transform.position;
        Vector2 direction = (targetPosition - currentPos).normalized;
        float step = speed * Time.deltaTime;
        float remainingDistance = Vector2.Distance(currentPos, targetPosition);
        float threshold = 0.05f; // small threshold to account for floating-point imprecision
        
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
        // Do not trigger explosion if colliding with the owner.
        if (collision.gameObject == owner)
            return;
        if (!exploded && NetworkManager.Singleton.IsServer)
            Explode();
    }
    
    private void Explode()
    {
        if (exploded) return;
        exploded = true;
        Debug.Log("Poop Bomb exploded at " + transform.position);
        
        // Optionally disable the collider so no further collisions occur.
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;
        
        // Trigger explosion effects on all clients.
        TriggerExplosionEffectsClientRpc(transform.position);
        
        // Apply the slow effect to enemy players.
        ApplySlowToEnemies();
        
        // Despawn the projectile after a short delay.
        Invoke(nameof(DespawnSelf), 1f);
    }
    
    [ClientRpc]
    private void TriggerExplosionEffectsClientRpc(Vector2 explosionPos)
    {
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, explosionPos, Quaternion.identity);
        }
        // (Optional) Play explosion sounds here.
    }
    
    /// <summary>
    /// Finds any players (except the owner) within the explosion radius and applies a slow effect.
    /// </summary>
    private void ApplySlowToEnemies()
    {
        Debug.Log("Poop Bomb applying slow effect");
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Player") && collider.gameObject != owner)
            {
                Debug.Log("Applying slow to " + collider.name);
                // Get the PlayerMovement component and use its server RPC to apply the slow.
                PlayerMovement movement = collider.GetComponent<PlayerMovement>();
                if (movement != null)
                {
                    movement.ApplyStaggerServerRpc(slowDuration, slowMultiplier);
                }
            }
        }
    }
    
    private void DespawnSelf()
    {
        if (TryGetComponent<NetworkObject>(out NetworkObject netObj))
        {
            netObj.Despawn();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // For debugging in the Scene view.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
