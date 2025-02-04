using System.Collections;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    private PlayerInfos _playerInfos;

    public float maxHealth;
    public float currentHealth;
    public float maxShield;
    public float currentShield;
    public bool isBurning = false;

    private void Start()
    {
        _playerInfos = GetComponent<PlayerInfos>();
        maxHealth = _playerInfos.characterClass.maxHealth;
        maxShield = _playerInfos.characterClass.maxShield;

        currentHealth = maxHealth;
    }


    public void TakeDamage(float damage)
    {
        if(currentShield > 0)
        {
            float shieldDamage = Mathf.Min(currentShield, damage);
            currentShield -= shieldDamage;
            damage -= shieldDamage;
        }

        if(damage > 0)
        {
            currentHealth -= damage;
        }

        if(currentHealth <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        Debug.Log(gameObject.name + " is dead");
        Destroy(gameObject);
    }

    public void ActiShield(float duration)
    {
        StartCoroutine(ActivateShield(duration));
    }

    private IEnumerator ActivateShield(float duration)
    {
        currentShield = 100;
        yield return new WaitForSeconds(duration);
        currentShield = 0;
    }

    public IEnumerator ApplyBurn(float burnDamage, float duration)
    {
        if (isBurning) yield break;
        isBurning = true;

        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            TakeDamage(burnDamage);
            timeElapsed += 1f;
            yield return new WaitForSeconds(1f);
        }

        isBurning = false;
    }

    public void ReduceShield(float amount)
    {
        currentShield = Mathf.Max(0, currentShield - amount);
    }
}
