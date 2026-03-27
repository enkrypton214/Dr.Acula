
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("Refrences")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;

    public Rigidbody playerRB;

    public float roationSpeed;

    void Start()
    {
        Cursor.lockState= CursorLockMode.Locked;
        Cursor.visible=false;
    }

    void Update()
    {
        //rotate orientation
        Vector3 viewDirection = player.position- new Vector3(transform.position.x,player.position.y,transform.position.z);
        orientation.forward= viewDirection.normalized;

        //rotate player obj
        float horizontalInput = -Input.GetAxis("Horizontal");
        float verticalInput = -Input.GetAxis("Vertical");
        Vector3 inputDirection = orientation.forward*verticalInput + orientation.right*horizontalInput;

        if (inputDirection != Vector3.zero)
        {
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDirection.normalized,Time.deltaTime*roationSpeed);
        }
    }

}
