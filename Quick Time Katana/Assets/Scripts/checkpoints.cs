using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class checkpoints : MonoBehaviour
{
    [SerializeField] public Transform[] checkpointTransforms;
    [SerializeField] public bool[] checkpointBools;

    [Space]
    [SerializeField] public GameObject sceneTransitionBox;



    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCheckPoint(int checkPointIndex)
    {
        checkpointBools[checkPointIndex] = true;
    }

    public Transform GetCurrentCheckpoint()
    {
        int index = 0;
        foreach(bool checkpointBool in checkpointBools)
        {
            if(checkpointBool)
            {
                index++;
            }
            else
            {
                break;
            }
        }

        return checkpointTransforms[index];
    }


    public IEnumerator DeathBehaviour()
    {
        yield return new WaitForSeconds(10f);
        sceneTransitionBox.GetComponent<Animator>().SetTrigger("Fade_In_Trigger");
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(0);

    }


}
