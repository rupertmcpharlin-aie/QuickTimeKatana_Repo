using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class decapitationImageFillScript : MonoBehaviour
{
    public float multiplier;
    Image image;



    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (image.fillAmount < 1)
        {
            image.fillAmount += multiplier * Time.deltaTime;
        }
    }
}
