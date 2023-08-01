using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] GameObject body;
    [SerializeField] PlayerController playerController;
    [SerializeField] Animator enemyAnimator;

    [Header("Combat Variables")]
    [SerializeField] public bool awareOfPlayer;
    [SerializeField] public bool inCombat;
    [SerializeField] public bool facingPlayer;

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
            enemyAnimator.SetTrigger("SeenPlayer");
        }
    }



    public void EnemyCombat()
    { 
        if(!facingPlayer)
        {

        }
    }
}
