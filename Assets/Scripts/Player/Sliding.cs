using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sliding : MonoBehaviour
{
   [Header ("Refrences")]
   public Transform orientation;
   public Transform playerObj;
   private Rigidbody rb;
   private PlayerMovement pm;

   [Header("Sliding")]
   public float maxslideTime;
   public float slideForce;
   private float slideTime;

   public float slideYScale;
   private float StartYScale;
   [Header("Inputs")]
   public KeyCode slideKey = KeyCode.LeftControl;
   private float horizontalInput;
   private float verticalInput;

   

   private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        StartYScale = playerObj.localScale.y;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(slideKey)&& (horizontalInput!=0 || verticalInput != 0)  && pm.grounded)
        {
            StartSlide();
        }
        if(Input.GetKeyUp(slideKey)&& pm.sliding)
        {
            StopSlide();
        }
    }

    private void FixedUpdate()
    {
        if (pm.sliding)
        {
            SlidingMovement();
        }
    }

    private void StartSlide()
    {
        pm.sliding= true;
        playerObj.localScale = new Vector3(playerObj.localScale.x,slideYScale,playerObj.localScale.z);
        slideTime = maxslideTime;
    }
    
    private void SlidingMovement()
    {
        
        Vector3 inputDirection = orientation.forward * verticalInput +orientation.right*horizontalInput;

        if(!pm.OnSlope()|| rb.velocity.y>-.02f){
        rb.AddForce(inputDirection.normalized*slideForce,ForceMode.Force);
        slideTime-=Time.deltaTime;
        
        }

        else
        {
        rb.AddForce(pm.GetSlopeMoveDirection(inputDirection).normalized*slideForce,ForceMode.Force);
        }

        if (slideTime < 0)
        {
            StopSlide();
        }
    }

    private void StopSlide()
    {
        pm.sliding = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, StartYScale,playerObj.localScale.z);
    }

    
}
