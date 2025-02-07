using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    private Vector2 _move;
    private float _speed;
    private Rigidbody2D _rb;
    private float _originalSpeed;

    public override void OnNetworkSpawn()
    {
        gameObject.name = "Player " + OwnerClientId.ToString();
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
        float classSpeed = GetComponent<PlayerInfos>().characterClass.speed;
        Debug.Log("CharacterClass speed: " + classSpeed);
        _speed = classSpeed;
        _originalSpeed = classSpeed;
    }

    private void Update()
    {
        _rb.linearVelocity = _move;
    }

    public void MovementPlayer(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        _move = new Vector2(input.x, input.y) * _speed;
    }

    // Applies a stagger effect by temporarily reducing the speed,
    public void ApplyStagger(float duration, float speedMultiplier)
    {
        // Debug.Log("Local ApplyStagger called with multiplier " + speedMultiplier);
        StartCoroutine(StaggerCoroutine(duration, speedMultiplier));
    }

    private IEnumerator StaggerCoroutine(float duration, float speedMultiplier)
    {
        // Debug.Log("Original speed: " + _originalSpeed);
        _speed = _originalSpeed * speedMultiplier;
        // Debug.Log("Stagger applied, new speed: " + _speed);
        yield return new WaitForSeconds(duration);
        _speed = _originalSpeed;
        // Debug.Log("Stagger ended, speed restored: " + _speed);
    }

    // ServerRpc (non-ownership required) to apply stagger remotely.
    [ServerRpc(RequireOwnership = false)]
    public void ApplyStaggerServerRpc(float duration, float speedMultiplier, ServerRpcParams rpcParams = default)
    {
        // Propagate to all clients.
        ApplyStaggerClientRpc(duration, speedMultiplier);
    }

    // ClientRpc to tell all clients (including the owner) to apply the stagger locally.
    [ClientRpc]
    private void ApplyStaggerClientRpc(float duration, float speedMultiplier)
    {
        ApplyStagger(duration, speedMultiplier);
    }
}