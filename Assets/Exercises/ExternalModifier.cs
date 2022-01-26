using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalModifier : MonoBehaviour
{
    public GameObject CubeToModify;

    // Start is called before the first frame update
    void Start()
    {
        CubeToModify.GetComponent<Rotate3DObject>().RotationSpeed = 1000;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
