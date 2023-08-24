using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeRandomizer : MonoBehaviour
{
    [SerializeField] float xRotMin;
    [SerializeField] float xRotMax;

    [SerializeField] float yRotMin;
    [SerializeField] float yRotMax;

    [SerializeField] float zRotMin;
    [SerializeField] float zRotMax;

    [SerializeField] float xScaleMin;
    [SerializeField] float xScaleMax;

    [SerializeField] float yScaleMin;
    [SerializeField] float yScaleMax;

    [SerializeField] float zScaleMin;
    [SerializeField] float zScaleMax;


    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(Random.Range(xScaleMin, xScaleMax), Random.Range(yScaleMin, yScaleMax), Random.Range(zScaleMin, zScaleMax));
        Vector3 rot = new Vector3(Random.Range(xRotMin, xRotMax), Random.Range(yRotMin, yRotMax), Random.Range(zRotMin, zRotMax));
        transform.rotation = Quaternion.Euler(rot);
    }

}
