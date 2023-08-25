using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class TriggerDialogue : MonoBehaviour
{
    [SerializeField] public string blockTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerController")
        {
            Fungus.Flowchart.BroadcastFungusMessage(blockTrigger);
        }
    }
}
