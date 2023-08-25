using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class checkpoint : MonoBehaviour
{
    [SerializeField] checkpoints checkpoints;
    [Space]
    [SerializeField] UnityEvent action;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerController")
        {
            action.Invoke();
            Destroy(gameObject);
        }
    }
}
