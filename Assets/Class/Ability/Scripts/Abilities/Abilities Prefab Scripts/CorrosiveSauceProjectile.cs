using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class CorrosiveSauceProjectile : NetworkBehaviour
{
    private Vector2 targetPosition;
    private float speed;
    private bool exploded = false;
    private GameObject owner;

    private ulong ownerClientId;
    
    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 3f;   // 3 m radius.
    [SerializeField] private float damage = 20f;           // 20 points of damage.
    [SerializeField] private float shieldReduction = 50f;  // Reduces enemy shields by 50.
    [SerializeField] private float cloudDuration = 4f;     // Cloud effect lasts 4 seconds.
    [SerializeField] private GameObject explosionEffectPrefab;

    public void Initialize(Vector2 targetPos, float projectileSpeed)
    {
        targetPosition = targetPos;
        speed = projectileSpeed;
    }

    public void SetOwner(GameObject ownerGameObject)
    {
        owner = ownerGameObject;
        ownerClientId = ownerGameObject.GetComponent<NetworkObject>()?.OwnerClientId ?? 0;
    
        Collider2D projCollider = GetComponent<Collider2D>();
        Collider2D ownerCollider = ownerGameObject.GetComponent<Collider2D>();
        if (projCollider != null && ownerCollider != null)
        {
            Physics2D.IgnoreCollision(projCollider, ownerCollider);
        }
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == owner) return;
        if (!exploded && NetworkManager.Singleton.IsServer)
            Explode();
    }

    private void Explode()
    {
        if (exploded) return;
        exploded = true;
        Debug.Log("CorrosiveSauceProjectile exploded at " + transform.position);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        TriggerExplosionEffectsClientRpc(transform.position);
        ApplyEffectsToEnemies();
        Invoke(nameof(DespawnSelf), cloudDuration);
    }

    [ClientRpc]
    private void TriggerExplosionEffectsClientRpc(Vector2 explosionPos)
    {
        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, explosionPos, Quaternion.identity);
    }

    private void ApplyEffectsToEnemies()
    {
        Debug.Log("CorrosiveSauceProjectile applying effects");
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, Physics2D.AllLayers);
        Debug.Log("CorrosiveSauceProjectile found " + hitColliders.Length + " colliders");
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                // Retrieve the NetworkObject from the collider's root GameObject.
                NetworkObject colliderNetObj = collider.transform.root.GetComponent<NetworkObject>();
                if (colliderNetObj != null && colliderNetObj.OwnerClientId == ownerClientId)
                {
                    // This collider belongs to the caster; skip applying effects.
                    continue;
                }
                
                Debug.Log("Applying corrosive effect to " + collider.name);
                PlayerLifeManager lifeManager = collider.GetComponent<PlayerLifeManager>();
                if (lifeManager != null)
                {
                    lifeManager.ReduceShield(shieldReduction);
                    lifeManager.ApplyDamage(damage);
                }
                else
                {
                    Debug.LogWarning("CorrosiveSauceProjectile: No PlayerLifeManager on " + collider.name);
                }
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
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
