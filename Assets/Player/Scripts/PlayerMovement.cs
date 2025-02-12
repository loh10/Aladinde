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
    public bool canMove;
    public Camera _cam;
    public GameObject spritePlayer;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _rb = GetComponent<Rigidbody2D>();
        canMove = false;
        GetStats();
    }

    private void GetStats()
    {
        float classSpeed = GetComponent<PlayerInfos>().characterClass.speed;
        Debug.Log("CharacterClass speed: " + classSpeed);
        _speed = classSpeed;
        _originalSpeed = classSpeed;
    }

    private void RotatePlayer()
    {
        Vector3 mousePosition = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Pour éviter tout problème de profondeur

        Vector3 direction = mousePosition - spritePlayer.transform.position;
        
        if (direction != Vector3.zero) // Vérification pour éviter NaN
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            spritePlayer.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }

    private void Update()
    {
        if (_rb == null)
            return;
        if (canMove)
        {
            _rb.linearVelocity = _move;
            RotatePlayer();
        }

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
