using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class SpicyBombProjectile : NetworkBehaviour
{
    private Vector2 targetPosition;
    private float speed;
    private bool exploded = false;
    private GameObject owner;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float immediateDamage = 30f;
    [SerializeField] private float burnDamagePerSecond = 5f;
    [SerializeField] private float burnDuration = 3f;    // seconds of burn effect
    [SerializeField] private float cloudDuration = 4f;   // seconds explosion cloud persists

    // Optional: a prefab or particle system for explosion effects.
    [SerializeField] private GameObject explosionEffectPrefab;
    
    public void Initialize(Vector2 targetPos, float projectileSpeed)
    {
        targetPosition = targetPos;
        speed = projectileSpeed;
    }

    private void Update()
    {
        // Only the server should drive the explosion
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

    public void SetOwner(GameObject ownerGameObject)
    {
        owner = ownerGameObject;
        // Optionally, tell the physics engine to ignore collisions between the projectile and its owner:
        Collider2D projCollider = GetComponent<Collider2D>();
        Collider2D ownerCollider = owner.GetComponent<Collider2D>();
        if (projCollider != null && ownerCollider != null)
        {
            Physics2D.IgnoreCollision(projCollider, ownerCollider);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Skip collisions with the owner.
        if (collision.gameObject == owner)
            return;
    
        Debug.Log("Projectile collided with " + collision.gameObject.name);
    
        if (!exploded && NetworkManager.Singleton.IsServer)
            Explode();
    }

    private void Explode()
    {
        // Only run the explosion logic once.
        if (exploded) return;
        exploded = true;
        Debug.Log("Explosion method called");

        // (Optional) Disable your collider so you won’t get further collision events.
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        TriggerExplosionEffectsClientRpc(transform.position);
        StartCoroutine(ExplosionCoroutine());
        Invoke(nameof(DespawnSelf), cloudDuration);
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

    private IEnumerator ExplosionCoroutine()
    {
        Debug.Log("Explosion coroutine started");
        
        // Immediately apply explosion damage.
        ApplyDamageToPlayers(immediateDamage);

        // Apply burning damage once per second.
        float elapsed = 0f;
        while (elapsed < burnDuration)
        {
            ApplyDamageToPlayers(burnDamagePerSecond);
            yield return new WaitForSeconds(1f);
            elapsed += 1f;
        }
    }

    private void ApplyDamageToPlayers(float damage)
    {
        Debug.Log("Attempting to apply damage to players");
        
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                Debug.Log("Player detected: " + collider.name);
                
                // Here we call the networked damage method.
                PlayerLifeManager lifeManager = collider.GetComponent<PlayerLifeManager>();
                NetworkObject netObj = collider.GetComponent<NetworkObject>();
                if (lifeManager != null && netObj != null)
                {
                    Debug.Log("Dealing damage to " + collider.name);
                    lifeManager.TakeDamageServerRpc(damage, netObj.OwnerClientId);
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
