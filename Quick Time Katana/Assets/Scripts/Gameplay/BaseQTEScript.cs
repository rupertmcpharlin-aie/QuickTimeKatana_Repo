using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class BaseQTEScript : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] PlayerController playerController;
    [SerializeField] EnemyController enemyController;

    [Space]
    [Header("QTE System")]
    [SerializeField] GameObject[] QTEElements;
    [SerializeField] GameObject currentQTEBackground;
    [SerializeField] GameObject currentQTEElement;
    [SerializeField] string currentQTEElementValue;

    [Space]
    [Header("Hidden fang")]
    [SerializeField] bool isHiddenFangActive;
    [SerializeField] int hiddenFangIndex;
    [SerializeField] HiddenFang hiddenFangScript;   
    

    /*[Space]
    [Space]
    [Header("Audio")]
    [SerializeField] AudioSource swordSource;
    [SerializeField] AudioClip[] swordHits;
    [SerializeField] float swordPitchRange;
    [SerializeField] AudioSource killSource;
    [SerializeField] AudioClip[] killSFX;*/

    /// <summary>
    /// START
    /// </summary>
    void Start()
    {
        
    }

    /// <summary>
    /// UPDATE
    /// </summary>
    void Update()
    {
        //runs while the player is in combat and not stunned and the enemy is aware of the player
        if (!playerController.playerStunned && playerController.inCombat)
        {
            //gets inputs from the player
            CombatInputManager();
        }

        //runs while enemy is alive
        if(enemyController.enemyAlive && enemyController.enemyInCombat)
        {
            //enemy behaviours
            EnemyBehaviour();

            //manages qte
            MakeQTEELements();
        }
    }

    /// <summary>
    /// ENEMY
    /// </summary>
    //controls the enemies behaviour
    private void EnemyBehaviour()
    {

        //while the enemy is alive
        if (enemyController.enemyAlive && enemyController.enemyInCombat)
        {
            //refill poise
            enemyController.enemyPoise += Time.deltaTime / enemyController.enemyPoiseRecoverySpeed;
            enemyController.enemyPoise = Mathf.Clamp01(enemyController.enemyPoise);

            //ready next attack
            enemyController.enemyNextAttack += Time.deltaTime / enemyController.enemyNextAttackSpeed;
            enemyController.enemyNextAttack = Mathf.Clamp01(enemyController.enemyNextAttack);
        }

        //if next attack reaches completion
        if (enemyController.enemyNextAttack == 1)
        {
            //kill player
            KillPlayer();
        }
    }

    //MAKE QTE Elements
    private void MakeQTEELements()
    {
        //if there is no current element
        if (currentQTEElement == null && !isHiddenFangActive)
        {
            Debug.Log("TEST");
            //instantiate
            currentQTEElement = Instantiate(QTEElements[Random.Range(0, QTEElements.Length)], currentQTEBackground.transform);
        }
        else if (currentQTEElement == null && isHiddenFangActive)
        {
            //instantiate
            currentQTEElement = Instantiate(hiddenFangScript.hiddenFangs[hiddenFangIndex][Random.Range(0, hiddenFangScript.hiddenFangs[hiddenFangIndex].Length)], currentQTEBackground.transform);
            hiddenFangIndex++;
        }

        //reassign value 
        currentQTEElementValue = currentQTEElement.GetComponent<QTEElementScript>().QTEElementValue;

        //reassign its position
        currentQTEElement.transform.position = currentQTEBackground.transform.position;
    }

    /// <summary>
    /// PLAYER
    /// </summary>
    //Manages player input
    private void CombatInputManager()
    {
        //CORRECT INPUT
        if (Input.GetButtonDown(currentQTEElementValue))
        {
            StartCoroutine("CorrectInputFeedback");

            /*//play metal SFX
            swordSource.pitch = 1;
            swordSource.clip = swordHits[Random.Range(0, swordHits.Length)];
            float pitchRandom = Random.Range(-swordPitchRange, swordPitchRange);
            swordSource.pitch += pitchRandom;
            swordSource.Play();*/

            //destroy current QTE Element and reset value
            DestroyCurrentQTEElement();

            //NORMAL BENHAVIOUR
            if (!isHiddenFangActive)
            {
                //playerController.damage
                //if playerController.damage is greater than the amount of poise enemy has left
                if (enemyController.enemyPoise < playerController.damage)
                {
                    //kill enemy
                    KillEnemy();
                }
                else
                {
                    //do playerController.damage
                    enemyController.enemyPoise -= playerController.damage;
                    enemyController.enemyNextAttack = 0;
                }
            }
            //HIDDEN FANG BEHAVIOUR
            else
            {
                if(hiddenFangIndex == hiddenFangScript.hiddenFangs.Length && enemyController.enemyNextAttack > 0)
                {
                    hiddenFangIndex = 0;
                    isHiddenFangActive = false;
                    currentQTEBackground.GetComponent<Image>().color = Color.black;

                    //kill enemy
                    KillEnemy();
                }
            }
        }

        //INCORRECT INPUT
        foreach(GameObject QTEElement in QTEElements)
        {
            string tempString = QTEElement.GetComponent<QTEElementScript>().QTEElementValue;
            //if there is a current value
            if (currentQTEElementValue != null)
            {
                //buttons
                if (tempString != currentQTEElementValue)
                {
                    if (Input.GetButtonDown(tempString))
                    {
                        StartCoroutine("WrongInput");
                    }
                }
            }
        }

        //hidden fang
        if(Input.GetButtonDown("LeftStickDown"))
        {
            StartCoroutine("CorrectInputFeedback");

            //destroy current QTEElement
            DestroyCurrentQTEElement();

            //reset enemy next attack buffer
            enemyController.enemyNextAttack = 0;

            //set hidden fang variables
            isHiddenFangActive = true;
            hiddenFangIndex = 0;
        }

        //set colour of background to yellow during hidden fang
        if(isHiddenFangActive && currentQTEBackground.GetComponent<Image>().color != Color.yellow)
        {
            //change background of QTE
            currentQTEBackground.GetComponent<Image>().color = Color.yellow;
        }
    }

    //stuns the player if they press the wrong input
    public IEnumerator WrongInput()
    {
        enemyController.enemyPoise = 1;
        currentQTEBackground.GetComponent<Image>().color = Color.red;
        playerController.playerStunned = true;

        yield return new WaitForSeconds(.5f);

        currentQTEBackground.GetComponent<Image>().color = Color.black;
        playerController.playerStunned = false;
    }

    /// <summary>
    /// UTILITY METHODS
    /// </summary>
    //DESTROYS THE CURRENT QTE ELEMENT AND RESETS VALUE
    private void DestroyCurrentQTEElement()
    {
        Destroy(currentQTEElement);
        currentQTEElementValue = null;
    }

    //KILLS THE ENEMY
    private void KillEnemy()
    {        
        DestroyCurrentQTEElement();

        enemyController.enemyAlive = false;
        enemyController.enemyPoise = 0;
        enemyController.enemyNextAttack = 0;
    }

    //KILLS THE PLAYER
    private void KillPlayer()
    {
        DestroyCurrentQTEElement();

        Debug.Log("in combat = false 1");
        playerController.inCombat = false;
        currentQTEBackground.GetComponent<Image>().color = Color.black;
    }

    //STARTS COMBAT
    public void StartCombat()
    {
        playerController.inCombat = true;
    }

    //player feedback correct input
    IEnumerator CorrectInputFeedback()
    {
        currentQTEBackground.GetComponent<Image>().color = Color.green;
        yield return new WaitForSeconds(0.1f);
        currentQTEBackground.GetComponent<Image>().color = Color.black;
    }
}
