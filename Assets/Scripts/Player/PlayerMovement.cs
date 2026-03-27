using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed;

    [Header("Ground Check")]
    public float playerHeight;
    public float groundDrag;
    public float jumpForce;
    public float jumpCD;
    public float airMultiplier;
    bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;
    float horizontalInput;
    float verticalInput;

    Vector3 movementDirection;
    Rigidbody playerRB;
    

    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        playerRB.freezeRotation=true;
        readyToJump = true;
    }

    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight*0.5f+.2f,whatIsGround);
        MyInput();
        SpeedControl();

        if (grounded)
        {
            playerRB.drag = groundDrag;
        }
        else
        {
            playerRB.drag=0;
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput=Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if(Input.GetKey(jumpKey)&& readyToJump && grounded)
        {
            readyToJump=false;
            Jump();
            Invoke(nameof(JumpReset),jumpCD);
        }
    }

     private void MovePlayer()
    {
        movementDirection= orientation.forward*verticalInput+ orientation.right*horizontalInput;
        if(grounded)
        {
            playerRB.AddForce(movementDirection.normalized*movementSpeed*10f,ForceMode.Force);
        }
        else if(!grounded){
            playerRB.AddForce(movementDirection.normalized*movementSpeed*10f*airMultiplier,ForceMode.Force);    
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(playerRB.velocity.x,0f,playerRB.velocity.z);

        if (flatVelocity.magnitude > movementSpeed)
        {
            Vector3 limitVelocity =  flatVelocity.normalized*movementSpeed;
            playerRB.velocity= new Vector3(limitVelocity.x,playerRB.velocity.y,limitVelocity.z);
        }
    }
    private void Jump()
    {
        playerRB.velocity = new Vector3(playerRB.velocity.x,0f,playerRB.velocity.z);
        playerRB.AddForce(transform.up*jumpForce,ForceMode.Impulse);
    }
    
    private void JumpReset()
    {
         readyToJump=true;
    }
}