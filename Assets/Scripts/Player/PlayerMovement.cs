
using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float movementSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float slideSpeed;
    
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;
    
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
    public KeyCode crouchKey = KeyCode.C;
    bool iscrouching;


    [Header("Ground Check")]
    public Transform groundCheck;
    public  float groundDistance = 0.4f;
    public LayerMask whatIsGround;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    
    bool exitingSlope;
    public bool grounded;

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
        walking, sprinting,air,crouching,sliding
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        iscrouching= false;
        startYScale = transform.localScale.y;
    }
    public bool sliding;

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

        //remove bouncing on slopes
//         if (OnSlope())
// {
//         rb.AddForce(Vector3.down * 30, ForceMode.Force);
// }
        
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey)&& readyToJump && grounded && !iscrouching)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCD);
        }

        if(Input.GetKeyDown(crouchKey) && grounded)
        {
            iscrouching = true;
            transform.localScale = new Vector3(transform.localScale.x,crouchYscale,transform.localScale.z);
            rb.AddForce(Vector3.down,ForceMode.Impulse);
        }
        if(Input.GetKeyUp(crouchKey))
        {
            if(!Physics.Raycast(transform.position, Vector3.up,0.5f))
            {
            iscrouching=false;
            transform.localScale = new Vector3(transform.localScale.x,startYScale,transform.localScale.z);
            rb.AddForce(Vector3.down *5f,ForceMode.Impulse);
            }
        }
    }

    void StateHandler()
    {
        if(sliding)
        {
            state = MovementState.sliding;
            if(OnSlope() && rb.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
            }
            else
            desiredMoveSpeed=slideSpeed;
        }
        else if(grounded && Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        
        else if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        else if (!grounded)
        {
            state = MovementState.air;
        }

        
        float difference = Mathf.Abs(desiredMoveSpeed - movementSpeed);

        if (lastDesiredMoveSpeed == slideSpeed)
        {
            // Always smooth lerp when coming from sliding (smooth slide -> sprint or slide -> walk)
            StopAllCoroutines();
            StartCoroutine(SmoothLerpMoveSpeed());
        }
        else if (difference <= 10f && movementSpeed != 0)
        {
            // Snap when transitioning between walk and sprint
            movementSpeed = desiredMoveSpeed;
        }
        else
        {
            // Otherwise snap instantly
            movementSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }
    private IEnumerator SmoothLerpMoveSpeed()
    {
        float time=0;
        float difference = Mathf.Abs(desiredMoveSpeed-movementSpeed);
        difference = Mathf.Max(difference, 0.01f);
        float startValue = movementSpeed;

        while (time < difference)
        {
            movementSpeed=Mathf.Lerp(startValue,desiredMoveSpeed,time/difference);

            if (OnSlope())
            {
                float slopeAngle= Vector3.Angle(Vector3.up,slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle/90);
                time += Time.deltaTime*speedIncreaseMultiplier*slopeIncreaseMultiplier *slopeAngleIncrease;
            }
            else
            time+=Time.deltaTime*speedIncreaseMultiplier;
            yield return null;
        }
        movementSpeed=desiredMoveSpeed;

    }

    void MovePlayer()
    {
        moveDirection = orientation.forward*verticalInput + orientation.right*horizontalInput;

        //slope
        if (OnSlope() && !exitingSlope)
        {
            Vector3 slopeDirection = GetSlopeMoveDirection(moveDirection);
            rb.AddForce(slopeDirection * movementSpeed * 10f, ForceMode.Force);

            

            // stick to slope when going down
            if (rb.velocity.y <= 0)
            {  
                rb.AddForce(Vector3.down * 15f, ForceMode.Force);
            }
            
            //normal walking speed when going uphill
            if (rb.velocity.y > 0 && rb.velocity.y < 2f)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            }
        }

        //gravity off on slope
        

        //inGround
        else if(grounded){
        rb.AddForce(moveDirection.normalized * movementSpeed *10f,ForceMode.Force);
        }

        //air
        else
        {
            rb.AddForce(moveDirection.normalized * movementSpeed *10f * airMultiplier,ForceMode.Force);
        }
        // rb.useGravity = !OnSlope();
        rb.useGravity = true;
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

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up,slopeHit.normal);
            return angle< maxSlopeAngle && angle!=0;
        }
        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction,slopeHit.normal).normalized;
    }

    
}
