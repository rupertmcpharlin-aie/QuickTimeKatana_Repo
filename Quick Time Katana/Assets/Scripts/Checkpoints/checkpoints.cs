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
    [SerializeField] public GameObject[] enemies;
    [SerializeField] public bool[] isEnemyAliveCurrent;
    [SerializeField] public bool[] isEnemyAliveWorld;

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

        if(isEnemyAliveCurrent.Length == 0)
        {
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            isEnemyAliveCurrent = new bool[enemies.Length];
            isEnemyAliveWorld = new bool[enemies.Length];

            for(int i = 0; i < enemies.Length; i++)
            {
                isEnemyAliveCurrent[i] = true;
                isEnemyAliveWorld[i] = true;

                if (enemies[i].GetComponent<EnemyController>() != null)
                {
                    enemies[i].GetComponent<EnemyController>().checkPointIndex = i;
                }
            }
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

            for(int i = 0; i < isEnemyAliveWorld.Length; i++)
            {
                isEnemyAliveWorld[i] = isEnemyAliveCurrent[i];
            }
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

        yield return new WaitForSeconds(.1f);

        DespawnDeadEnemies();
        sceneTransitionBox.GetComponent<Animator>().SetTrigger("Fade_Out_Trigger");

        yield return new WaitForSeconds(fadeWaitTime);
    }

    public void AssignScripts()
    {
        playerController = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
    }

    public void DespawnDeadEnemies()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            if (!isEnemyAliveWorld[enemies[i].GetComponent<EnemyController>().checkPointIndex])
            {
                Destroy(enemies[i]);
            }
        }
    }
}
