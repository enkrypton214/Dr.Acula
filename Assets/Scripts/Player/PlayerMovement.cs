
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float movementSpeed;

    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCD;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYscale;
    private float startYScale;
    public KeyCode crouchKey = KeyCode.LeftControl;


    [Header("Ground Check")]
    public Transform groundCheck;
    public  float groundDistance = 0.4f;
    public LayerMask whatIsGround;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    
    bool exitingSlope;
    bool grounded;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;
    
    [Header("KeyBinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    public Transform orientation;
    public MovementState state;

    public enum MovementState{
        walking, sprinting,air,crouching
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        MyInput();
        SpeedControl();
        StateHandler();
        //groundCheck

        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround); 
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else rb.drag = 0;
        
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey)&& readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCD);
        }

        if(Input.GetKeyDown(crouchKey) && grounded)
        {
            transform.localScale = new Vector3(transform.localScale.x,crouchYscale,transform.localScale.z);
            rb.AddForce(Vector3.down,ForceMode.Impulse);
        }
        if(Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x,startYScale,transform.localScale.z);
            rb.AddForce(Vector3.down *5f,ForceMode.Impulse);
        }
    }

    void StateHandler()
    {
        if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            movementSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            movementSpeed = walkSpeed;
        }

        else if (!grounded)
        {
            state = MovementState.air;
        }

        if(grounded && Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            movementSpeed = crouchSpeed;
        }

        
    }

    void MovePlayer()
    {
        moveDirection = orientation.forward*verticalInput + orientation.right*horizontalInput;

        //slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection()*movementSpeed*10f,ForceMode.Force);
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down*75f, ForceMode.Force);
            }
        }

        //gravity off on slope
        

        //inGround
        if(grounded){
        rb.AddForce(moveDirection.normalized * movementSpeed *10f,ForceMode.Force);
        }

        //air
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * movementSpeed *10f * airMultiplier,ForceMode.Force);
        }
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {

        //speed control on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > movementSpeed)
            {
                rb.velocity = rb.velocity.normalized*movementSpeed;
            }
        }

        //limiting on ground and air
        else
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x,0,rb.velocity.z);

        if (flatVelocity.magnitude > movementSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * movementSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y ,limitedVelocity.z);

        }
        }

        
    }

    private void Jump()
    {
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0 , rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope=false;
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up,slopeHit.normal);
            return angle< maxSlopeAngle && angle!=0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection,slopeHit.normal).normalized;
    }
}
