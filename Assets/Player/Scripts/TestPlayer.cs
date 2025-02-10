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
        // Récupération des entrées
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // Création du vecteur de mouvement
        movement = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        // Application du mouvement avec Rigidbody2D
        rb.linearVelocity = movement * speed;
    }
}
