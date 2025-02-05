using UnityEngine;

public class MapBorder : MonoBehaviour
{
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float damage = 5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            LifeManager life = collision.gameObject.GetComponent<LifeManager>();
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (life != null)
            {
                life.TakeDamage(damage);
            }

            if (rb != null)
            {
                Vector2 forceDirection = (collision.transform.position - transform.position).normalized;
                rb.AddForce(forceDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }        
    }
}
