using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadyHarringtonScript : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    
    [SerializeField] GameObject head;
    [SerializeField] GameObject currentQTEBackground;

    [SerializeField] GameObject meshes;
    [SerializeField] GameObject cutBody;

    [SerializeField] public bool inQTE;

    private void Update()
    {
        if(playerController == null && GameObject.FindGameObjectWithTag("PlayerController") != null)
        {
            playerController = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        }

        if (playerController != null)
        {
            Vector3 targetDirection = playerController.head.transform.position - head.transform.position;
            Quaternion toRotationHead = Quaternion.LookRotation(targetDirection, Vector3.up);
            head.transform.rotation = Quaternion.RotateTowards(head.transform.rotation, toRotationHead, 5f);
        }
    }

    public void SetinQTE()
    {
        inQTE = true;
        playerController.SetPlayerState(PlayerController.PlayerState.cutSceneCombat);

        currentQTEBackground.SetActive(true);
        playerController.qteController.currentQTEBackground = currentQTEBackground;

        playerController.combatCamera.Follow = playerController.transform;
        playerController.combatCamera.LookAt = playerController.transform;
    }

    public void SwapBodies()
    {
        meshes.SetActive(false);
        cutBody.SetActive(true);
    }




}
