using System.Collections;
using UnityEngine;

public class ThornTrap : MonoBehaviour, ITrapEffect
{
    [SerializeField] private float speedReduction;
    [SerializeField] private float damagePerSecond;
    private Coroutine damageCoroutine;

    public void ApplyEffect(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        LifeManager life = player.GetComponent<LifeManager>();

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
        }
    }

    private IEnumerator DamageOverTime(Rigidbody2D rb, LifeManager life)
    {
        while (true)
        {
            if (rb != null && life != null && rb.linearVelocity.magnitude > 0.1f)
            {
                life.TakeDamage(damagePerSecond);
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
