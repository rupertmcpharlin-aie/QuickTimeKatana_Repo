using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// VARIABLES
    /// </summary>
    /// 
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
    [SerializeField] public GameObject horse;
    [SerializeField] public GameObject environment;
    [Space]
    [SerializeField] public GameObject galen;
    [SerializeField] public GameObject galen_body;
    [SerializeField] public GameObject galen_horse;

    [Space]
    [Header("Movement Variables")]
    [SerializeField] public bool playerMovementActive;
    [SerializeField] public bool isNearHorse;
    [SerializeField] public bool canClimb;
    [Space]
    [Range(-1,1)]
    [SerializeField] public float leftStickXAxis;
    [Range(-1, 1)]
    [SerializeField] public float leftStickYAxis;
    [Space]
    [SerializeField] public float movementSpeed;
    [SerializeField] public float standingMovementSpeed;
    [SerializeField] public float crouchedMovementSpeed;
    [SerializeField] public float mountedMovementSpeed;
    [Space]
    [SerializeField] public float rotationSpeed;
    [SerializeField] public float standingRotationSpeed;
    [SerializeField] public float mountedRotationSpeed;
    [SerializeField] public float ySpeed;

    [Space]
    [Header("Combat Variables")]
    [SerializeField] public bool playerStunned;
    [SerializeField] public float damage;
    [Space]
    [SerializeField] public GameObject engagedEnemy;
    [SerializeField] public EnemyController engagedEnemyController;
    [SerializeField] public GameObject[] enemies;
    [Space]
    [SerializeField] public bool stealthKillReady;
    [SerializeField] public float stealthEngageDistance;
    [SerializeField] public int stealthHitsIndex;

    [Space]
    [Header("QTE")]
    [SerializeField] public QTEController qteController;
    [SerializeField] public Vector2 engagedEnemyScreenSpacePos;

    [Space]
    [Header("Camera")]
    [SerializeField] public CameraState cameraState;
    [SerializeField] public Transform cameraTransform;
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
    [SerializeField] public CinemachineVirtualCamera mountedCamera;

    /// <summary>
    /// ENUMS
    /// </summary>
    public enum PlayerState
    {
        exploring,
        mounted,
        crouched,
        stealthKill,
        combat,
        dead
    }

    public enum CameraState
    {
        freeCam,
        combatCam,
        lockOnCam,
        mountedCam,
    }

    /// <summary>
    /// METHODS
    /// </summary>
    // UPDATE METHOD controls everything when things are running, state dependant
    void Update()
    {
        //camera controls
        CameraManager();

        //when the player is exploring or crouched the player can:
        if (playerState == PlayerState.exploring || playerState == PlayerState.crouched)
        {
            //move
            Movement();

            //crouch
            Crouch();
        }

        //horse behaviours
        if (isNearHorse && playerState == PlayerState.exploring)
        {
            //if the player is exploring and near the horse
            if (Input.GetButtonDown("ButtonDown"))
            {
                //mount the horse
                MountHorseBehaviour();
            }
        }
        //if the player is already mounted
        else if (playerState == PlayerState.mounted)
        {
            //movement
            Movement();
            //control the speed of the horse walking animation
            HorseWalkingAnimationSpeedController();
            //controls galen being close to the player
            GalenHorseController();
            //unmount the horse on input
            UnMountHorseBehaviour();
        }

        //if the player is in combat
        if (playerState == PlayerState.combat)
        {
            //do combat
            Combat();
        }

        //if the player is crouched or doing stealth kill
        if(playerState == PlayerState.crouched || playerState == PlayerState.stealthKill)
        {
            //stealthy
            Stealth();
        }        
    }
    /********************************************************************************************************************************/
    
    //CAMERA MANAGER
    private void CameraManager()
    {
        //TRANSITION: freecam -> lock on cam
        if (Input.GetButtonDown("RightStickDown") && cameraState == CameraState.freeCam)
        {
            if (FindRemainingAliveEnemies_Unaware().Count > 0)
            {
                //TO FIX ~ Make it closest player can see
                lockedOnEnemy = FindClosestEnemy_Aware();
                CameraTransition_LockOn();
            }
            //else reset player camera
            else
            {
                //set the direction to forward
                freeCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value = playerMeshes.transform.rotation.eulerAngles.y - 180;
            }
        }

        //TRANSITION: lockon -> free cam
        else if (Input.GetButtonDown("RightStickDown") && cameraState == CameraState.lockOnCam)
        {
            freeCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value = playerMeshes.transform.rotation.eulerAngles.y - 180;
            CameraTransition_FreeCam();
        }

        //TO FIX
        //LOCK ON CAM: control the distance of the camera from the player
        if (cameraState == CameraState.lockOnCam)
        {
            //face enemy
            Vector3 targetDirection = lockedOnEnemy.transform.position - playerMeshes.transform.position;
            Quaternion toRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            playerMeshes.transform.rotation = Quaternion.RotateTowards(playerMeshes.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

            //scale distances
            lockOnDistance = targetDirection.magnitude;
        }

        //TO FIX
        //COMBAT CAM: transition to next enemy
        if (cameraState == CameraState.combatCam && 
            playerState == PlayerState.stealthKill &&
            engagedEnemy.GetComponent<EnemyController>().enemyState == EnemyController.EnemyState.dead)
        {
            //if there are no more enemies
            if (FindRemainingAliveEnemies_Aware().Count == 0)
            {

                if (playerState != PlayerState.stealthKill)
                {
                    SetPlayerState(PlayerState.exploring);
                }
                else
                {
                    SetPlayerState(PlayerState.crouched);
                }

                //transition to free cam
                CameraTransition_FreeCam();
            }

            //TO FIX
            //if there are still enemies
            else
            {
                /*engagedEnemy = FindClosestEnemy_Aware();

                StartCombat(engagedEnemy);
                engagedEnemy.GetComponent<EnemyController>().SetEnemyState(EnemyController.EnemyState.inCombat);

                //transition to new combat camera position
                combatCamera.Follow = engagedEnemy.GetComponent<EnemyController>().cameraFocus.transform;
                combatCamera.LookAt = engagedEnemy.GetComponent<EnemyController>().cameraFocus.transform;
                combatCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value = 0;*/
            }
        }
    }

    public void CameraTransition_LockOn()
    {
        freeCamera.Priority = 0;
        lockOnCamera.Priority = 1;
        combatCamera.Priority = 0;
        mountedCamera.Priority = 0;

        cameraState = CameraState.lockOnCam;
    }

    public void CameraTransition_FreeCam()
    {
        freeCamera.Priority = 1;
        lockOnCamera.Priority = 0;
        combatCamera.Priority = 0;
        mountedCamera.Priority = 0;

        cameraState = CameraState.freeCam;
    }

    public void CameraTransition_CombatCam()
    {
        freeCamera.Priority = 0;
        lockOnCamera.Priority = 0;
        combatCamera.Priority = 1;
        mountedCamera.Priority = 0;

        cameraState = CameraState.combatCam;
    }

    public void CameraTransition_MountedCam()
    {
        freeCamera.Priority = 0;
        combatCamera.Priority = 0;
        mountedCamera.Priority = 1;
        lockOnCamera.Priority = 0;

        cameraState = CameraState.mountedCam;
    }


    /*******************************************************************************************************************************/
    //EXPLORING AND CROUCHEED METHODS
    //MOVEMENT
    public void Movement()
    {
        //get axis'S
        leftStickYAxis = Input.GetAxis("LeftStickYAxis");
        leftStickXAxis = Input.GetAxis("LeftStickXAxis");

        //If the player cannot climb
        if (!canClimb)
        {
            //refine player input
            Vector3 movementDirection = new Vector3(leftStickXAxis, 0, leftStickYAxis);
            float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);
            float speed = inputMagnitude * movementSpeed;

            //PLAYER ROTATION
            //free camera || mounted camera
            if (cameraState == CameraState.freeCam || cameraState == CameraState.mountedCam)
            {
                //get direction from camera position / rotation
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
            else if (cameraState == CameraState.lockOnCam)
            {
                movementDirection = Quaternion.AngleAxis(playerMeshes.transform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
                movementDirection.Normalize();
            }

            //get velocity
            Vector3 velocity = movementDirection * speed;

            //gravity stuff
            ySpeed = Physics.gravity.y;
            velocity.y = ySpeed;

            //move character
            characterController.Move(velocity * Time.deltaTime);
        }
        //if the player is climbing
        else
        {
            //refine player input
            Vector3 movementDirection = new Vector3(0, leftStickYAxis, 0);
            float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);
            float speed = inputMagnitude * movementSpeed;

            //get velocity
            Vector3 velocity = movementDirection * speed;

            //move character
            characterController.Move(velocity * Time.deltaTime);
        }
    }

    //CROUCH
    public void Crouch()
    {
        if (Input.GetButtonDown("LeftStickDown"))
        {
            if (playerState == PlayerState.exploring)
            {
                animator.SetTrigger("CrouchTrigger");
                SetPlayerState(PlayerState.crouched);
                movementSpeed = crouchedMovementSpeed;
            }
            else if (playerState == PlayerState.crouched || playerState == PlayerState.stealthKill)
            {
                animator.SetTrigger("StandTrigger");
                SetPlayerState(PlayerState.exploring);
                movementSpeed = standingMovementSpeed;
            }
        }
    }

    /*******************************************************************************************************************************/
    //HORSE SHIT BABYYYYY
    private void MountHorseBehaviour()
    {
        //set state
        SetPlayerState(PlayerState.mounted);
        
        //trigger animation
        animator.SetTrigger("Mount_Horse");

        //start horse walking animation
        horse.GetComponent<HorseScript>().StartWalkingAnimation();

        //set player position and rotation
        gameObject.transform.position = horse.transform.position;
        playerMeshes.transform.rotation = horse.transform.rotation;

        //set the horse to be under the player meshes
        horse.transform.SetParent(playerMeshes.transform);

        //transition to mounted cam
        CameraTransition_MountedCam();

        //set speeds
        movementSpeed = mountedMovementSpeed;
        rotationSpeed = mountedRotationSpeed;

        //galen mount as well
        galen.transform.position = galen_horse.transform.position;
        galen_body.transform.position = new Vector3(galen_body.transform.position.x, galen_body.transform.position.y + 2, galen_body.transform.position.z);
        galen_horse.transform.SetParent(galen.transform);
    }

    //horse walking animation speed controller
    private void HorseWalkingAnimationSpeedController()
    {
        horse.GetComponent<Animator>().speed = Mathf.Clamp01(Mathf.Abs(leftStickXAxis) + Mathf.Abs(leftStickYAxis));
    }

    //set galens position
    public void GalenHorseController()
    {
        galen.GetComponent<NavMeshAgent>().SetDestination(new Vector3(playerMeshes.transform.position.x + 4, playerMeshes.transform.position.y, playerMeshes.transform.position.z));
    }

    //unmount horse behaviour
    private void UnMountHorseBehaviour()
    {
        //when press button down
        if (Input.GetButtonDown("ButtonDown"))
        {
            //reset horse hierarchy
            horse.transform.SetParent(environment.transform);

            //set player state
            SetPlayerState(PlayerState.exploring);

            //start player animation
            animator.SetTrigger("UnMount_Horse");

            //start horse idle animation
            horse.GetComponent<HorseScript>().StartIdleAnimation();
            horse.GetComponent<Animator>().speed = 1;

            //move player
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + 5);

            //transition to free cam
            CameraTransition_FreeCam();

            //reset speeds
            movementSpeed = standingMovementSpeed;
            rotationSpeed = standingRotationSpeed;
        }
    }

    /******************************************************************************************************************************/
    //COMBAT METHODS
    //COMBAT
    public void Combat()
    {
        //things to do once
        if (cameraState != CameraState.combatCam)
        {
            //transition to combat camera
            CameraTransition_CombatCam();

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

    //begin combat
    public void StartCombat(GameObject nearestEnemy)
    {
        //player
        SetPlayerState(PlayerState.combat);

        //enemy
        engagedEnemy = nearestEnemy;
        EnemyController nearestEnemyController = nearestEnemy.GetComponent<EnemyController>();
        nearestEnemyController.SetEnemyState(EnemyController.EnemyState.inCombat);
        nearestEnemyController.currentQTEBackground.SetActive(true);

        //combat camera
        combatCamera.Follow = nearestEnemyController.cameraFocus.transform;
        combatCamera.LookAt = nearestEnemyController.cameraFocus.transform;

        //qte
        qteController.currentQTEBackground = nearestEnemyController.currentQTEBackground;
        qteController.enemyController = nearestEnemyController;
    }

    /*******************************************************************************************************************************/
    //STEALTH
    public void Stealth()
    {
        //if stealth kill isnt ready
        if (playerState != PlayerState.stealthKill)
        {
            //for each enemy that is alive
            foreach (GameObject enemy in FindRemainingAliveEnemies_Unaware())
            {
                //check if distance between player and enemies is less than threshold to start stealth kill
                if (Vector3.Distance(transform.position, enemy.transform.position) < stealthEngageDistance)
                {
                    //change player state to stealth kill
                    SetPlayerState(PlayerState.stealthKill);

                    //set engaged enemy variables
                    engagedEnemy = enemy;
                    engagedEnemyController = engagedEnemy.GetComponent<EnemyController>();
                    engagedEnemyController.currentQTEBackground.SetActive(true);

                    //set camerra variables
                    combatCamera.Follow = engagedEnemyController.cameraFocus.transform;
                    combatCamera.LookAt = engagedEnemyController.cameraFocus.transform;

                    //transition camera
                    CameraTransition_CombatCam();

                    //set qte variables
                    qteController.enemyController = engagedEnemyController;
                    qteController.currentQTEBackground = engagedEnemyController.currentQTEBackground;
                }
            }
        }

        //if player state is stealth kill
        if(playerState == PlayerState.stealthKill)
        {
            //face enemy
            Vector3 direction = engagedEnemy.transform.position - transform.position;
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            playerMeshes.transform.rotation = Quaternion.RotateTowards(playerMeshes.transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

            //slowly rotate camera
            combatCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value +=  combatCameraRotationSpeed * Time.deltaTime;
            
            //set position of QTE background
            engagedEnemyScreenSpacePos = RectTransformUtility.WorldToScreenPoint(Camera.main, engagedEnemy.GetComponent<EnemyController>().torsoe.transform.position);
            engagedEnemy.GetComponent<EnemyController>().currentQTEBackground.transform.position = new Vector3(engagedEnemyScreenSpacePos.x, engagedEnemyScreenSpacePos.y, 0);
        }
    }
    /*********************************************************************************************************************************/
    //UTILITY METHODS
    //FIND ENEMIES
    public List<GameObject> FindRemainingAliveEnemies_Aware()
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

    public List<GameObject> FindRemainingAliveEnemies_Unaware()
    {
        List<GameObject> tempList = new List<GameObject>();

        //check if there is anymore enemies
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController.enemyState == EnemyController.EnemyState.alive)
            {
                tempList.Add(enemy);
            }
        }

        return tempList;
    }

    public GameObject FindClosestEnemy_Aware()
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

    public void SetPlayerState(PlayerState newState)
    {
        Debug.Log("New player state = " + newState);
        playerState = newState;
    }
}