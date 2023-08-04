using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] Rigidbody playerRB;
    [SerializeField] GameObject body;
    [SerializeField] GameObject katana;


    [Space]
    [Header("Camera")]
    [SerializeField] public GameObject playerCamera;
    [SerializeField] public bool resetCam;
    [SerializeField] public float camYMin;
    [SerializeField] public float camYMax;


    [Space]
    [Header("Movement Variables")]
    [SerializeField] public bool playerMovementActive = true;
    [Space]
    [SerializeField] public float leftStickXAxis;
    [SerializeField] public float leftStickYAxis;
    [SerializeField] public float movementSpeed;
    [SerializeField] public float movementSpeedVertical;
    [SerializeField] public float movementSpeedHorizontal;
    [Space]
    [SerializeField] public Vector3 cameraForwardVector;
    [SerializeField] public Vector3 cameraHorizontalVector;
    [SerializeField] public Vector3 forwardTargetPos;
    [SerializeField] public Vector3 horizontalTargetPos;
    [Space]
    [Space]
    [SerializeField] public float rotationSpeed;
    [SerializeField] public Vector3 cameraRotationVector;
    [SerializeField] public Vector3 bodyRotationVector;
    [SerializeField] public Quaternion cameraRotationQuat;
    [SerializeField] public Quaternion bodyRotationQuat;

    [Space]
    [Header("Combat Variables")]
    [SerializeField] public bool inCombat;
    [SerializeField] public GameObject[] enemies;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (playerMovementActive && !inCombat)
        {
            Movement();
            //Rotation();
        }
    }

    public void Movement()
    {
        //MOVEMENT
        //vertical
        //get axis
        leftStickYAxis = Input.GetAxis("LeftStickYAxis");
        leftStickXAxis = Input.GetAxis("LeftStickXAxis");

        cameraForwardVector = Camera.main.transform.forward;

        Vector3 movementDirection = new Vector3(cameraForwardVector.x * leftStickXAxis, 0, cameraForwardVector.z * leftStickYAxis);
        movementDirection.Normalize();

        transform.Translate(cameraForwardVector * movementSpeed * Time.deltaTime, Space.World);

        //get camera vector
        



    }

    private void Rotation()
    {
        /*if(Input.GetButtonDown("RightStickDown"))
        {
            resetCam = true;
        }*/

        //ROTATION
        cameraRotationVector = Camera.main.transform.rotation.eulerAngles;
        cameraRotationVector = new Vector3(0, cameraRotationVector.y, 0);
        cameraRotationQuat = Quaternion.Euler(cameraRotationVector.x, cameraRotationVector.y, cameraRotationVector.z);

        //forward left stick
        if (leftStickYAxis > 0)
        {
            //rotate to face away from camera
            body.transform.rotation = Quaternion.RotateTowards(body.transform.rotation, cameraRotationQuat, rotationSpeed);
        }
    }




}

/*[Header("Testing")]
[SerializeField] bool A;
[SerializeField] bool B;
[SerializeField] bool X;
[SerializeField] bool Y;
[SerializeField] bool LB;
[SerializeField] bool RB;
[SerializeField] bool start;
[SerializeField] bool select;
[SerializeField] float DPadHorizontal;
[SerializeField] float DPadVertical;
[SerializeField] float LeftTrigger;
[SerializeField] float RightTrigger;*/

/*private void Testing()
{

    //A
    if (Input.GetButtonDown("A"))
    {
        if (A)
        {
            A = false;
        }
        else
        {
            A = true;
        }
    }

    //B
    if (Input.GetButtonDown("B"))
    {
        if (B)
        {
            B = false;
        }
        else
        {
            B = true;
        }
    }

    //X
    if (Input.GetButtonDown("X"))
    {
        if (X)
        {
            X = false;
        }
        else
        {
            X = true;
        }
    }

    //Y
    if (Input.GetButtonDown("Y"))
    {
        if (Y)
        {
            Y = false;
        }
        else
        {
            Y = true;
        }
    }

    //LB
    if (Input.GetButtonDown("LeftBumper"))
    {
        if (LB)
        {
            LB = false;
        }
        else
        {
            LB = true;
        }
    }

    //RB
    if (Input.GetButtonDown("RightBumper"))
    {
        if (RB)
        {
            RB = false;
        }
        else
        {
            RB = true;
        }
    }

    //start
    if (Input.GetButtonDown("Start"))
    {
        if (start)
        {
            start = false;
        }
        else
        {
            start = true;
        }
    }

    //select
    if (Input.GetButtonDown("Select"))
    {
        if (select)
        {
            select = false;
        }
        else
        {
            select = true;
        }
    }

    LeftTrigger = Input.GetAxis("LeftTrigger");
    RightTrigger = Input.GetAxis("RightTrigger");
    DPadHorizontal = Input.GetAxis("DPadXAxis");
    DPadVertical = Input.GetAxis("DPadYAxis");
}*/
