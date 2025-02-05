using System.Collections.Generic;
using UnityEngine;

public interface ITrapEffect
{
    void ApplyEffect(GameObject player);
    void RemoveEffect(GameObject player);
}

public class TrapHandler : MonoBehaviour
{
    private HashSet<ITrapEffect> activeTraps = new HashSet<ITrapEffect>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        ITrapEffect trap = other.GetComponent<ITrapEffect>();
        if (trap != null)
        {
            trap.ApplyEffect(gameObject);
            activeTraps.Add(trap);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        ITrapEffect trap = other.GetComponent<ITrapEffect>();
        if (trap != null && activeTraps.Contains(trap))
        {
            trap.RemoveEffect(gameObject);
            activeTraps.Remove(trap);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        GrassTrap grassTrap = other.GetComponent<GrassTrap>();
        if (grassTrap != null)
        {
            grassTrap.UpdateTransparency(gameObject);
        }
    }
}
