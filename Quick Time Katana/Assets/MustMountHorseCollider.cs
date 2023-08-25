using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MustMountHorseCollider : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectWithTag("PlayerController") != null)
        {
            PlayerController playerController = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();

            if (playerController.playerState == PlayerController.PlayerState.mounted)
            {
                Destroy(gameObject);
            }
        }
    }
}
