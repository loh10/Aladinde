using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class PlayerMovement : NetworkBehaviour
{
    private Vector2 _move;
    private float _speed;

    public override void OnNetworkSpawn()
    {
        gameObject.name = "Player "+OwnerClientId.ToString();
        if (!IsOwner)
        {
            Debug.Log("here");
            enabled = false;
            GetComponent<PlayerInput>().enabled = false;
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
            GetComponent<SpriteRenderer>().color = Color.red;
            GetComponent<PlayerLifeManager>().enabled = false;
            return;
        }
        GetStats();
    }

    private void GetStats()
    {
        _speed = GetComponent<PlayerInfos>().characterClass.speed;
    }

    private void Update()
    {
        transform.Translate(_move);
    }

    public void MovementPlayer(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        _move = new Vector2(input.x, input.y) * _speed * Time.deltaTime;
    }
}
