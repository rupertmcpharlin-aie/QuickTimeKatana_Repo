using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSunTransition : MonoBehaviour
{
    [SerializeField] SunScript sunScript;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerController")
        {
            sunScript.isTransitioning = true;
        }
    }
}
