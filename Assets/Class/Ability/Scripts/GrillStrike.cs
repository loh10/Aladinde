using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "GrillStrikeAbility", menuName = "Scriptable Objects/Abilities/Grill Strike")]
public class GrillStrike : Ability
{
    private float _currentTime;
    private bool _canUse;

    public override void Activate(GameObject user)
    {
        base.Activate(user);
    }
}