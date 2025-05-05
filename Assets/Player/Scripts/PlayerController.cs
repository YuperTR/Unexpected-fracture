using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    //Components
    [Header("Components")]
    private PlayerInputActions controls;
    private Animator animator;
    private Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public GameObject deathScreen;
    //public Canvas playerUI;

    [Header("Health & Stamina")]
    public float playerHealth = 10f;
    public float playerStamina = 20f;

    //Fine-Tuning
    [Header("Attributes")]
    public Vector3 spawnPoint = new Vector3(-12.05f, -29.72f, 0f);
    public Vector2 moveInput;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float dashForce = 20f;
    public float dashTimer = 0.3f;
    public float dashCoolDown = 0.5f;
    public float runMultiplier = 1.6f;
    public float groundCheckRadius = 0.2f;
    public float cornerDetectionDistance = 0.1f;

    //Abilities
    [Header("Abilities")]
    public bool canDash = true;
    public bool canStrike;

    //States
    [Header("States")]
    public bool isAlive = true;
    public bool isJumping;
    public bool isFalling;
    public bool isRunning;
    public bool isClimbing;
    public bool isHanging;
    public bool isDashing;

    [Header("Mechanical States")]
    public bool isLookingRight;
    public bool isGrounded;
    public bool isTeleported;


    private void Awake()
    {

        DontDestroyOnLoad(gameObject);
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();


        controls = new PlayerInputActions();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => Jump();
        controls.Player.Teleport.performed += ctx => Teleport();

        controls.Player.Run.performed += ctx => isRunning = true;
        controls.Player.Run.canceled += ctx => isRunning = false;

        controls.Player.Dash.performed += ctx => isDashing = true;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {

    }

    void Update()
    {

        if (isGrounded)
        {
            isFalling = false;
            animator.SetBool("isFalling", false);
        }

        //Yürüme ve Koþma arasýnda animasyon hýzý ayarlayýcý
        if (isRunning && moveInput.x == 1 || moveInput.x == -1)
        {
            animator.speed = 1 * runMultiplier;

            if (playerStamina >= 0)
            {
                playerStamina -= Time.deltaTime;
            }

        }
        else
        {
            animator.speed = 1;

            if (playerStamina <= 20)
            {
                playerStamina += Time.deltaTime;
            }

        }

        if (playerHealth > 0)
        {


            //Yürüme Mekaniði
            rb.linearVelocityX = moveInput.x * (isRunning ? moveSpeed * runMultiplier : moveSpeed);
            animator.SetFloat("Speed", moveInput.sqrMagnitude); // Idle-Walk geçiþi için



            //Saða Sola dönme mekaniði
            if (moveInput.x > 0)
            {
                isLookingRight = true;
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (moveInput.x < 0)
            {
                isLookingRight = false;
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }



        //Zýplama Animasyon Mekaniði
        if (isJumping)
        {

            if (rb.linearVelocityY > 0)
            {
                animator.SetBool("isJumping", true);
            }
            else
            {
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", true);
                isFalling = true;
                isJumping = false;
            }
        }


        if (playerHealth <= 0)
        {
            deathScreen.SetActive(true);
            animator.SetBool("isDead", true);
        }
    }

    void FixedUpdate()
    {



        //Karakterin yerde olup olmadýðýný kontrol eden kontrolcü
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);


        Dash();

    }


    void Teleport()
    {
        if (playerHealth > 0)
        {
            if (!isTeleported)
            {
                transform.position = new Vector3(transform.position.x, (transform.position.y + 16f), transform.position.z);
                isTeleported = true;
            }
            else if (isTeleported)
            {
                transform.position = new Vector3(transform.position.x, (transform.position.y - 16f), transform.position.z);
                isTeleported = false;
            }
        }



    }


    //Checkpoint alma
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CheckPoint"))
        {
            spawnPoint = collision.transform.position;
        }
    }

    void Jump()
    {
        if (playerHealth > 0)
        {
            if (isGrounded)
            {
                rb.linearVelocityY = jumpForce;
                isJumping = true;
            }
        }

    }

    void Dash()
    {
        if (playerHealth > 0)
        {
            if (dashCoolDown >= 0)
            {
                dashCoolDown -= Time.deltaTime;
            }

            if (dashCoolDown <= 0)
            {
                canDash = true;
            }
            else if (dashCoolDown > 0)
            {
                canDash = false;
                isDashing = false;
            }




            if (isDashing && canDash)
            {
                dashTimer -= Time.deltaTime;
                if (dashTimer > 0)
                {
                    rb.linearVelocityX = isLookingRight ? dashForce : dashForce * -1;
                    animator.SetBool("isDashing", true);
                    playerStamina -= (Time.deltaTime * 10);

                }
                else if (dashTimer < 0)
                {
                    isDashing = false;
                    animator.SetBool("isDashing", false);
                    rb.linearVelocityX = 0;
                    dashTimer = 0.3f;
                    dashCoolDown = 1f;
                }
            }
        }



    }

    public void tryAgain()
    {
        playerHealth = 10f;
        animator.SetBool("isDead", false);
        transform.position = spawnPoint;
        deathScreen.SetActive(false);
    }


}


