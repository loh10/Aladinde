using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ThornTrap : MonoBehaviour, ITrapEffect
{
    [SerializeField] private float speedReduction;
    [SerializeField] private float damagePerSecond;
    private Coroutine damageCoroutine;
    private bool isIn;

    public void ApplyEffect(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        PlayerLifeManager life = player.GetComponent<PlayerLifeManager>();
        isIn = true;
        if (rb != null) rb.linearVelocity *= speedReduction;

        if (damageCoroutine == null && rb != null)
            damageCoroutine = StartCoroutine(DamageOverTime(rb, life));
    }

    public void RemoveEffect(GameObject player)
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
            isIn = false;
        }
    }

    private IEnumerator DamageOverTime(Rigidbody2D rb, PlayerLifeManager life)
    {
        while (isIn)
        {
            if (rb != null && life != null && rb.linearVelocity.magnitude > 0.1f)
            {
                NetworkObject networkObject = rb.gameObject.GetComponent<NetworkObject>();
                if (networkObject != null)
                {
                    life.TakeDamageServerRpc(damagePerSecond, networkObject.OwnerClientId);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
}