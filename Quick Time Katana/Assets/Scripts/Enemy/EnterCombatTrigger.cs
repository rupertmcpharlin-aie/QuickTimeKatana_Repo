using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterCombatTrigger : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Trigger enter");

        if (collision.gameObject.tag == "Player")
        {
            playerController.inCombat = true;
        }
    }
}
