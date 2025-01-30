using UnityEngine;

public class PlayerCheckCollisions : MonoBehaviour
{
    private PlayerInfos _playerInfos;
    private LifeManager _lifeManager;
    private UltimateCharge _ultimateCharge;

    private void Start()
    {
        _playerInfos = GetComponent<PlayerInfos>();
        _lifeManager = GetComponent<LifeManager>();
        _ultimateCharge = GetComponent<UltimateCharge>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ConsumableItem consumable = other.GetComponent<ConsumableItem>();
        if (consumable != null)
        {
            Consumable data = consumable.GetConsumableData();
            data.ApplyEffect(_playerInfos, _lifeManager, _ultimateCharge);

            Debug.Log(data.consumableName + " consumed!");
            Destroy(other.gameObject);
        }
    }
}