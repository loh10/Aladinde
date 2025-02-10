using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class PlayerMovement : NetworkBehaviour
{
    private Vector2 _move;
    private float _speed;
    private Rigidbody2D _rb;
    [FormerlySerializedAs("canMoove")] public bool canMove;

    public override void OnNetworkSpawn()
    {
        gameObject.name = "Player "+OwnerClientId.ToString();
        canMove = false;
        if (!IsOwner)
        {
            Debug.Log(gameObject.name + " is connected");
            enabled = false;
            GetComponent<PlayerInput>().enabled = false;
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
            GetComponent<SpriteRenderer>().color = Color.red;
            GetComponent<PlayerLifeManager>().enabled = false;
            return;
        }
        _rb = GetComponent<Rigidbody2D>();
        GetComponentInChildren<Camera>().tag = "MainCamera";
        gameObject.layer = 2;
        GetStats();
    }

    private void GetStats()
    {
        _speed = GetComponent<PlayerInfos>().characterClass.speed;
    }

    private void Update()
    {
        if(canMove)
            _rb.linearVelocity = _move;
    }

    public void MovementPlayer(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        _move = new Vector2(input.x, input.y) * _speed;
    }

}
