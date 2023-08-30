using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static PlayerController;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

public class QTEController : MonoBehaviour
{
    /// <summary>
    /// VARIABLES
    /// </summary>
    [Header("Scripts")]
    [SerializeField] public PlayerController playerController;
    [SerializeField] public EnemyController enemyController;

    [Space]
    [Header("QTE System")]
    [SerializeField] public GameObject[] QTEElements;
    [SerializeField] public GameObject currentQTEBackground;
    [SerializeField] public GameObject currentQTEElement;
    [SerializeField] string currentQTEElementValue;

    [Space]
    [Header("Hidden fang")]
    [SerializeField] HiddenFang hiddenFangScript;
    [SerializeField] bool isHiddenFangActive;
    [SerializeField] int hiddenFangIndex;

    [Space]
    [Header("Lady Harrington Kill")]
    [SerializeField] int harringtonSpareIndex;
    [SerializeField] int harringtonSpareMaxIndex;
    [Space]
    [SerializeField] float harringtonColorValue;
    [SerializeField] int harringtonRedMaxValue;
    [SerializeField] int harringtonRedMinValue;
    [SerializeField] float harringtonColourChangeSpeed;

    [Space]
    [Header("Audio")]
    [SerializeField] AudioSource swordSource;
    [SerializeField] AudioClip[] swordHits;
    [SerializeField] float swordPitchRange;
    [SerializeField] AudioSource killSource;
    [SerializeField] AudioClip[] killSFX;
    /**************************************************************************************************************************/

