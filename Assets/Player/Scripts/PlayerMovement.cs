using System.Collections;
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
    private float _originalSpeed;

    [FormerlySerializedAs("canMoove")] public bool canMove;

    public override void OnNetworkSpawn()
    {
        gameObject.name = "Player "+OwnerClientId.ToString();
        canMove = false;
        if (!IsOwner)
        {
            Debug.Log(gameObject.name + " is connected (non-owner client)");
            GetComponent<PlayerInput>().enabled = false;
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
            GetComponent<SpriteRenderer>().color = Color.red;
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
        if (_rb == null)
            return;
        if(canMove)
            _rb.linearVelocity = _move;

    }

    public void MovementPlayer(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        _move = new Vector2(input.x, input.y) * _speed;
    }

    public void ApplyStagger(float duration, float speedMultiplier)
    {
        StartCoroutine(StaggerCoroutine(duration, speedMultiplier));
    }

    private IEnumerator StaggerCoroutine(float duration, float speedMultiplier)
    {
        _speed = _originalSpeed * speedMultiplier;
        yield return new WaitForSeconds(duration);
        _speed = _originalSpeed;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ApplyStaggerServerRpc(float duration, float speedMultiplier, ServerRpcParams rpcParams = default)
    {
        ApplyStaggerClientRpc(duration, speedMultiplier);
    }

    [ClientRpc]
    private void ApplyStaggerClientRpc(float duration, float speedMultiplier)
    {
        ApplyStagger(duration, speedMultiplier);
    }

    // *** NEW: Directly apply a stagger effect from the server ***
    public void ApplyStaggerEffect(float duration, float speedMultiplier)
    {
        // Apply locally on the server.
        ApplyStagger(duration, speedMultiplier);
        // Propagate the effect to all clients.
        ApplyStaggerClientRpc(duration, speedMultiplier);
    }
}
