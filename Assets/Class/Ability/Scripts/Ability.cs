using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Scriptable Objects/Ability")]
public class Ability : ScriptableObject
{
    public string abilityName;
    public string abilityDescription;
    public float cooldown;
    public float range;
    public float damages;

    public Sprite icon;
    public GameObject abilityPrefab;

    public virtual void Activate(GameObject user)
    {
        Debug.Log(abilityName + "  triggered by  " + user.name);
    }
}
