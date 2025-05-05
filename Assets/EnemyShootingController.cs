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
    private bool isFacingRight = true; // Tracks the enemy's current facing direction

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

        // Initialize facing direction based on initial scale if needed
        // If your enemy sprite starts facing left, set isFacingRight = false;
        // and ensure transform.localScale.x is initially negative.
        // We assume the default sprite faces right and scale.x is positive.
        if (transform.localScale.x < 0)
        {
            isFacingRight = false;
        }
    }

    void Update()
    {
        if (playerTransform == null) return; // Don't do anything if we haven't found the player

        // --- Detection Logic ---
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {
            // --- Flipping Logic ---
            CheckDirectionAndFlip(); // Call the new flipping method

            // --- Aiming Logic ---
            Vector2 directionToPlayer = (playerTransform.position - firePoint.position).normalized;
            // Optional: Rotate the firePoint or enemy to face the player
            // float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            // Adjust angle calculation based on whether enemy is flipped if rotating the whole enemy
            // firePoint.rotation = Quaternion.Euler(0f, 0f, angle); // Simplest rotation towards target

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

    void CheckDirectionAndFlip()
    {
        if (playerTransform == null) return;

        // Determine direction TO player horizontally
        float horizontalDirectionToPlayer = playerTransform.position.x - transform.position.x;

        // If player is to the right (positive direction) AND enemy is facing left
        if (horizontalDirectionToPlayer > 0 && !isFacingRight)
        {
            Flip();
        }
        // If player is to the left (negative direction) AND enemy is facing right
        else if (horizontalDirectionToPlayer < 0 && isFacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        // Switch the facing direction flag
        isFacingRight = !isFacingRight;

        // Multiply the x component of localScale by -1 to flip
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;

        // Optional: If flipping the FirePoint is needed separately
        // If the FirePoint is a child, its local position might implicitly flip.
        // If aiming depends on firePoint's local rotation, you might need to adjust it here too.
    }


    void Shoot(Vector2 direction)
    {
        if (projectilePrefab == null || firePoint == null) return;

        // Instantiate the projectile at the firePoint's position and rotation
        // Using firePoint.rotation might be less ideal if the enemy flips via scale.
        // Instantiating with Quaternion.identity might be safer unless firePoint rotation is handled carefully.
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity); // Use Quaternion.identity for neutral rotation

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Apply force directly towards the calculated player direction
            rb.linearVelocity = direction * projectileSpeed;
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