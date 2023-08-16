using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static PlayerController;

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
    [SerializeField] public GameObject[] cutBodies;

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

        if(enemyInCombat)
        {
            Engage();
        }
    }

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
    }

    public void Engage()
    {
        if (playerController.playerState != PlayerState.combat)
        {
            playerController.StartCombat(gameObject);
        }
    }
}
