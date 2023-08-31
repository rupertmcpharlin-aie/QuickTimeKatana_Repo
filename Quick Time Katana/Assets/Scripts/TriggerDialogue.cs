using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class TriggerDialogue : MonoBehaviour
{
    [SerializeField] public string blockTrigger;
    [SerializeField] public bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerController" && hasTriggered == false)
        {
            hasTriggered = true;
            Fungus.Flowchart.BroadcastFungusMessage(blockTrigger);
        }
    }
}
