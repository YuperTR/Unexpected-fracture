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

    //Fine-Tuning
    [Header("Attributes")]
    public Vector2 moveInput;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float dashForce = 20f;
    public float dashTimer = 0.3f;
    public float runMultiplier = 1.6f;
    public float groundCheckRadius = 0.2f;

    //Abilities
    [Header("Abilities")]
    public bool canDash;
    public bool canStrike;

    //States
    [Header("States")]
    public bool isJumping;
    public bool isFalling;
    public bool isRunning;
    public bool isClimbing;
    public bool isHanging;
    public bool isCrouching;
    public bool isDashing;

    [Header("Mechanical States")]
    public bool isLookingRight;
    public bool isGrounded;
    public bool isTeleported;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();


        controls = new PlayerInputActions();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => Jump();
        controls.Player.Teleport.performed += ctx => Teleport();
        controls.Player.Crouch.performed += ctx => isCrouching = true;
        controls.Player.Crouch.canceled += ctx => isCrouching = false;

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

        //Yürüme Mekaniði
        rb.linearVelocityX = moveInput.x * (isRunning ? moveSpeed  * runMultiplier : moveSpeed);
        animator.SetFloat("Speed", moveInput.sqrMagnitude); // Idle-Walk geçiþi için


        //Yürüme ve Koþma arasýnda animasyon hýzý ayarlayýcý
        if(isRunning)
        {
            animator.speed = 1 * runMultiplier;
        } else
        {
            animator.speed = 1;
        }

        //Saða Sola dönme mekaniði
        if (moveInput.x > 0)
        {
            isLookingRight = true;
            transform.localScale = isCrouching ? new Vector3(5, 5, 1) : new Vector3(5, 5, 1); 
        }
        else if (moveInput.x < 0)
        {
            isLookingRight = false;
            transform.localScale = new Vector3(-5, 5, 1);
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

        if(isCrouching)
        {
            animator.SetBool("isCrouching", true);
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

    void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocityY = jumpForce;
            isJumping = true;
        }

    }

    void Dash()
    {
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer > 0)
            {
                rb.linearVelocityX = isLookingRight ? dashForce : dashForce * -1;
                animator.SetBool("isDashing", true);
            }
            else if (dashTimer < 0)
            {
                isDashing = false;
                animator.SetBool("isDashing", false);
                rb.linearVelocityX = 0;
                dashTimer = 0.3f;
            }
        }
        

    }

    void Crouch()
    {
        animator.SetBool("isCrouching", true);
    }
}


