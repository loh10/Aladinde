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

    [SerializeField] private GameObject explosionEffectPrefab;
    
    public void Initialize(Vector2 targetPos, float projectileSpeed)
    {
        targetPosition = targetPos;
        speed = projectileSpeed;
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
        Debug.Log("Explosion method called");

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
                PlayerLifeManager lifeManager = collider.GetComponent<PlayerLifeManager>();
                if (lifeManager != null)
                {
                    Debug.Log("Dealing damage to " + collider.name);
                    // Call the local damage method directly.
                    lifeManager.ApplyDamage(damage);
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
