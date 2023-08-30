using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableTrigger : MonoBehaviour
{
    [SerializeField] public bool isCloseToTable;
    [SerializeField] public bool hasBeenTriggered;
    [SerializeField] public Transform cutSceneTransform;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerController")
        {
            isCloseToTable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PlayerController")
        {
            isCloseToTable = false;
        }
    }

    private void Update()
    {
        if (isCloseToTable && Input.GetButtonDown("ButtonDown") && !hasBeenTriggered)
        {
            hasBeenTriggered = true;
            GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>().StartLadyHarringtonCutScene(cutSceneTransform);

        }
    }

    public void LadyHarrington_DialogueTrigger_3()
    {
        StartCoroutine("DialogueCoroutine3");
        
    }


    IEnumerator DialogueCoroutine3()
    {
        GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>().animator.SetTrigger("AngryAtLady");
        yield return new WaitForSeconds(3f);
        Fungus.Flowchart.BroadcastFungusMessage("Lady Harrington ~ Dialogue 3");
    }
}
