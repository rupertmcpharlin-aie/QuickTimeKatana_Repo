using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class BaseQTEScript : MonoBehaviour
{
    [Header("QTE System")]
    [SerializeField] GameObject[] QTEElements;
    [SerializeField] GameObject currentQTEBackground;
    [SerializeField] GameObject currentQTEElement;
    [SerializeField] string currentQTEElementValue;
    [Header("Player")]
    [SerializeField] bool inCombat;
    [SerializeField] bool playerStunned;
    [SerializeField] float damage;
    [Space]
    [Header("Hidden fang")]
    [SerializeField] bool isHiddenFangActive;
    [SerializeField] int hiddenFangIndex;
    [SerializeField] HiddenFang hiddenFangScript;
    [Space]
    [Header("Enemy")]
    [SerializeField] bool enemyAlive;
    [SerializeField] bool enemyAwareOfPlayer;
    [SerializeField] GameObject enemyPoise;
    [SerializeField] float enemyPoiseRecoverySpeed;
    [Space]
    [SerializeField] GameObject enemyNextAttack;
    [SerializeField] float enemyNextAttackSpeed;

    [Space]
    [Space]
    [Header("Audio")]
    [SerializeField] AudioSource swordSource;
    [SerializeField] AudioClip[] swordHits;
    [SerializeField] float swordPitchRange;
    [SerializeField] AudioSource killSource;
    [SerializeField] AudioClip[] killSFX;
    [Header("UI")]
    [SerializeField] TextMeshProUGUI whatHappened;
    [SerializeField] ParticleSystem hitPS;

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
        if (!playerStunned && inCombat)
        {
            //gets inputs from the player
            CombatInputManager();
        }
        else if(!playerStunned && inCombat)
        {

        }



        //runs while enemy is alive
        if(enemyAlive)
        {
            //enemy behaviours
            EnemyBehaviour();

            //manages qte
            MakeQTEELements();
        }

        //TESTING STUFF
        //Restart combat
        if(!inCombat && Input.GetButtonDown("ButtonDown"))
        {
            //restarts combat
            RestartCombat();
        }
        //respawn enemy
        if(!enemyAlive)
        {
            if (Input.GetButtonDown("ButtonDown"))
            {
                //makes a new enemy on defeat
                whatHappened.text = "";
                StartCoroutine("RespawnEnemy");
            }
        }
    }

    /// <summary>
    /// ENEMY
    /// </summary>
    //controls the enemies behaviour
    private void EnemyBehaviour()
    {
        //while the enemy is alive
        if (enemyAlive)
        {
            //refill poise
            enemyPoise.GetComponent<Image>().fillAmount += Time.deltaTime / enemyPoiseRecoverySpeed;

            //ready next attack
            enemyNextAttack.GetComponent<Image>().fillAmount += Time.deltaTime / enemyNextAttackSpeed;
        }

        //if next attack reaches completion
        if (enemyNextAttack.GetComponent<Image>().fillAmount == 1)
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
            //play particle system
            hitPS.Play();

            //play metal SFX
            swordSource.pitch = 1;
            swordSource.clip = swordHits[Random.Range(0, swordHits.Length)];
            float pitchRandom = Random.Range(-swordPitchRange, swordPitchRange);
            swordSource.pitch += pitchRandom;
            swordSource.Play();

            //destroy current QTE Element and reset value
            DestroyCurrentQTEElement();

            //NORMAL BENHAVIOUR
            if (!isHiddenFangActive)
            {
                //DAMAGE
                //if damage is greater than the amount of poise enemy has left
                if (enemyPoise.GetComponent<Image>().fillAmount < damage)
                {
                    //kill enemy
                    KillEnemy();
                }
                else
                {
                    //do damage
                    enemyPoise.GetComponent<Image>().fillAmount -= damage;
                    enemyNextAttack.GetComponent<Image>().fillAmount = 0;
                }
            }
            //HIDDEN FANG BEHAVIOUR
            else
            {
                if(hiddenFangIndex == hiddenFangScript.hiddenFangs.Length && enemyNextAttack.GetComponent<Image>().fillAmount > 0)
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
            //destroy current QTEElement
            DestroyCurrentQTEElement();

            //reset enemy next attack buffer
            enemyNextAttack.GetComponent<Image>().fillAmount = 0;

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
        enemyPoise.GetComponent<Image>().fillAmount = 1;
        currentQTEBackground.GetComponent<Image>().color = Color.red;
        whatHappened.text = "oops wrong input";
        playerStunned = true;

        yield return new WaitForSeconds(.5f);

        currentQTEBackground.GetComponent<Image>().color = Color.black;
        whatHappened.text = "";
        playerStunned = false;
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
        //TESTING-TO-REMOVE
        whatHappened.text = "Enemy killed, press A to restart";
        
        DestroyCurrentQTEElement();

        enemyAlive = false;
        enemyPoise.GetComponent<Image>().fillAmount = 0;
        enemyNextAttack.GetComponent<Image>().fillAmount = 0;

        killSource.clip = killSFX[0];
        killSource.Play();
    }

    //KILLS THE PLAYER
    private void KillPlayer()
    {
        //TESTING-TO-REMOVE
        whatHappened.text = "u ded lol, press A to restart";

        DestroyCurrentQTEElement();
        inCombat = false;
        currentQTEBackground.GetComponent<Image>().color = Color.black;
    }

    //STARTS COMBAT
    public void StartCombat()
    {
        inCombat = true;
    }

    //resets variables
    private void RestartCombat()
    {
        inCombat = true;
        enemyAlive = true;
        enemyPoise.GetComponent<Image>().fillAmount = 1;
        enemyNextAttack.GetComponent<Image>().fillAmount = 0;
        whatHappened.text = "";
    }

    //RESPAWNS ENEMY 
    public IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(1f);
        enemyAlive = true;
        inCombat = true;
        enemyPoise.GetComponent<Image>().fillAmount = 1;
        enemyNextAttack.GetComponent<Image>().fillAmount = 0;
    }
}
