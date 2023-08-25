using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimationTrigger : MonoBehaviour
{
    [SerializeField] NavMeshAgent[] maids;
    [SerializeField] Transform maidDestination;

    [Space]

    [SerializeField] string dialogueTrigger;
    [SerializeField] bool dialogueTriggered;

    [Space]

    [SerializeField] EnemyController enemy;

    private void Update()
    {
        if (enemy.enemyState == EnemyController.EnemyState.dead)
        {
            foreach (NavMeshAgent maid in maids)
            {
                maid.SetDestination(maidDestination.position);
            }

            if (!dialogueTriggered)
            {
                dialogueTriggered = true;
                Fungus.Flowchart.BroadcastFungusMessage(dialogueTrigger);
            }
        }
    }

}
