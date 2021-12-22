using System;
using System.Collections;
using System.Collections.Generic;
//using NUnit.Framework.Constraints;
using UnityEngine;
using uWintab;

public class QuantizationBuild : MonoBehaviour
{
    //Linked object
    public GameObject targetToFollow;
    //Target position values
    public Vector3 targetPosition;
    public Quaternion targetRotation;
    //own position values
    public Vector3 currentPosition;
    public Quaternion currentRotation;
    //Smooth Values
    public float smoothTime;
    public float rotationSpeed;
    
    public Vector3 Velocity;

    public Quaternion actualRotation;



    
    
    // Start is called before the first frame update
    void Start()
    { 
        Velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
       
        //rounding
        targetPosition = new Vector3(rtd(targetToFollow.transform.position.x), rtd(targetToFollow.transform.position.y), rtd(targetToFollow.transform.position.z));
        targetRotation = new Quaternion(targetToFollow.transform.rotation.x, targetToFollow.transform.rotation.y, targetToFollow.transform.rotation.z, targetToFollow.transform.rotation.w);
        
        //gets position and rotation of the object itself
        currentPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        currentRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);


        targetRotation = quantizeRotation(targetRotation);
        
        
        
        //smooth the position
        transform.position = Vector3.SmoothDamp(currentPosition, targetPosition, ref Velocity, smoothTime );
        //smooth the rotation
        transform.rotation = Quaternion.Slerp(currentRotation,targetRotation, rotationSpeed);
        
    }
    private Quaternion quantizeRotation(Quaternion targetRotation)
    {

        float rotationClamp = 45f;
        Vector3 converted = targetRotation.eulerAngles;
        Vector3 clampedRotation = new Vector3();
        clampedRotation = new Vector3(clampAndOffset(converted.x,rotationClamp),clampAndOffset(converted.y,rotationClamp),clampAndOffset(converted.z,rotationClamp));

        Debug.Log(converted);
        Debug.Log(clampedRotation);


        targetRotation = Quaternion.Euler(clampedRotation);

        return targetRotation;
    }
    private float clampAndOffset(float rot, float rotationClamp)
    {
        float offset = rotationClamp / 2;
        //rot += offset;
        rot = (float) Math.Round(rot / rotationClamp);
        
        
        // Debug.Log(rot + "nach round");
        rot = rot * rotationClamp;

        return rot;


    }

    
    
    
    
    
    
    
    
    private Quaternion help1(Quaternion targetRotation)
    {

        float rotationClamp = 30f;
        Vector3 converted = targetRotation.eulerAngles;
        Vector3 clampedRotation = new Vector3();
        clampedRotation = new Vector3(converted.x-(converted.x % rotationClamp),converted.y-(converted.y % rotationClamp), converted.z-(converted.z % rotationClamp));
        
        
        Debug.Log(converted);
        Debug.Log(clampedRotation);



        return targetRotation;
    }
    

    


    private float rtd(float f)
    {
        f = Mathf.Round(f * 1.0f) * 1.0f;
        return f;
    }
    
   /* 
    public Quaternion wtf()
    {
        float rotationClamp = 15f;
        
        //Todo check if using transform.rotation or targettofollow.transform.rotation is better
        actualRotation = targetRotation;
        //Set our current rotation to actualRotation to restore any lost values (less than rotationClamp)
        transform.rotation = actualRotation;

        //Get a rotation on the X and Z axis between -1 and 1
        var rotationInput = new Vector3(Input.GetAxis("Vertical"), 0f, Input.GetAxis("Horizontal"));

        //Rotate the transform based on the rotationInput
        transform.Rotate(rotationInput);

        //Back up the rotation BEFORE we clamp it.
        actualRotation = transform.rotation;

        //Pull the current euler angle of our rotation
        var currentEulerRotation = transform.rotation.eulerAngles;

        //Clamp it by taking the current euler rotation and subtracting any remainder when we divide each angle by rotationClamp
        return Quaternion.Euler(currentEulerRotation - new Vector3(currentEulerRotation.x % rotationClamp, currentEulerRotation.y % rotationClamp, currentEulerRotation.z % rotationClamp));
        
        
        
    }
    */
}
