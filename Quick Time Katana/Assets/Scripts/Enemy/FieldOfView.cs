using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] EnemyController enemyController;

    public float FOVradius;

    [Range(0,360)]
    public float FOVangle;
    public float combatDistance;
    [Space]
    public Collider[] rangeChecks;
    [Space]
    public bool canSeePlayer;
    public GameObject playerRef;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    


    // Start is called before the first frame update
    void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine("FOVRoutine");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator FOVRoutine()
    {
        float delay = 0.2f;

        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        rangeChecks = Physics.OverlapSphere(transform.position, FOVradius, targetMask);

        //if player is in range of circle
        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;

            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

            //if player is in range of circle && inbetween angle
            if (angleToTarget < FOVangle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                //if player can be seen directly by enemy
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    if(enemyController.enemyState == EnemyController.EnemyState.alive)
                    {
                        Debug.Log("Set enemy state to aware");
                        enemyController.SetEnemyState(EnemyController.EnemyState.awareOfPlayer);
                    }
                    

                    if(distanceToTarget <= combatDistance && enemyController.enemyState == EnemyController.EnemyState.awareOfPlayer)
                    {
                        Debug.Log("Set enemy state to combat");
                        enemyController.SetEnemyState(EnemyController.EnemyState.inCombat);
                    }
                }
            }

            if(enemyController.playerController.playerState != PlayerController.PlayerState.crouched &&
                enemyController.playerController.playerState != PlayerController.PlayerState.stealthKill
                &&
                enemyController.enemyState != EnemyController.EnemyState.inCombat &&
                enemyController.enemyState != EnemyController.EnemyState.dead)
            {
                enemyController.SetEnemyState(EnemyController.EnemyState.awareOfPlayer);
            }
        }
    }
}
