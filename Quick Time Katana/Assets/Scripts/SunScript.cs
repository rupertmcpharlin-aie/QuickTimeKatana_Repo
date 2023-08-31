using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour
{
    [SerializeField] public bool isTransitioning;
    [SerializeField] public float startTime;
    [SerializeField] public float duration;
    [SerializeField] public float timer;
    [SerializeField] public float t;
    [Space]

    [SerializeField] public Vector3 quat_Sunset;
    [SerializeField] public float intensity_Sunset;
    [Space]
    [SerializeField] public Vector3 quat_Dark;
    [SerializeField] public float intensity_Dark;

    // Start is called before the first frame update
    void Start()
    {
        /*GetComponent<Light>().intensity = intensity_Sunset;
        gameObject.transform.rotation = Quaternion.Euler(Vector3.Lerp(quat_Sunset, quat_Dark, t));*/
    }

    // Update is called once per frame
    void Update()
    {
        

        if(isTransitioning)
        {
            t = (timer - startTime) / duration;

            GetComponent<Light>().intensity = Mathf.Lerp(intensity_Sunset, intensity_Dark, t);
            gameObject.transform.rotation = Quaternion.Euler(Vector3.Lerp(quat_Sunset, quat_Dark, t));

            timer += Time.deltaTime;
        }

        
    }
}
