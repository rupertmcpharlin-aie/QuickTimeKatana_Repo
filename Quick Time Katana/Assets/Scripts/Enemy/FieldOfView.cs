using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius;

    [Range(0,360)]
    public float angle;
    [Space]
    public Collider[] rangeChecks;
    public bool canSeePlayer;
    [Space]
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

        if(rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - target.position).normalized;

            if(Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                
                if(!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                }
                else
                {
                    canSeePlayer = false;
                }
            }
            else
            {
                canSeePlayer = false;
            }
        }

        else
        {
            if(canSeePlayer)
            {
                canSeePlayer = false;
            }
        }
    }
}
