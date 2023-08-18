using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenFang : MonoBehaviour
{
    [SerializeField] public GameObject[][] hiddenFangs = new GameObject[4][];
    [SerializeField] GameObject[] module0;
    [SerializeField] GameObject[] module1;
    [SerializeField] GameObject[] module2;
    [SerializeField] GameObject[] module3;

    // Start is called before the first frame update
    void Start()
    {
        hiddenFangs[0] = module0;
        hiddenFangs[1] = module1;
        hiddenFangs[2] = module2;
        hiddenFangs[3] = module3;
    }
}
