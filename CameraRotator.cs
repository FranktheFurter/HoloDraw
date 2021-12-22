using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{

    public float rotSpeed = 6.0f;
    public float translationSpeed = 0.1f;

    public bool rotating = true;
    public bool action = false;
    
    void Update()
    {


        if (action)
        {

            if (rotating)
            {
                transform.Rotate(0, -rotSpeed * Time.deltaTime, 0);
            }

            else
            {
                transform.position += new Vector3(0, 0, translationSpeed * Time.deltaTime);
            }

        }
  
     
    }
}
