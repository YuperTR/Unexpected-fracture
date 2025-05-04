using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 10; // Amount of damage the projectile deals

    // --- Option 1: Using Triggers (Collider2D set to IsTrigger = true) ---
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the projectile collided with the Player
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemy projectile hit Player!");
            Destroy(gameObject); // Destroy the projectile after hitting the player
        }
       
    }

    // --- Option 2: Using Collisions (Collider2D set to IsTrigger = false) ---
    /*
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the projectile collided with the Player
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemy projectile hit Player!");
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject); // Destroy the projectile
        }
        else if (!collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("ProjectileTag"))
        {
             Debug.Log("Enemy projectile hit something else: " + collision.gameObject.name);
             Destroy(gameObject); // Destroy the projectile
        }
    }
    */

    // Note: If using collisions, you might want the projectile to bounce off walls.
    // If using triggers, it will pass through unless you destroy it on collision.
}