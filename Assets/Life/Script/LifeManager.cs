using System.Collections;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    [SerializeField] private PlayerInfos _playerInfos;

    public float maxHealth;
    public float currentHealth;
    public float _maxShield;
    public float currentShield;
    public bool _isBurning = false;

    void Start()
    {
        maxHealth = _playerInfos.characterClass.maxHealth;
        _maxShield = _playerInfos.characterClass.maxShield;
         
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
        print("pipi");
        StartCoroutine(ActivateShield(duration));
    }

    private IEnumerator ActivateShield(float duration)
    {
        currentShield = 100;
        print("caca");
        yield return new WaitForSeconds(duration);
        currentShield = 0;
    }

    public IEnumerator ApplyBurn(float burnDamage, float duration)
    {
        if (_isBurning) yield break;
        _isBurning = true;

        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            TakeDamage(burnDamage);
            timeElapsed += 1f;
            yield return new WaitForSeconds(1f);
        }

        _isBurning = false;
    }

    public void ReduceShield(float amount)
    {
        currentShield = Mathf.Max(0, currentShield - amount);
    }
}
