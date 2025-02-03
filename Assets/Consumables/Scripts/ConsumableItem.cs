using UnityEngine;

public class ConsumableItem : MonoBehaviour
{
    [SerializeField] private Consumable _consumableData;

    public Consumable GetConsumableData()
    {
        return _consumableData;
    }
}
