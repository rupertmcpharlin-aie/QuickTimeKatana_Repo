using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class BaseQTEScript : MonoBehaviour
{
    [Header("Player stuff")]
    [SerializeField] GameObject[] QTEElements;
    [SerializeField] TextMeshProUGUI whatHappened;
    [SerializeField] bool inCombat;
    [SerializeField] bool playerStunned;
    [SerializeField] float damage;
    [Space]
    [Header("Hidden fang")]
    [SerializeField] bool hiddenFang;
    [SerializeField] int hiddenFangIndex;
    [SerializeField] GameObject[][] hiddenFangQTE;
    [Space]
    [Header("QTE")]
    [SerializeField] GameObject currentQTEBackground;
    [SerializeField] GameObject currentQTEElement;
    [SerializeField] string currentQTEElementValue;
    [Space]
    [Header("Enemy")]
    [SerializeField] bool enemyAlive;
    [SerializeField] GameObject enemyPoise;
    [SerializeField] float enemyPoiseRecoverySpeed;
    [Space]
    [SerializeField] GameObject enemyNextAttack;
    [SerializeField] float enemyNextAttackSpeed;
    [Space]
    [SerializeField] ParticleSystem hitPS;
    [Space]
    [Header("Audio")]
    [SerializeField] AudioSource swordSource;
    [SerializeField] AudioClip[] swordHits;
    [SerializeField] float swordPitchRange;
    [SerializeField] AudioSource killSource;
    [SerializeField] AudioClip[] killSFX;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerStunned && inCombat)
        {
            InputManager();
            RespawnEnemyMethod();
        }

        if(enemyAlive)
        {
            MakeQTEELements();
            EnemyBehaviour();
        }

        if(!inCombat && Input.GetButtonDown("ButtonDown"))
        {
            inCombat = true;
            enemyAlive = true;
            inCombat = true;
            enemyPoise.GetComponent<Image>().fillAmount = 1;
            enemyNextAttack.GetComponent<Image>().fillAmount = 0;
            whatHappened.text = "";
        }

    }

    public void RespawnEnemyMethod()
    {
        if(!enemyAlive)
        {
            StartCoroutine("RespawnEnemy");
        }
    }

    private void EnemyBehaviour()
    {
        //while the enemy is alive
        if (enemyAlive)
        {
            //refill poise
            enemyPoise.GetComponent<Image>().fillAmount += Time.deltaTime / enemyPoiseRecoverySpeed;

            //next attack
            enemyNextAttack.GetComponent<Image>().fillAmount += Time.deltaTime / enemyNextAttackSpeed;
        }

        if (enemyNextAttack.GetComponent<Image>().fillAmount == 1)
        {
            //kill player
            whatHappened.text = "u ded lol\npress A to restart";
            inCombat = false;
        }        
    }

    public IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(1f);
        enemyAlive = true;
        inCombat = true;
        enemyPoise.GetComponent<Image>().fillAmount = 1;
        enemyNextAttack.GetComponent<Image>().fillAmount = 0;
    }

    private void InputManager()
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

            //destroy current QTE Element
            Destroy(currentQTEElement);

            //reset Current QTE Value
            currentQTEElementValue = null;

            //DAMAGE
            //if damage is greater than the amount of poise enemy has left
            if (enemyPoise.GetComponent<Image>().fillAmount < damage)
            {
                //kill enemy
                enemyAlive = false;
                enemyPoise.GetComponent<Image>().fillAmount = 0;
                enemyNextAttack.GetComponent<Image>().fillAmount = 0;

                killSource.clip = killSFX[0];
                killSource.Play();
            }
            else
            {
                //do damage
                enemyPoise.GetComponent<Image>().fillAmount -= damage;
                enemyNextAttack.GetComponent<Image>().fillAmount = 0;
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
            Debug.Log("LEFT STICK DOWN DETECTED");
            //reset next attack to 0
            enemyNextAttack.GetComponent<Image>().fillAmount = 0;
            hiddenFang = true;
            hiddenFangIndex = 0;
        }
    }

    private void MakeQTEELements()
    {
        //if there is no current element
        if (currentQTEElement == null && !hiddenFang)
        {
            //instantiate
            currentQTEElement = Instantiate(QTEElements[Random.Range(0, QTEElements.Length)], currentQTEBackground.transform);
        }
        else if(currentQTEElement == null && hiddenFang)
        {
            //instantiate
            currentQTEElement = Instantiate(hiddenFangQTE[hiddenFangIndex][Random.Range(0, hiddenFangQTE[hiddenFangIndex].Length)], currentQTEBackground.transform);
            hiddenFangIndex++;
        }

        //reassign value 
        currentQTEElementValue = currentQTEElement.GetComponent<QTEElementScript>().QTEElementValue;

        //reassign its position
        currentQTEElement.transform.position = currentQTEBackground.transform.position;


    }

    public void StartCombat()
    {
        inCombat = true;
    }

    public IEnumerator WrongInput()
    {
        currentQTEBackground.GetComponent<Image>().color = Color.red;
        whatHappened.text = "oops wrong input";
        playerStunned = true;

        yield return new WaitForSeconds(.5f);

        currentQTEBackground.GetComponent<Image>().color = Color.black;
        whatHappened.text = "";
        playerStunned = false;
    }
}
