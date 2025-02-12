using UnityEngine;

public class TrophiesManager : MonoBehaviour
{
    public int currentTrophies;

    public void UpdateTrophies(int amount)
    {
        currentTrophies += amount;
    }
}
