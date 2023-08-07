using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    [Header("GameObjects")]
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
    [SerializeField] public CharacterController characterController;
    [Space]
    [SerializeField] public float leftStickXAxis;
    [SerializeField] public float leftStickYAxis;
    [SerializeField] public float movementSpeed;
    [Space]
    [Space]
    [SerializeField] public float rotationSpeed;
    [SerializeField] public float ySpeed;
    [SerializeField] public Transform cameraTransform;

    [Space]
    [Header("Combat Variables")]
    [SerializeField] public bool inCombat;
    [SerializeField] public GameObject[] enemies;


    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
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

        //get player input
        Vector3 movementDirection = new Vector3(leftStickXAxis, 0, leftStickYAxis);
        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);

        //if crouching half speed

        float speed = inputMagnitude * movementSpeed;
        movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        movementDirection.Normalize();

        ySpeed += Physics.gravity.y * Time.deltaTime;

        Vector3 velocity = movementDirection * speed;
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

        //get camera vector
        
        if(movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            body.transform.rotation = Quaternion.RotateTowards(body.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }



    }

    /*cameraForwardVector = Camera.main.transform.forward;*/




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
