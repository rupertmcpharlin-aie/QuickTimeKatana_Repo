using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static PlayerController;

public class EnemyController : MonoBehaviour
{
    /// <summary>
    /// VARIABLES
    /// </summary>
    [Space]
    [Header("Enemy Variables")]
    [SerializeField] public EnemyState enemyState;
    [Space]
    [SerializeField] public GameObject enemyMeshes;
    [SerializeField] public GameObject torsoe;
    [SerializeField] public GameObject head;
    [Space]
    [SerializeField] public GameObject cameraFocus;
    [Space]
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public Animator enemyAnimator;
    [Space]
    [SerializeField] public PlayerController playerController;
    
    [Header("Combat Variables")]
    [Space]
    [Range(0, 1)]
    [SerializeField] public float enemyPoise;
    [SerializeField] public float enemyPoiseRecoverySpeed;
    [Space]
    [Range(0, 1)]
    [SerializeField] public float enemyNextAttack;
    [SerializeField] public float enemyNextAttackSpeed;
    [Space]
    [SerializeField] public float stunnedRecoveryTime;
    [Space]
    [SerializeField] public GameObject cutBodies_TopLeft;
    [SerializeField] public GameObject cutBodies_TopRight;
    [SerializeField] public GameObject[] cutBodies_HiddenFang;
    [SerializeField] public GameObject[] cutBodies_StealthKill;

    [Space]
    [Header("Patrol Variables")]
    [SerializeField] public bool isPatrolling;
    [SerializeField] public Transform currentPatrolTransform;
    [SerializeField] public Transform[] patrolTransforms;
    [Range(0,100)]
    [SerializeField] public int patrolIndex;

    [Space]
    [Header("Aware of player variables")]
    [SerializeField] public float facePlayerRotationSpeed;
    [SerializeField] public EnemyController[] awarenessGroup;

    [Space]
    [Header("QTE")]
    [SerializeField] public GameObject currentQTEBackground;

    /*******************************************************************************************************************************/

    /// <summary>
    /// ENUM
    /// </summary>
    public enum EnemyState
    {
        alive,
        awareOfPlayer,
        waitingForCombat,
        inCombat,
        stunned,
        dead
    }

    /*******************************************************************************************************************************/

    /// <summary>
    /// METHODS
    /// </summary>

    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //patrol
        if(enemyState == EnemyState.alive)
        {
            Patrol();
        }

        //if the enemy is aware of the player
        if(enemyState == EnemyState.awareOfPlayer)
        {
            //do behaviour
            AwareOfPlayerBehaviour();
        }

        if (enemyState == EnemyState.inCombat || playerController.playerState == PlayerState.stealthKill)
        {
            //position the camera focus
            cameraFocus.transform.position = new Vector3((transform.position.x + playerController.transform.position.x) / 2,
                                                         (transform.position.y + playerController.transform.position.y) / 2,
                                                         (transform.position.z + playerController.transform.position.z) / 2);
        }

        if (enemyState == EnemyState.inCombat)
        {
            //engage the player
            Engage();
        }
    }

    /******************************************************************************************************************************************/
    /// <summary>
    /// PATROL
    /// </summary>
    public void Patrol()
    {
        //if the patrolling variable is active
        if (isPatrolling)
        {
            //get patrol point if has none
            if (currentPatrolTransform == null)
            {
                currentPatrolTransform = patrolTransforms[patrolIndex];
            }

            //set the enemys destination
            agent.SetDestination(currentPatrolTransform.position);

            //move onto next patrol point if reached current objective
            if (Vector3.Distance(transform.position, currentPatrolTransform.position) < 0.1f)
            {
                if (patrolIndex == patrolTransforms.Length - 1)
                {
                    patrolIndex = 0;
                }
                else
                {
                    patrolIndex++;
                }

                currentPatrolTransform = patrolTransforms[patrolIndex];
            }
        }
    }


    /*********************************************************************************************************************************************/
    /// <summary>
    /// AWARE OF PLAYER
    /// </summary>
    public void AwareOfPlayer()
    {
        if (enemyState != EnemyState.awareOfPlayer)
        {
            SetEnemyState(EnemyState.awareOfPlayer);
            agent.stoppingDistance = 6f;
            enemyAnimator.SetTrigger("AwareOfPlayer");
        }
    }    

    //behaviour when the enemy is aware of the player
    private void AwareOfPlayerBehaviour()
    {
        //face player look at player
        Vector3 targetDirection = playerController.head.transform.position - head.transform.position;
        Quaternion toRotationHead = Quaternion.LookRotation(targetDirection, Vector3.up);
        head.transform.rotation = Quaternion.RotateTowards(head.transform.rotation, toRotationHead, facePlayerRotationSpeed * Time.deltaTime);

        //body turn towards player
        targetDirection = playerController.torsoe.transform.position - torsoe.transform.position;
        Quaternion toRotationTorsoe = Quaternion.LookRotation(targetDirection, Vector3.up);
        toRotationTorsoe.eulerAngles = new Vector3(0, toRotationTorsoe.eulerAngles.y, 0);
        torsoe.transform.rotation = Quaternion.RotateTowards(torsoe.transform.rotation, toRotationTorsoe, facePlayerRotationSpeed * Time.deltaTime);

        //go towards player
        agent.SetDestination(playerController.transform.position);

        //back off player if already engaged

        //make group aware
        foreach(EnemyController enemy in awarenessGroup)
        {
            if (enemy.enemyState == EnemyState.alive)
            {
                enemy.AwareOfPlayer();
            }
        }
    }

    /*******************************************************************************************************************************************/
    /// <summary>
    /// COMBAT
    /// </summary>
    public void Engage()
    {
        //face player look at player
        Vector3 targetDirection = playerController.head.transform.position - head.transform.position;
        Quaternion toRotationHead = Quaternion.LookRotation(targetDirection, Vector3.up);
        head.transform.rotation = Quaternion.RotateTowards(head.transform.rotation, toRotationHead, facePlayerRotationSpeed * Time.deltaTime);

        //body turn towards player
        targetDirection = playerController.torsoe.transform.position - torsoe.transform.position;
        Quaternion toRotationTorsoe = Quaternion.LookRotation(targetDirection, Vector3.up);
        toRotationTorsoe.eulerAngles = new Vector3(0, toRotationTorsoe.eulerAngles.y, 0);
        torsoe.transform.rotation = Quaternion.RotateTowards(torsoe.transform.rotation, toRotationTorsoe, facePlayerRotationSpeed * Time.deltaTime);

        //set players state
        if (playerController.playerState != PlayerState.combat)
        {
            playerController.StartCombat(gameObject);
        }
    }


    /**************************************************************************************************************************************************/
    /// <summary>
    /// UTILITY METHODS
    /// </summary>
    public void SetEnemyState(EnemyState newState)
    {
        Debug.Log("Set enemy state to: " + newState);
        enemyState = newState;
    }
}
