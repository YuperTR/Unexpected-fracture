using UnityEngine;

public class EnemyShootingController : MonoBehaviour
{
    public GameObject projectilePrefab; // Assign the EnemyProjectile Prefab in the Inspector
    public Transform firePoint;         // Assign the FirePoint child object in the Inspector
    public float fireRate = 1f;         // How often the enemy shoots (shots per second)
    public float projectileSpeed = 10f; // How fast the projectile moves
    public float detectionRange = 10f; // How far the enemy can 'see' the player

    private float nextFireTime = 0f;
    private Transform playerTransform;

    void Start()
    {
        // Find the player GameObject by tag. Make sure your player has the "Player" tag.
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player GameObject has the 'Player' tag.");
            enabled = false; // Disable the script if player isn't found
        }

        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile Prefab not assigned on " + gameObject.name);
            enabled = false;
        }
        if (firePoint == null)
        {
            Debug.LogError("Fire Point not assigned on " + gameObject.name);
            enabled = false;
        }
    }

    void Update()
    {
        if (playerTransform == null) return; // Don't do anything if we haven't found the player

        // --- Detection Logic ---
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {
            // --- Aiming Logic (Optional: Simple aim towards player) ---
            Vector2 directionToPlayer = (playerTransform.position - firePoint.position).normalized;
            // Optional: Rotate the firePoint or enemy to face the player
            // float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            // firePoint.rotation = Quaternion.Euler(0f, 0f, angle - 90f); // Adjust angle offset if needed

            // --- Shooting Logic ---
            // Check if enough time has passed since the last shot
            if (Time.time >= nextFireTime)
            {
                Shoot(directionToPlayer);
                // Set the time for the next allowed shot
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
    }

    void Shoot(Vector2 direction)
    {
        if (projectilePrefab == null || firePoint == null) return;

        // Instantiate the projectile at the firePoint's position and rotation
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation); // Use firePoint rotation for basic forward shooting

        // Get the Rigidbody2D component from the instantiated projectile
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Apply force to the projectile
            // Use the calculated direction to shoot towards the player
            rb.linearVelocity = direction * projectileSpeed;

            // Alternative: Shoot straight based on firePoint's forward direction
            // rb.velocity = firePoint.up * projectileSpeed; // Assuming 'up' is forward in your 2D setup
        }
        else
        {
            Debug.LogError("Projectile Prefab is missing Rigidbody2D component!");
        }

        // Optional: Destroy the projectile after some time if it doesn't hit anything
        Destroy(projectile, 5f); // Destroy after 5 seconds
    }

    // Optional: Visualize the detection range in the Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}