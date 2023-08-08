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
    [SerializeField] public CinemachineVirtualCamera freeCamera;
    [SerializeField] public bool resetCam;
    [SerializeField] public float camYMin;
    [SerializeField] public float camYMax;
    [Space]
    [SerializeField] public CinemachineVirtualCamera lockOnCamera;
    [SerializeField] public GameObject lockedOnEnemy;
    [SerializeField] public float rightStickXAxis;
    [SerializeField] public float lockOnDistance;
    [SerializeField] public float lockOnOffset;
    [Space]
    [SerializeField] public CinemachineVirtualCamera combatCamera;

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
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        //free cam movement
        if (playerMovementActive && !inCombat)
        {
            Movement();
        }

        CameraManager();
    }

    private void CameraManager()
    {
        //freecam -> lock on cam
        if(Input.GetButtonDown("RightStickDown") && freeCamera.Priority == 1 && enemies.Length > 0)
        {
            //find closest enemy;
            float minDistance = 0;
            foreach(GameObject enemy in enemies)
            {
                float tempDistance = Vector3.Distance(transform.position, enemy.transform.position);
                if(tempDistance > minDistance)
                {
                    minDistance = tempDistance;
                    lockedOnEnemy = enemy;
                }
            }

            //set cameras;
            freeCamera.Priority = 0;
            lockOnCamera.Priority = 1;
        }
        //lockon -> free cam
        else if (Input.GetButtonDown("RightStickDown") && lockOnCamera.Priority == 1)
        {
            freeCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value = body.transform.rotation.eulerAngles.y-180;
            freeCamera.Priority = 1;
            lockOnCamera.Priority = 0;
        }

        //lock on cam distance
        if (lockOnCamera.Priority == 1)
        {
            //face enemy
            Vector3 targetDirection = lockedOnEnemy.transform.position - body.transform.position;
            Quaternion toRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            body.transform.rotation = Quaternion.RotateTowards(body.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

            //scale distances
            lockOnDistance = targetDirection.magnitude;

            //rightStickXAxis
            rightStickXAxis = Input.GetAxis("test");
            lockOnCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().ShoulderOffset.x = rightStickXAxis;
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

        //gravity stuff
        ySpeed += Physics.gravity.y * Time.deltaTime;


        //free camera
        if (freeCamera.Priority == 1)
        {
            //if crouching half speed

            float speed = inputMagnitude * movementSpeed;
            movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
            movementDirection.Normalize();

            //get velocity
            Vector3 velocity = movementDirection * speed;
            velocity.y = ySpeed;

            //move character
            characterController.Move(velocity * Time.deltaTime);


            //face direction of movement
            if (movementDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

                body.transform.rotation = Quaternion.RotateTowards(body.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }
        }

        //lock on cam
        if (lockOnCamera.Priority == 1)
        {
            //if crouching half speed

            float speed = inputMagnitude * movementSpeed;
            movementDirection = Quaternion.AngleAxis(body.transform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
            movementDirection.Normalize();

            //get velocity
            Vector3 velocity = movementDirection * speed;
            velocity.y = ySpeed;

            //move character
            characterController.Move(velocity * Time.deltaTime);
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
