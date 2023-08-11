using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TreeRandomizer : MonoBehaviour
{
    [SerializeField] float yHeightMin;
    [SerializeField] float yHeightMax;

    [Space]

    [SerializeField] float xRotMin;
    [SerializeField] float xRotMax;
    [SerializeField] float yRotMin;
    [SerializeField] float yRotMax;
    [SerializeField] float zRotMin;
    [SerializeField] float zRotMax;

    [Space]

    [SerializeField] float xScaleMin;
    [SerializeField] float xScaleMax;
    [SerializeField] float yScaleMin;
    [SerializeField] float yScaleMax;
    [SerializeField] float zScaleMin;
    [SerializeField] float zScaleMax;

    // Start is called before the first frame update
    void Start()
    {
        float yOffset = Random.Range(yHeightMin, yHeightMax);
        Vector3 rotation = new Vector3(Random.Range(xRotMin, xRotMax), Random.Range(yRotMin, yRotMax), Random.Range(zRotMin, zRotMax));
        Vector3 scale = new Vector3(Random.Range(xScaleMin, xScaleMax), Random.Range(yScaleMin, yScaleMax), Random.Range(zScaleMin, zScaleMax));

        gameObject.transform.rotation = Quaternion.Euler(rotation);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x,
                                                    gameObject.transform.position.y + yOffset,
                                                    gameObject.transform.position.z);
        gameObject.transform.localScale = scale;
    }
}
