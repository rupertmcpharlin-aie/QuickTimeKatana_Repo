using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] public GameObject enemyMeshes;
    [SerializeField] public GameObject torsoe;
    [SerializeField] public GameObject head;
    [SerializeField] PlayerController playerController;
    [SerializeField] public GameObject cameraFocus;
    [SerializeField] public NavMeshAgent agent;
    //[SerializeField] Animator enemyAnimator;

    [Header("Combat Variables")]
    [SerializeField] public bool enemyAlive = true;
    [SerializeField] public bool enemyInCombat;
    [Space]
    [Range(0, 1)]
    [SerializeField] public float enemyPoise;
    [SerializeField] public float enemyPoiseRecoverySpeed;
    [Space]
    [Range(0, 1)]
    [SerializeField] public float enemyNextAttack;
    [SerializeField] public float enemyNextAttackSpeed;
    [Space]
    [SerializeField] public GameObject cutBody;

    [Space]
    [Header("Patrol Variables")]
    [SerializeField] public bool isPatrolling;
    [SerializeField] public Transform[] patrolTransforms;
    [Range(0,100)]
    [SerializeField] public float patrolSpeed;

    [Space]
    [Header("Aware of player variables")]
    [SerializeField] public bool enemyAwareOfPlayer;
    [SerializeField] public float noticePlayerWaitTime;
    [SerializeField] public float chaseSpeed;
    [SerializeField] public float facePlayerRotationSpeed;

    [Space]
    [Header("QTE")]
    [SerializeField] public GameObject currentQTEBackground;


    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyInCombat)
        {
            EnemyCombat();
        }

        if(enemyAwareOfPlayer)
        {
            AwareOfPlayerBehaviour();
        }

        if (enemyAlive)
        {
            cameraFocus.transform.position = new Vector3((transform.position.x + playerController.transform.position.x) / 2,
                                                         (transform.position.y + playerController.transform.position.y) / 2,
                                                         (transform.position.z + playerController.transform.position.z) / 2);
        }
    }

    private void AwareOfPlayerBehaviour()
    {
        //face player
        Vector3 targetDirection = playerController.head.transform.position - head.transform.position;
        Quaternion toRotationHead = Quaternion.LookRotation(targetDirection, Vector3.up);
        head.transform.rotation = Quaternion.RotateTowards(head.transform.rotation, toRotationHead, facePlayerRotationSpeed * Time.deltaTime);

        targetDirection = playerController.torsoe.transform.position - torsoe.transform.position;
        Quaternion toRotationTorsoe = Quaternion.LookRotation(targetDirection, Vector3.up);
        toRotationTorsoe.eulerAngles = new Vector3(0, toRotationTorsoe.eulerAngles.y, 0);
        torsoe.transform.rotation = Quaternion.RotateTowards(torsoe.transform.rotation, toRotationTorsoe, facePlayerRotationSpeed * Time.deltaTime);

        agent.SetDestination(playerController.transform.position);
    }

    public void EnemyCombat()
    { 
        
    }

    public void Engage()
    {
        if (!playerController.inCombat)
        {
            Debug.Log("Engage!");
            playerController.inCombat = true;
            playerController.engagedEnemy = gameObject;
            enemyInCombat = true;
        }
    }
}
