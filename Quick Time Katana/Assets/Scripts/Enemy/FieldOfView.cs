using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] EnemyController enemyController;

    public float radius;

    [Range(0,360)]
    public float angle;
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
        rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        //if player is in range of circle
        if(rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - target.position).normalized;

            //if player is in range of circle && inbetween angle
            if(Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                
                //if player can be seen directly by enemy
                if(!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                    enemyController.awareOfPlayer = true;
                }

                else
                {
                    canSeePlayer = false;
                    enemyController.awareOfPlayer = false;
                }
            }
            else
            {
                canSeePlayer = false;
                enemyController.awareOfPlayer = false;
            }
        }

        else
        {
            if(enemyController.awareOfPlayer)
            {
                canSeePlayer = false;
                enemyController.awareOfPlayer = false;
            }
        }
    }
}
