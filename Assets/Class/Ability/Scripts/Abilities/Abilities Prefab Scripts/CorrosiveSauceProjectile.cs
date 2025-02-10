using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class CorrosiveSauceProjectile : NetworkBehaviour
{
    private Vector2 targetPosition;
    private float speed;
    private bool exploded = false;
    private GameObject owner;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 3f;   // Zone d'effet: 3 m.
    [SerializeField] private float damage = 20f;           // Deals 20 points of damage.
    [SerializeField] private float shieldReduction = 50f;  // Reduces enemy shields by 50 points.
    [SerializeField] private float cloudDuration = 4f;     // Explosion effect lasts 4 seconds.

    // (Optional) Explosion effect prefab.
    [SerializeField] private GameObject explosionEffectPrefab;

    /// <summary>
    /// Initializes the projectile with its target position and speed.
    /// </summary>
    public void Initialize(Vector2 targetPos, float projectileSpeed)
    {
        targetPosition = targetPos;
        speed = projectileSpeed;
    }

    /// <summary>
    /// Sets the owner so that the projectile does not affect its thrower.
    /// </summary>
    public void SetOwner(GameObject ownerGameObject)
    {
        owner = ownerGameObject;
        Collider2D projCollider = GetComponent<Collider2D>();
        Collider2D ownerCollider = owner.GetComponent<Collider2D>();
        if (projCollider != null && ownerCollider != null)
        {
            Physics2D.IgnoreCollision(projCollider, ownerCollider);
        }
    }

    private void Update()
    {
        // Only the server drives the projectile’s movement.
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
        if (collision.gameObject == owner)
            return;
        if (!exploded && NetworkManager.Singleton.IsServer)
            Explode();
    }

    private void Explode()
    {
        if (exploded) return;
        exploded = true;
        Debug.Log("Corrosive Sauce exploded at " + transform.position);

        // Disable collider so no further collisions are processed.
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // Trigger explosion effects on clients.
        TriggerExplosionEffectsClientRpc(transform.position);
        // Apply the corrosive effects (shield reduction and damage) to enemy players.
        ApplyEffectsToEnemies();
        // Despawn the projectile after a delay.
        Invoke(nameof(DespawnSelf), cloudDuration);
    }

    [ClientRpc]
    private void TriggerExplosionEffectsClientRpc(Vector2 explosionPos)
    {
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, explosionPos, Quaternion.identity);
        }
    }

    /// <summary>
    /// Finds enemy players within the explosion radius (except the owner) and applies the corrosive effect.
    /// </summary>
    private void ApplyEffectsToEnemies()
    {
        Debug.Log("Corrosive Sauce applying effects");
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Player") && collider.gameObject != owner)
            {
                Debug.Log("Applying corrosive effect to " + collider.name);
                PlayerLifeManager lifeManager = collider.GetComponent<PlayerLifeManager>();
                NetworkObject netObj = collider.GetComponent<NetworkObject>();
                if (lifeManager != null && netObj != null)
                {
                    // First, reduce shield by the specified amount.
                    lifeManager.ReduceShield(shieldReduction);
                    // Then, apply damage.
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