    /// <summary>
    /// UPDATE
    /// </summary>
    void Update()
    {
        //runs while the player is in combat and not stunned and the enemy is aware of the player
        if (!playerController.playerStunned && playerController.playerState == PlayerState.combat)
        {
            //gets inputs from the player
            CombatInputManager();

            //hidden fang
            HiddenFang();
        }

        //runs while enemy is alive
        if (enemyController != null)
        {
            if (enemyController.enemyState == EnemyController.EnemyState.inCombat ||
                enemyController.enemyState == EnemyController.EnemyState.stunned &&
                playerController.playerState != PlayerState.dead)
            {
                if (enemyController.enemyState == EnemyController.EnemyState.inCombat)
                {
                    //enemy behaviours
                    EnemyBehaviour();
                }

                //manages qte
                MakeCombatQTEELements();
            }
        }

        if (playerController.playerState == PlayerState.stealthKill && enemyController.enemyState != EnemyController.EnemyState.dead)
        {
            MakeCombatQTEELements();
            StealthInputManager();
        }

        if(playerController.playerState == PlayerState.cutSceneCombat)
        {
            MakeCombatQTEELements();
            CutSceneCombatInputManager();

            harringtonColorValue -= harringtonColourChangeSpeed * Time.deltaTime;

            currentQTEBackground.GetComponent<Image>().color = new Color(harringtonColorValue/255, 0, 0);

            if(harringtonColorValue <= harringtonRedMinValue)
            {
                StartCutScene_KillHarrington();
            }
        }
    }
    /*****************************************************************************************************************************/

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
                if (enemyController.enemyPoise < playerController.damage && enemyController.enemyState == EnemyController.EnemyState.inCombat)
                {
                    //stun enemy
                    StartCoroutine("EnemyStunned");
                }
                else if (enemyController.enemyState == EnemyController.EnemyState.stunned)
                {
                    //kill enemy
                    StartCoroutine("KillEnemyStanding");

                    //pick random kill animation
                    int random = Random.Range(0, 2);
                    if (random == 0)
                    {
                        playerController.animator.SetTrigger("StandardKill_TopLeftTrigger");
                    }
                    else
                    {
                        playerController.animator.SetTrigger("StandardKill_TopRightTrigger");
                    }
                }
                else
                {
                    //do playerController.damage
                    enemyController.enemyPoise -= playerController.damage;
                    enemyController.enemyNextAttack = 0;

                    //play player hit animation
                    playerController.animator.Play("Hit");
                    
                    //play enemy animation
                    enemyController.enemyAnimator.Play("Raise weapon", -1, 0.0f);

                    
                }
            }

            //HIDDEN FANG BEHAVIOUR
            else
            {
                //hidden fang kill
                if (hiddenFangIndex == hiddenFangScript.hiddenFangs.Length-1 && enemyController.enemyNextAttack < 1)
                {
                    //reset varaiables
                    hiddenFangIndex = 0;
                    isHiddenFangActive = false;

                    //kill enemy
                    StartCoroutine("KillEnemyStanding");
                    playerController.animator.SetTrigger("HiddenFangKillTrigger");
                }
                //move onto next hidden fang
                else if(hiddenFangIndex < hiddenFangScript.hiddenFangs.Length-1)
                {
                    hiddenFangIndex++;
                }
            }
        }

        //INCORRECT INPUT
        foreach (GameObject QTEElement in QTEElements)
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
                        StartCoroutine("WrongInputCombat");
                    }
                }
            }
        }        
    }

    //player stealth stuff
    private void StealthInputManager()
    {
        //CORRECT INPUT
        if (Input.GetButtonDown(currentQTEElementValue))
        {
            playerController.stealthHitsIndex++;
            StartCoroutine("CorrectInputFeedback");
            DestroyCurrentQTEElement();

            if (playerController.stealthHitsIndex == 4)
            {
                StartCoroutine("KillEnemyCrouched");
                playerController.animator.SetTrigger("StealthKill");
            }
        }

        //INCORRECT INPUT
        foreach (GameObject QTEElement in QTEElements)
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
                        //reset stealth hits
                        playerController.stealthHitsIndex = 0;

                        //destroy current element
                        DestroyCurrentQTEElement();

                        //start combat
                        playerController.StartCombat(playerController.engagedEnemy);
                        playerController.SetPlayerState(PlayerState.combat);
                        enemyController.SetEnemyState(EnemyController.EnemyState.inCombat);

                        //play player combat animation
                        playerController.animator.SetTrigger("StandTrigger");
                    }
                }
            }
        }
    }

    //controls hidden fang
    private void HiddenFang()
    {
        //hidden fang
        if (Input.GetButtonDown("LeftStickDown"))
        {
            //destroy current QTEElement
            DestroyCurrentQTEElement();

            //reset enemy next attack buffer
            enemyController.enemyNextAttack = 0;

            //set hidden fang variables
            isHiddenFangActive = true;
            hiddenFangIndex = 0;
        }

        //set colour of background to yellow during hidden fang
        if (isHiddenFangActive && currentQTEBackground.GetComponent<Image>().color != Color.yellow)
        {
            //change background of QTE
            currentQTEBackground.GetComponent<Image>().color = Color.yellow;
        }
    }

    private void CutSceneCombatInputManager()
    {
        //CORRECT INPUT
        if (Input.GetButtonDown(currentQTEElementValue))
        {
            //destroy current QTE Element and reset value
            DestroyCurrentQTEElement();

            //play spare harrington cutscene
            if (harringtonSpareIndex == harringtonSpareMaxIndex)
            {
                //play cutscene
                StartCutScene_SpareHarrington();
            }
            else
            {
                //INCREASE INDEX
                harringtonSpareIndex++;

                harringtonColorValue = harringtonRedMaxValue;
                currentQTEBackground.GetComponent<Image>().color = new Color(harringtonColorValue, 0, 0);
            }

        }

        //INCORRECT INPUT
        foreach (GameObject QTEElement in QTEElements)
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
                        StartCutScene_KillHarrington();
                    }
                }
            }
        }
    }

    public void StartCutScene_KillHarrington()
    {
        Debug.Log("Kill");
        finalCutSceneCommon();


        playerController.animator.SetTrigger("Harrington_Kill");
    }

    public void StartCutScene_SpareHarrington()
    {
        Debug.Log("Spare");
        finalCutSceneCommon();


        playerController.animator.SetTrigger("Harrington_Spare");

    }

    public void finalCutSceneCommon()
    {
        currentQTEBackground.SetActive(false);
        playerController.SetPlayerState(PlayerState.cutScene);
        playerController.CameraTransition_CutSceneCam();
        playerController.combatCamera.Follow = null;
    }

    /****************************************************************************************************************************/

    /// <summary>
    /// ENEMY
    /// </summary>
    //controls the enemies behaviour
    private void EnemyBehaviour()
    {
        //refill poise
        enemyController.enemyPoise += Time.deltaTime / enemyController.enemyPoiseRecoverySpeed;
        enemyController.enemyPoise = Mathf.Clamp01(enemyController.enemyPoise);

        //ready next attack
        enemyController.enemyNextAttack += Time.deltaTime / enemyController.enemyNextAttackSpeed;
        enemyController.enemyNextAttack = Mathf.Clamp01(enemyController.enemyNextAttack);

        //if next attack reaches completion
        if (enemyController.enemyNextAttack == 1)
        {
            //kill player
            if (playerController.playerState != PlayerState.dead)
            {
                StartCoroutine("KillPlayer");
            }
        }
    }

    //STUN THE ENEMY
    IEnumerator EnemyStunned()
    {
        //STUN ENEMY
        enemyController.enemyPoise = 0;
        enemyController.enemyNextAttack = 0;
        enemyController.SetEnemyState(EnemyController.EnemyState.stunned);

        yield return new WaitForSeconds(enemyController.stunnedRecoveryTime);

        if (enemyController.enemyState == EnemyController.EnemyState.stunned)
        {
            enemyController.SetEnemyState(EnemyController.EnemyState.inCombat);
        }
    }

    
    /*****************************************************************************************************************************/
    /// <summary>
    /// QTE STUFF
    /// </summary>
    
    //MAKE QTE Elements
    private void MakeCombatQTEELements()
    {
        //if there is no current element
        if (currentQTEElement == null && !isHiddenFangActive)
        {
            //instantiate
            currentQTEElement = Instantiate(QTEElements[Random.Range(0, QTEElements.Length)], currentQTEBackground.transform);
        }
        else if (currentQTEElement == null && isHiddenFangActive)
        {
            //instantiate
            currentQTEElement = Instantiate(hiddenFangScript.hiddenFangs[hiddenFangIndex][Random.Range(0, hiddenFangScript.hiddenFangs[hiddenFangIndex].Length)], currentQTEBackground.transform);
        }

        //reassign value 
        currentQTEElementValue = currentQTEElement.GetComponent<QTEElementScript>().QTEElementValue;

        //reassign its position
        currentQTEElement.transform.position = currentQTEBackground.transform.position;
    }

    //player feedback correct input
    IEnumerator CorrectInputFeedback()
    {
        //set colour to green
        currentQTEBackground.GetComponent<Image>().color = Color.green;

        //wait for a sec
        yield return new WaitForSeconds(0.1f);

        //reset colour
        if (playerController.playerState == PlayerState.combat)
        {
            currentQTEBackground.GetComponent<Image>().color = Color.black;
        }
        else if (playerController.playerState == PlayerState.stealthKill)
        {
            currentQTEBackground.GetComponent<Image>().color = Color.gray;
        }
    }

    //stuns the player if they press the wrong input
    public IEnumerator WrongInputCombat()
    {
        enemyController.enemyPoise = 1;
        currentQTEBackground.GetComponent<Image>().color = Color.red;
        playerController.playerStunned = true;

        yield return new WaitForSeconds(.5f);

        if(playerController.playerState == PlayerState.combat)
        {
            currentQTEBackground.GetComponent<Image>().color = Color.black;
            playerController.playerStunned = false;
        }
    }

    //DESTROYS THE CURRENT QTE ELEMENT AND RESETS VALUE
    private void DestroyCurrentQTEElement()
    {
        Destroy(currentQTEElement);
        currentQTEElementValue = null;
    }

    /***********************************************************************************************************/
    /// <summary>
    /// KILLLLLLLL
    /// </summary>


    //KILLS THE ENEMY
    IEnumerator KillEnemyStanding()
    {
        //remove qte stuff
        DestroyCurrentQTEElement();
        currentQTEBackground.SetActive(false);

        //deactivate player variables
        enemyController.SetEnemyState(EnemyController.EnemyState.dead);
        enemyController.enemyPoise = 0;
        enemyController.enemyNextAttack = 0;
        enemyController.gameObject.GetComponent<NavMeshAgent>().speed = 0;

        enemyController.enemyAnimator.playbackTime = 0;
        enemyController.enemyAnimator.SetBool("isAlive", false);


        yield return new WaitForSeconds(1f);

        //player variables
        GameObject.FindGameObjectWithTag("Checkpoints").GetComponent<checkpoints>().isEnemyAliveCurrent[enemyController.checkPointIndex] = false;
        playerController.animator.SetBool("RaiseWeapon_Bool", false);
        playerController.SetPlayerState(PlayerState.exploring);
        playerController.CameraTransition_FreeCam();
    }

    IEnumerator KillEnemyCrouched()
    {
        //remove qte stuff
        DestroyCurrentQTEElement();
        currentQTEBackground.SetActive(false);

        //deactivate player variables
        enemyController.SetEnemyState(EnemyController.EnemyState.dead);
        enemyController.enemyPoise = 0;
        enemyController.enemyNextAttack = 0;
        enemyController.gameObject.GetComponent<NavMeshAgent>().speed = 0;

        

        yield return new WaitForSeconds(1f);

        //player variables
        GameObject.FindGameObjectWithTag("Checkpoints").GetComponent<checkpoints>().isEnemyAliveCurrent[enemyController.checkPointIndex] = false;
        playerController.SetPlayerState(PlayerState.crouched);
        playerController.CameraTransition_FreeCam();
    }

    public void KillEnemy_Standard_TopRight()
    {
        //enable cut body
        enemyController.enemyMeshes.SetActive(false);
        enemyController.cutBodies_TopRight.SetActive(true);
    }

    public void KillEnemy_Standard_TopLeft()
    {
        //enable cut body
        enemyController.enemyMeshes.SetActive(false);
        enemyController.cutBodies_TopLeft.SetActive(true);
    }

    public void KillEnemyStealth()
    {
        //enable cut body
        enemyController.enemyMeshes.SetActive(false);
        enemyController.cutBodies_StealthKill[0].SetActive(true);
    }

    public void KillEnemyHiddenFang()
    {
        //enable cut body
        enemyController.enemyMeshes.SetActive(false);
        enemyController.cutBodies_HiddenFang[0].SetActive(true);
    }

    //KILLS THE PLAYER
    IEnumerator KillPlayer()
    {
        DestroyCurrentQTEElement();
        playerController.SetPlayerState(PlayerState.dead);
        currentQTEBackground.SetActive(false);

        yield return new WaitForSeconds(1f);
    }

    
}
