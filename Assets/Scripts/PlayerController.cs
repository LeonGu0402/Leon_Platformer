using System.Security.Cryptography.X509Certificates;
using Unity.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    public enum FacingDirection
    {
        left, right
    }
    //public Transform groundCheck;

    [Header("Horizontal")]
    public float maxSpeed;
    public float accelerationTime;
    public float decelerationTime;
    public float dashForce;
    public float dashTime;

    [Header("Vertical")]
    public float apexHeight;
    public float apexTime;
    public float terminalSpeed;
    public float coyoteTime;

    [Header("Ground Check")]
    public bool isGround;
    public float groundCheckOffset = 0.5f;
    public Vector2 groundCheckSize = new Vector2(0.4f, 0.1f);
    public LayerMask groundCheckMask;


    private Vector2 playerInput;
    private FacingDirection currentdirection = FacingDirection.right;
    private float acceleration;
    private float deceleration;
    private Vector2 velocity;
    //private float maxSpeedSqr;
    //private bool isJump;
    private float Gravity;
    private float jumpSpeed;
    private float airTime;
    private float fallingTimer;
    private float dashTimer = 0;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //maxSpeedSqr = maxSpeed * maxSpeed;
    }

    void Update()
    {
        //horizontalMove = Input.GetAxisRaw("Horizontal");
        //Vector2 playerInput = new Vector2(horizontalMove, 0);
        //MovementUpdate(playerInput);
        //Debug.Log(rb.velocity);
        //Debug.Log(fallingTimer);
        CheckForGround();

        acceleration = maxSpeed / accelerationTime;
        deceleration = maxSpeed / decelerationTime;
        Gravity = -2 * apexHeight / (apexTime * apexTime);
        jumpSpeed = 2 * apexHeight / apexTime;


        rb.gravityScale = 0;

        playerInput.x = Input.GetAxisRaw("Horizontal");

        Dash();
        MovementUpdate(playerInput);
        JumpUpdate();
        
        //applying gravity
        if (!isGround)
        {
            velocity.y += Gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = 0;
        }

        rb.velocity = velocity;

    }

    private void FixedUpdate()
    {
    }


    //old movement
    //private void MovementUpdate(Vector2 playerInput)
    //{
    //    //rb.velocity = new Vector2(playerInput.x * speed, 0);
    //    rb.velocity = new Vector2(playerInput.x * speed, 0);

    //    //if (horizontalMove > 0 || horizontalMove < 0)
    //    //{
    //    //    speed += acceleration * Time.deltaTime;
    //    //    if (speed > maxSpeed)
    //    //    {
    //    //        speed = maxSpeed;
    //    //    }
    //    //}

    //    //if (horizontalMove == 0)
    //    //{
    //    //    speed -= deceleration * Time.deltaTime;
    //    //    if (speed < 0)
    //    //    {
    //    //        speed = 0;
    //    //    }
    //    //}

    //}

    //new Movement function
    //private void MovementUpdate()
    //{
    //    Vector2 moveDirection = Vector2.zero;

    //    if (Input.GetKey(KeyCode.A))
    //    {
    //        moveDirection += Vector2.left;
    //    }
    //    if (Input.GetKey(KeyCode.D))
    //    {
    //        moveDirection += Vector2.right;
    //    }

    //    if (moveDirection.sqrMagnitude > 0)
    //    {
    //        velocity += Time.deltaTime * acceleration * moveDirection;
    //        if (velocity.sqrMagnitude > maxSpeedSqr)
    //        {
    //            velocity = velocity.normalized * maxSpeed;
    //        }
    //    }
    //    else
    //    {
    //        Vector2 velocityDelta = Time.deltaTime * deceleration * velocity.normalized;
    //        if (velocityDelta.sqrMagnitude > velocity.sqrMagnitude)
    //        {
    //            velocity = Vector2.zero;
    //        }
    //        else
    //        {
    //            velocity -= velocityDelta;
    //        }
    //    }
    //    rb.position += velocity * Time.deltaTime;
    //}


    //latest movement
    private void MovementUpdate(Vector2 playerInput)
    {
        //facing direction
        if (playerInput.x < 0)
        {
            currentdirection = FacingDirection.left;
        }
        else if (playerInput.x > 0)
        {
            currentdirection = FacingDirection.right;
        }
        //countdown dash timer
        dashTimer -= Time.deltaTime;

        if (playerInput.x != 0)
        {
            //accelerate
            velocity.x += acceleration * playerInput.x * Time.deltaTime;
            //clamp speed when dash is over
            if (dashTimer <= 0)
            {
                velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
            }
        }
        else
        {
            //decelerate
            if (velocity.x > 0)
            {
                velocity.x -= deceleration * Time.deltaTime;
                velocity.x = Mathf.Max(velocity.x, 0);
            }
            else if (velocity.x < 0)
            {
                velocity.x += deceleration * Time.deltaTime;
                velocity.x = Mathf.Min(velocity.x, 0);
            }
        }

    }


    //old jump
    //private void Jumping()
    //{
    //    Gravity = -2 * apexHeight / (apexTime * apexTime);
    //    jumpVelocity = 2 * apexHeight / apexTime;



    //    if (isGround && Input.GetKeyDown(KeyCode.Space))
    //    {
    //        airTime = 0;
    //        isJump = true;
    //    }

    //    //turn off rb gravity
    //    if (isGround)
    //    {
    //        rb.gravityScale = 1;
    //        fallingTimer = 0;
    //    }
    //    else
    //    {
    //        rb.gravityScale = 0;
    //        fallingTimer += Time.deltaTime;
    //    }

    //    if (isJump)
    //    {

    //        if (fallingTimer < coyoteTime && Input.GetKeyDown(KeyCode.Space))
    //        {
    //            airTime = 0;
    //        }

    //        airTime += Time.deltaTime;
    //        rb.velocity = new Vector2(rb.velocity.x, Gravity * airTime + jumpVelocity);

    //        //clamp fallling speed
    //        if (rb.velocity.y < -terminalSpeed)
    //        {
    //            rb.velocity = new Vector2(rb.velocity.x, -terminalSpeed);
    //        }
    //    }
    //}


    //new jump
    private void JumpUpdate()
    {
        if ((isGround) && Input.GetButton("Jump"))
        {
            velocity.y = jumpSpeed;
            isGround = false;
        }

        //terminal falling speed
        velocity.y = Mathf.Max(velocity.y, -terminalSpeed);
    }


    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //dash timer start
            dashTimer = dashTime;
            if (currentdirection == FacingDirection.left)
            {
                velocity.x += -dashForce;
            }
            else if ((currentdirection == FacingDirection.right))
            {
                velocity.x += +dashForce;
            }
        }
    }


    public bool IsWalking()
    {
        if (playerInput.x ==0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    private void CheckForGround()
    {
        isGround = Physics2D.OverlapBox(transform.position + Vector3.down * groundCheckOffset, groundCheckSize, 0, groundCheckMask);
    }
    public bool IsGrounded()
    {
        //isGround = Physics2D.OverlapCircle(groundCheck.position, 0.2f, ground);
        return isGround;
    }


    public FacingDirection GetFacingDirection()
    {
        return currentdirection;
        //if (faceDirection != 0)
        //{
        //    transform.localScale = new Vector3(faceDirection, 1, 1);
        //}
    }


}
