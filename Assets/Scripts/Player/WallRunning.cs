using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("WallRunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("KeyBoardInputs")]
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

     [Header ("Refrences")]
   public Transform orientation;
   private Rigidbody rb;
   private PlayerMovement pm;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        CheckForWall();
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position,orientation.right,out rightWallHit,wallCheckDistance,whatIsWall);
        wallLeft = Physics.Raycast(transform.position,orientation.right,out leftWallHit,wallCheckDistance,whatIsWall);
    }

    private bool AboveGround()
    {
        return Physics.Raycast(transform.position,Vector3.down,minJumpHeight,whatIsGround);
    }

    private void StateMachine()
    {
        horizontalInput=Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //State1 WallRun
        if ((wallLeft || wallRight) && verticalInput > 0 &&AboveGround())

        {
            //start wallrun            
        }
    }

    private void StartWallRun()
    {
        
    }
    private void WallRunMovement()
    {
        
    }
    private void StopWallRun()
    {
        
    }
}
