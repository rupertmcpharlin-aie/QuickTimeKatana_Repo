using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] public GameObject body;
    [SerializeField] PlayerController playerController;
    [SerializeField] public GameObject cameraFocus;
    //[SerializeField] Animator enemyAnimator;

    [Header("Combat Variables")]
    [SerializeField] public bool awareOfPlayer;
    [SerializeField] public bool inCombat;
    [SerializeField] public bool facingPlayer;
    [SerializeField] public bool isAlive = true;

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
            //enemyAnimator.SetBool("Seen Player", true);
        }

        if(isAlive)
        {
            cameraFocus.transform.position = new Vector3((playerController.transform.position.x - transform.position.x)/2,
                                                            (playerController.transform.position.y - transform.position.y)/2,
                                                            (playerController.transform.position.z - transform.position.z)/2);
        }
    }



    public void EnemyCombat()
    { 
        if(!facingPlayer)
        {

        }
    }
}
