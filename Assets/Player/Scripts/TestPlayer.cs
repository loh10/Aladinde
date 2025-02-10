using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    public float speed = 5f;
    [SerializeField] private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        
    }

    void Update()
    {
        // R�cup�ration des entr�es
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // Cr�ation du vecteur de mouvement
        movement = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        // Application du mouvement avec Rigidbody2D
        rb.linearVelocity = movement * speed;
    }
}
