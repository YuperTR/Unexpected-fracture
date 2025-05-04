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
    public float groundCheckRadius = 0.2f;

    //Abilities
    [Header("Abilities")]
    public bool canDoubleJump;
    public bool canDash;
    public bool canStrike;

    //States
    [Header("States")]
    public bool isJumping;
    public bool isRunning;
    public bool isCrouching;
    public bool isDashing;
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

        controls.Player.Jump.performed += ctx => isJumping = true;
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
        //Yürüme Mekaniði
        rb.linearVelocityX = moveInput.x * (isRunning ? 7f : 5f);
        animator.SetFloat("Speed", moveInput.sqrMagnitude); // Idle-Walk geçiþi için


        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void FixedUpdate()
    {
        //Saða Sola dönme mekaniði
        if (moveInput.x > 0)
        {
            isLookingRight = true;
            transform.localScale = new Vector3(4, 4, 1);
        }
        else if (moveInput.x < 0) {
            isLookingRight = false;
            transform.localScale = new Vector3(-4, 4, 1);
        }



        //Zýplama Mekaniði
        if (isJumping)
        {
            rb.linearVelocityY = jumpForce;
            isJumping = false;
        }

        if (isDashing)
        {
            // Dash logic
            isDashing = false;
        }
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
}


