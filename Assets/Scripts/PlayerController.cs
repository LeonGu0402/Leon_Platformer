using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum FacingDirection
    {
        left, right
    }
    public float speed;
    public float maxSpeed;
    public float acceleration;
    public float deceleration;
    public Transform groundCheck;
    public LayerMask ground;
    private Rigidbody2D rb;
    private float horizontalMove;
    private bool isGround;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // The input from the player needs to be determined and
        // then passed in the to the MovementUpdate which should
        // manage the actual movement of the character.

        horizontalMove = Input.GetAxisRaw("Horizontal");
        Vector2 playerInput = new Vector2(horizontalMove, 0);


        MovementUpdate(playerInput);
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        rb.velocity = new Vector2(playerInput.x * speed, 0);
        
        if (horizontalMove > 0 || horizontalMove < 0)
        {
            speed += acceleration * Time.deltaTime;
            if (speed > maxSpeed)
            {
                speed = maxSpeed;
            }
        }

        if (horizontalMove == 0)
        {
            speed -= deceleration * Time.deltaTime;
            if (speed < 0)
            {
                speed = 0;
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
    }
}
