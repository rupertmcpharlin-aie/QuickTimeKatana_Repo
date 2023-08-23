using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerController")
        {
            other.GetComponent<PlayerController>().isNearHorse = true;
            other.GetComponent<PlayerController>().horse = gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "PlayerController")
        {
            other.GetComponent<PlayerController>().isNearHorse = false;
            other.GetComponent<PlayerController>().horse = null;
        }
    }

    public void StartWalkingAnimation()
    {
        GetComponent<Animator>().SetTrigger("Mounted");
    }

    public void StartIdleAnimation()
    {
        GetComponent<Animator>().SetTrigger("UnMounted");
    }
}
