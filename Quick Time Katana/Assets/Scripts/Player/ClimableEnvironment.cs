using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimableEnvironment : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "PlayerController")
        {
            other.gameObject.GetComponent<PlayerController>().canClimb = true;
            Physics.gravity = new Vector3(0, 0, 0);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "PlayerController")
        {
            other.gameObject.GetComponent<PlayerController>().canClimb = false;
            Physics.gravity = new Vector3(0, -9.81f, 0);
        }
    }

}
