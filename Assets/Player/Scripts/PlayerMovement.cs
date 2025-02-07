using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    private Vector2 _move;
    private float _speed;
    private Rigidbody2D _rb;
    private float _originalSpeed; // to store the default speed

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

    // This is the local method that applies the stagger effect.
    public void ApplyStagger(float duration)
    {
        Debug.Log("Local ApplyStagger called");
        StartCoroutine(StaggerCoroutine(duration));
    }

    private IEnumerator StaggerCoroutine(float duration)
    {
        Debug.Log("Original speed: " + _originalSpeed);
        _speed = _originalSpeed * 0.1f; // reduce speed to 10%
        Debug.Log("Stagger applied, new speed: " + _speed);
        yield return new WaitForSeconds(duration);
        _speed = _originalSpeed;
        Debug.Log("Stagger ended, speed restored: " + _speed);
    }

    // NEW: ServerRpc that any client (even non-owners) can call on this object
    [ServerRpc(RequireOwnership = false)]
    public void ApplyStaggerServerRpc(float duration, ServerRpcParams rpcParams = default)
    {
        // On the server, call the ClientRpc to update all clients.
        ApplyStaggerClientRpc(duration);
    }

    // NEW: ClientRpc that tells all clients (including the owner) to apply the stagger locally
    [ClientRpc]
    private void ApplyStaggerClientRpc(float duration)
    {
        ApplyStagger(duration);
    }
}
