using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] public GameObject body;


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
        if (playerMovementActive)
        {
            Movement();
            Rotation();
        }
    }

    public void Movement()
    {
        //MOVEMENT
        //vertical
        //get axis
        leftStickYAxis = Input.GetAxis("LeftStickYAxis");

        //get camera vector
        cameraForwardVector = Camera.main.transform.forward;
        //get forward target vector
        forwardTargetPos = cameraForwardVector + transform.position;
        forwardTargetPos.y = 0;

        //movement towards and away from camera
        if (leftStickYAxis != 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, forwardTargetPos, leftStickYAxis * movementSpeedVertical);
        }

        //horizontal
        leftStickXAxis = Input.GetAxis("LeftStickXAxis");

        cameraHorizontalVector = Camera.main.transform.right;
        horizontalTargetPos = cameraHorizontalVector + transform.position;
        horizontalTargetPos.y = 0;

        if (leftStickXAxis != 0)
        {
            Debug.Log("MAde it");
            transform.position = Vector3.MoveTowards(transform.position, horizontalTargetPos, leftStickXAxis * movementSpeedHorizontal);
        }
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
