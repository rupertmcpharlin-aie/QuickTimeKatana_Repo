using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class checkpoints : MonoBehaviour
{
    [SerializeField] public PlayerController playerController;
    [SerializeField] public GameObject playerPrefab;

    [Space]
    [SerializeField] public Transform[] checkpointTransforms;
    [SerializeField] public bool[] checkpointBools;

    [Space]
    [SerializeField] public float transitionWaitTime;
    [SerializeField] public float fadeWaitTime;
    [SerializeField] public GameObject sceneTransitionBox;
    [Space]
    [SerializeField] public checkpoints[] checkpointSystems;




    // Start is called before the first frame update
    void Start()
    {
        checkpointSystems = FindObjectsOfType<checkpoints>();
        if (checkpointSystems.Length == 1)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(playerController == null)
        {
            Instantiate(playerPrefab, GetCurrentCheckpoint().position, GetCurrentCheckpoint().rotation, null);
            AssignScripts();
        }
    }

    public void SetCheckPoint(int checkPointIndex)
    {
        if(!checkpointBools[checkPointIndex])
        {
            checkpointBools[checkPointIndex] = true;
            playerController.PermaKillEnemies();
        }
    }

    public Transform GetCurrentCheckpoint()
    {        
        for(int i = checkpointBools.Length-1; i >= 0; i--)
        {
            if(checkpointBools[i])
            {
                return checkpointTransforms[i];
            }
        }

        return checkpointTransforms[0];
    }


    public IEnumerator DeathBehaviour()
    {

        yield return new WaitForSeconds(transitionWaitTime);

        sceneTransitionBox.GetComponent<Animator>().SetTrigger("Fade_In_Trigger");

        yield return new WaitForSeconds(fadeWaitTime);

        SceneManager.LoadScene(0);

        sceneTransitionBox.GetComponent<Animator>().SetTrigger("Fade_Out_Trigger");
        yield return new WaitForSeconds(fadeWaitTime);
    }

    public void AssignScripts()
    {
        playerController = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        playerController.SetCheckPointsScript(GetComponent<checkpoints>());
    }
}
