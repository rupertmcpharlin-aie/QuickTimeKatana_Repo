using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] public PlayerState playerState;
    [Space]
    [SerializeField] public Animator animator;
    [SerializeField] public CharacterController characterController;
    [Space]
    [SerializeField] public GameObject playerMeshes;
    [SerializeField] public GameObject torsoe;
    [SerializeField] public GameObject bodyStanding;
    [SerializeField] public GameObject head;
    [SerializeField] public GameObject katana;
    [Space]
    [SerializeField] public BaseQTEScript baseQTEScript;

    [Space]
    [Header("Camera")]
    [SerializeField] public CameraState cameraState;
    [Space]
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
    [SerializeField] public float combatCameraRotationSpeed;

    [Space]
    [Header("Movement Variables")]
    [SerializeField] public bool playerMovementActive = true;
    [Space]
    [Range(-1,1)]
    [SerializeField] public float leftStickXAxis;
    [Range(-1, 1)]
    [SerializeField] public float leftStickYAxis;
    [Space]
    [SerializeField] public float movementSpeed;
    [SerializeField] public float standingMovementSpeed;
    [SerializeField] public float crouchedMovementSpeed;
    [Space]
    [SerializeField] public float rotationSpeed;
    [SerializeField] public float ySpeed;
    [SerializeField] public Transform cameraTransform;

    [Space]
    [Header("Combat Variables")]
    [SerializeField] public bool playerStunned;
    [SerializeField] public float damage;
    [Space]
    [SerializeField] public GameObject engagedEnemy;
    [SerializeField] public EnemyController engagedEnemyController;
    [SerializeField] public GameObject[] enemies;
    [Space]
    [SerializeField] public bool stealthTakeDownActive;
    [SerializeField] public float stealthEngageDistance;
    [SerializeField] public int stealthHitsIndex;

    [Space]
    [Header("QTE")]
    [SerializeField] public BaseQTEScript QTEScript;
    [SerializeField] public Vector2 engagedEnemyScreenSpacePos;

    public enum PlayerState
    {
        exploring,
        crouched,
        stealthKill,
        combat,
        dead
    }

    public enum CameraState
    {
        freeCam,
        combatCam,
        lockOnCam
    }



    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //free cam movement
        if (playerState == PlayerState.exploring || playerState == PlayerState.crouched)
        {
            Movement();
            Crouch();
        }

        if(playerState == PlayerState.combat)
        {
            Combat();
        }

        if(playerState == PlayerState.crouched)
        {
            Stealth();
        }

        CameraManager();
    }

    public void Combat()
    {
        //things to do once
        if (combatCamera.Priority != 1)
        {
            //transition to combat camera
            freeCamera.Priority = 0;
            lockOnCamera.Priority = 0;
            combatCamera.Priority = 1;

            //set background active
            engagedEnemy.GetComponent<EnemyController>().currentQTEBackground.SetActive(true);
        }

        //face enemy
        Vector3 direction = engagedEnemy.transform.position - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        playerMeshes.transform.rotation = Quaternion.RotateTowards(playerMeshes.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

        //slowly rotate camera
        combatCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value += combatCameraRotationSpeed * Time.deltaTime;

        //set position of QTE background
        engagedEnemyScreenSpacePos = RectTransformUtility.WorldToScreenPoint(Camera.main, engagedEnemy.GetComponent<EnemyController>().torsoe.transform.position);
        engagedEnemy.GetComponent<EnemyController>().currentQTEBackground.transform.position = new Vector3(engagedEnemyScreenSpacePos.x, engagedEnemyScreenSpacePos.y, 0);
    }  


    private void CameraManager()
    {
        //freecam -> lock on cam
        if(Input.GetButtonDown("RightStickDown") && freeCamera.Priority == 1)
        {
            if(FindRemainingAliveEnemies().Count > 0)
            {
                lockedOnEnemy = FindClosestEnemy();

                //set cameras;
                freeCamera.Priority = 0;
                lockOnCamera.Priority = 1;
            }
            //else reset player camera
            else
            {
                freeCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value = playerMeshes.transform.rotation.eulerAngles.y - 180;
            }
        }

        //lockon -> free cam
        else if (Input.GetButtonDown("RightStickDown") && lockOnCamera.Priority == 1)
        {
            freeCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value = playerMeshes.transform.rotation.eulerAngles.y-180;
            freeCamera.Priority = 1;
            lockOnCamera.Priority = 0;
        }

        //lock on cam distance
        if (lockOnCamera.Priority == 1)
        {
            //face enemy
            Vector3 targetDirection = lockedOnEnemy.transform.position - playerMeshes.transform.position;
            Quaternion toRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            playerMeshes.transform.rotation = Quaternion.RotateTowards(playerMeshes.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

            //scale distances
            lockOnDistance = targetDirection.magnitude;

            //rightStickXAxis
            rightStickXAxis = Input.GetAxis("test");
            lockOnCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().ShoulderOffset.x = rightStickXAxis;
        }


        //if engaged enemy is no longer alive
        if(combatCamera.Priority == 1 && engagedEnemy.GetComponent<EnemyController>().enemyState == EnemyController.EnemyState.dead)
        {
            //if there are no more enemies
            if(FindRemainingAliveEnemies().Count == 0)
            {
                if (playerState != PlayerState.stealthKill)
                {
                    playerState = PlayerState.exploring;
                }
                else
                {
                    playerState = PlayerState.crouched;
                }
                //transition to free cam
                combatCamera.Priority = 0;
                freeCamera.Priority = 1;
            }

            //if there are still enemies
            else
            {
                Debug.Log("Move onto next enemy");
                engagedEnemy = FindClosestEnemy();

                StartCombat(engagedEnemy);
                engagedEnemy.GetComponent<EnemyController>().SetEnemyState(EnemyController.EnemyState.inCombat);

                //transition to new combat camera position
                combatCamera.Follow = engagedEnemy.GetComponent<EnemyController>().cameraFocus.transform;
                combatCamera.LookAt = engagedEnemy.GetComponent<EnemyController>().cameraFocus.transform;
                combatCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value = 0;
            }
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
        float speed = inputMagnitude * movementSpeed;

        //gravity stuff
        ySpeed += Physics.gravity.y * Time.deltaTime;

        //free camera
        if (freeCamera.Priority == 1)
        {
            movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
            movementDirection.Normalize();

            //face direction of movement
            if (movementDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
                playerMeshes.transform.rotation = Quaternion.RotateTowards(playerMeshes.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }
        }
        //lock on cam
        else if (lockOnCamera.Priority == 1)
        {
            movementDirection = Quaternion.AngleAxis(playerMeshes.transform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
            movementDirection.Normalize();
        }

        //get velocity
        Vector3 velocity = movementDirection * speed;
        velocity.y = ySpeed;

        //move character
        characterController.Move(velocity * Time.deltaTime);
    }

    //controls crouch stuff
    public void Crouch()
    {
        if(Input.GetButtonDown("LeftStickDown"))
        {
            if (playerState == PlayerState.exploring)
            {
                animator.SetTrigger("CrouchTrigger");
                playerState = PlayerState.crouched;
                movementSpeed = crouchedMovementSpeed;
            }
            else if(playerState == PlayerState.crouched || playerState == PlayerState.stealthKill)
            {
                animator.SetTrigger("StandTrigger");
                playerState = PlayerState.exploring;
                movementSpeed = standingMovementSpeed;
            }

        }
    }

    public void Stealth()
    {
        if (!stealthTakeDownActive)
        {
            foreach (GameObject enemy in FindRemainingAliveEnemies())
            {
                if (Vector3.Distance(transform.position, enemy.transform.position) < stealthEngageDistance &&
                    enemy.GetComponent<EnemyController>().enemyState == EnemyController.EnemyState.alive)
                {
                    Debug.Log("Start Stealth TakeDown");
                    playerState = PlayerState.stealthKill;

                    engagedEnemy = enemy;
                    engagedEnemyController = engagedEnemy.GetComponent<EnemyController>();
                    engagedEnemyController.currentQTEBackground.SetActive(true);

                    combatCamera.Follow = engagedEnemyController.cameraFocus.transform;
                    combatCamera.LookAt = engagedEnemyController.cameraFocus.transform;

                    combatCamera.Priority = 1;
                    freeCamera.Priority = 0;
                    lockOnCamera.Priority = 0;

                    baseQTEScript.enemyController = engagedEnemyController;
                    baseQTEScript.currentQTEBackground = engagedEnemyController.currentQTEBackground;
                }
            }
        }

        if(playerState == PlayerState.stealthKill)
        {
            //face enemy
            Vector3 direction = engagedEnemy.transform.position - transform.position;
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            playerMeshes.transform.rotation = Quaternion.RotateTowards(playerMeshes.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

            //slowly rotate camera
            combatCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value += combatCameraRotationSpeed * Time.deltaTime;

            //set position of QTE background
            engagedEnemyScreenSpacePos = RectTransformUtility.WorldToScreenPoint(Camera.main, engagedEnemy.GetComponent<EnemyController>().torsoe.transform.position);
            engagedEnemy.GetComponent<EnemyController>().currentQTEBackground.transform.position = new Vector3(engagedEnemyScreenSpacePos.x, engagedEnemyScreenSpacePos.y, 0);
        }




    }

    public void StartCombat(GameObject nearestEnemy)
    {
        EnemyController nearestEnemyController = nearestEnemy.GetComponent<EnemyController>();

        //player
        playerState = PlayerState.combat;

        //enemy
        engagedEnemy = nearestEnemy;
        nearestEnemyController.SetEnemyState(EnemyController.EnemyState.inCombat);
        nearestEnemyController.currentQTEBackground.SetActive(true);

        //combat camera
        combatCamera.Follow = nearestEnemyController.cameraFocus.transform;
        combatCamera.LookAt = nearestEnemyController.cameraFocus.transform;

        //qte
        QTEScript.currentQTEBackground = nearestEnemyController.currentQTEBackground;
        QTEScript.enemyController = nearestEnemyController;
    }


    public List<GameObject> FindRemainingAliveEnemies()
    {
        List<GameObject> tempList = new List<GameObject>();

        //check if there is anymore enemies
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController.enemyState == EnemyController.EnemyState.alive ||
                enemyController.enemyState == EnemyController.EnemyState.awareOfPlayer)
            {
                tempList.Add(enemy);
            }
        }

        return tempList;
    }

    public GameObject FindClosestEnemy()
    {
        //find closest enemy;
        float minDistance = 0;

        foreach (GameObject enemy in enemies)
        {
            float tempDistance = Vector3.Distance(transform.position, enemy.transform.position);

            if (tempDistance > minDistance && enemy.GetComponent<EnemyController>().enemyState == EnemyController.EnemyState.alive ||
                                              enemy.GetComponent<EnemyController>().enemyState == EnemyController.EnemyState.awareOfPlayer)
            {
                minDistance = tempDistance;
            }
        }

        Debug.Log(minDistance);

        foreach(GameObject enemy in enemies)
        {
            if (minDistance == Vector3.Distance(transform.position, enemy.transform.position) && enemy.GetComponent<EnemyController>().enemyState == EnemyController.EnemyState.alive ||
                                                                                                 enemy.GetComponent<EnemyController>().enemyState == EnemyController.EnemyState.awareOfPlayer)
            {
                return enemy;
            }
        }

        return null;
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
