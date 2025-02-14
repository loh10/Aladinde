using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class SpicyBombProjectile : NetworkBehaviour
{
    private Vector2 targetPosition;
    private float speed;
    private bool exploded = false;
    private GameObject owner;
    
    private ulong ownerClientId;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float immediateDamage = 20f;
    [SerializeField] private float burnDamagePerSecond = 5f;
    [SerializeField] private float burnDuration = 3f;    // seconds of burn effect
    [SerializeField] private float cloudDuration = 4f;   // seconds explosion cloud persists
    [SerializeField] private GameObject explosionEffectPrefab;
    
    public void Initialize(Vector2 targetPos, float projectileSpeed)
    {
        targetPosition = targetPos;
        speed = projectileSpeed;
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
        Debug.Log("SpicyBombProjectile exploded at " + transform.position);

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
            Instantiate(explosionEffectPrefab, explosionPos, Quaternion.identity);
    }

    private IEnumerator ExplosionCoroutine()
    {
        Debug.Log("SpicyBombProjectile explosion coroutine started");
        ApplyDamageToPlayersRpc(immediateDamage);

        float elapsed = 0f;
        while (elapsed < burnDuration)
        {
            ApplyDamageToPlayersRpc(burnDamagePerSecond);
            yield return new WaitForSeconds(1f);
            elapsed += 1f;
        }
    }
    
    private void ApplyDamageToPlayersRpc(float damage)
    {
        Debug.Log("SpicyBombProjectile attempting to apply damage: " + damage);
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, Physics2D.AllLayers);
        Debug.Log("SpicyBombProjectile found " + hitColliders.Length + " colliders");
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
                
                Debug.Log("SpicyBombProjectile dealing damage to " + collider.name);
                PlayerLifeManager lifeManager = collider.GetComponent<PlayerLifeManager>();
                if (lifeManager != null)
                    lifeManager.ApplyDamage(damage);
                else
                    Debug.LogWarning("SpicyBombProjectile: No PlayerLifeManager on " + collider.name);
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
