using System.Security.Cryptography.X509Certificates;
using Unity.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum FacingDirection
    {
        left, right
    }
    public Transform groundCheck;
    public LayerMask ground;
    public float maxSpeed;
    public float accelerationTime;
    public float decelerationTime;
    public float apexHeight;
    public float apexTime;
    public float terminalSpeed;
    public float coyoteTime;




    private Rigidbody2D rb;
    private float horizontalMove;
    public bool isGround;
    private bool isJump;
    private float acceleration;
    private float deceleration;
    private Vector2 currentVelocity;
    private float maxSpeedSqr;
    private float Gravity;
    private float jumpVelocity;
    private float airTime;
    private float fallingTimer;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        acceleration = maxSpeed / accelerationTime;
        deceleration = maxSpeed / decelerationTime;
        maxSpeedSqr = maxSpeed * maxSpeed;
    }

    void Update()
    {
        // The input from the player needs to be determined and
        // then passed in the to the MovementUpdate which should
        // manage the actual movement of the character.
        horizontalMove = Input.GetAxisRaw("Horizontal");
        //Vector2 playerInput = new Vector2(horizontalMove, 0);
        //MovementUpdate(playerInput);

        Jumping();
        //Debug.Log(rb.velocity);
        Debug.Log(fallingTimer);


    }

    private void FixedUpdate()
    {
        MovementUpdate();
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
    private void MovementUpdate()
    {
        Vector2 moveDirection = Vector2.zero;

        if (Input.GetKey(KeyCode.A))
        {
            moveDirection += Vector2.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += Vector2.right;
        }

        if (moveDirection.sqrMagnitude > 0)
        {
            currentVelocity += Time.deltaTime * acceleration * moveDirection;
            if (currentVelocity.sqrMagnitude > maxSpeedSqr)
            {
                currentVelocity = currentVelocity.normalized * maxSpeed;
            }
        }
        else
        {
            Vector2 velocityDelta = Time.deltaTime * deceleration * currentVelocity.normalized;
            if (velocityDelta.sqrMagnitude > currentVelocity.sqrMagnitude)
            {
                currentVelocity = Vector2.zero;
            }
            else
            {
                currentVelocity -= velocityDelta;
            }
        }

        rb.position += currentVelocity * Time.deltaTime;

    }


    private void Jumping()
    {
        Gravity = -2 * apexHeight / (apexTime * apexTime);
        jumpVelocity = 2 * apexHeight / apexTime;
        


        if (isGround && Input.GetKeyDown(KeyCode.Space))
        {
            airTime = 0;
            isJump = true;
        }

        //turn off rb gravity
        if (isGround)
        {
            rb.gravityScale = 1;
            fallingTimer = 0;
        }
        else
        {
            rb.gravityScale = 0;
            fallingTimer += Time.deltaTime;
        }

        if (isJump)
        {

            if (fallingTimer < coyoteTime && Input.GetKeyDown(KeyCode.Space))
            {
                airTime = 0;
            }

           airTime += Time.deltaTime;
            rb.velocity = new Vector2(rb.velocity.x, Gravity * airTime + jumpVelocity);

            //clamp fallling speed
            if (rb.velocity.y < -terminalSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -terminalSpeed);
            }
        }



    }
    public bool IsWalking()
    {
        if (horizontalMove ==0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    public bool IsGrounded()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, 0.2f, ground);

        

        return isGround;
    }


    public FacingDirection GetFacingDirection()
    {
        float faceDirection = horizontalMove;
        if (faceDirection < 0)
        {
            return FacingDirection.left;
        }
        else
        {
            return FacingDirection.right;
        }

        //if (faceDirection != 0)
        //{
        //    transform.localScale = new Vector3(faceDirection, 1, 1);
        //}
    }


}
