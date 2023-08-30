using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class checkpoint : MonoBehaviour
{
    [SerializeField] checkpoints checkpoints;
    [SerializeField] bool isActiveCheckpoint;
    [Space]
    [SerializeField] UnityEvent action;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerController" && isActiveCheckpoint)
        {
            action.Invoke();
            isActiveCheckpoint = false;
        }
    }
}
