using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] public GameObject body;
    [SerializeField] public GameObject head;
    [SerializeField] PlayerController playerController;
    [SerializeField] public GameObject cameraFocus;
    //[SerializeField] Animator enemyAnimator;

    [Header("Combat Variables")]
    [SerializeField] public bool isAlive = true;
    [SerializeField] public bool inCombat;    

    [Space]
    [Header("Patrol Variables")]
    [SerializeField] public bool isPatrolling;
    [SerializeField] public Transform[] patrolTransforms;
    [Range(0,100)]
    [SerializeField] public float patrolSpeed;

    [Space]
    [Header("Aware of player variables")]
    [SerializeField] public bool awareOfPlayer;
    [SerializeField] public float noticePlayerWaitTime;
    [SerializeField] public float chaseSpeed;
    [SerializeField] public float facePlayerRotationSpeed;



    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(inCombat)
        {
            EnemyCombat();
        }

        if(awareOfPlayer)
        {
            AwareOfPlayerBehaviour();
        }

        if (isAlive)
        {
            cameraFocus.transform.position = new Vector3((playerController.transform.position.x - transform.position.x)/2,
                                                            (playerController.transform.position.y - transform.position.y)/2,
                                                            (playerController.transform.position.z - transform.position.z)/2);
        }
    }

    private void AwareOfPlayerBehaviour()
    {
        //face player
        Vector3 targetDirection = playerController.transform.position - body.transform.position;
        Quaternion toRotationHead = Quaternion.LookRotation(targetDirection, Vector3.up);
        head.transform.rotation = Quaternion.RotateTowards(head.transform.rotation, toRotationHead, facePlayerRotationSpeed * Time.deltaTime);
    }

    public void EnemyCombat()
    { 
        
    }
}
