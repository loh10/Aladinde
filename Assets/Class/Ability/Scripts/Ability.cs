using System;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Scriptable Objects/Ability")]
public class Ability : ScriptableObject
{
    public string abilityName;
    public string abilityDescription;
    public float cooldown;
    public float range;
    public float damages;
    public Vector2 mousePos;
    public Vector2 spawnTransform;

    public Sprite icon;
    public GameObject abilityPrefab;

    public virtual void Activate(GameObject user)
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)user.transform.position).normalized;
        RaycastHit2D[] hits = Physics2D.RaycastAll(user.transform.position, direction, range * 5);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject != user)
            {
                user.GetComponent<PlayerLifeManager>().TakeDamageServerRpc(damages, hit.collider.GetComponent<NetworkObject>().OwnerClientId);
            }
        }
    }
}