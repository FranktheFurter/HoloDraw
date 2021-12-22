using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasAngle : MonoBehaviour
{
    public float turnDegrees = 15f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
        
        //rotate vias keyboard for debug
        if (Input.GetKeyDown(KeyCode.W))
        {
            rotateDown();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            rotateUp();
        }
    }
    //rotation Methods to be called by other objects
    public void rotateDown()
    {
        transform.Rotate(Vector3.left, turnDegrees);

    }
    public void rotateUp()
    {
        transform.Rotate(Vector3.right, turnDegrees);

    }
    
    
}