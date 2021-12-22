using System;
using System.Collections;
using System.Collections.Generic;
//using NUnit.Framework.Api;
using uWintab;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class IndicatorOnCanvasLogic : MonoBehaviour
{
    //Linked object
    public GameObject targetToFollow;
    public Vector3 tabletBounds;
    
    //linked object
    public float scale;
    public float sizeX;
    public float sizeZ;
    
    void Start()
    {
        targetToFollow = GameObject.Find("CanvasSeeker");
    }

    // Update is called once per frame
    void Update()
    {
        
        //gets the position of the indicator relative to the bound cube (0,0,0) = center x= left/right z=bottom/top y=distance [-0.5,0.5] <-inside
        tabletBounds = GameObject.Find("PenTipTracker").GetComponent<CoordsInsideBounds>().current;
        toggleVisibility(intersectCheck());
        transform.localPosition = new Vector3(0,0,0);
        //offset
        transform.localPosition = new Vector3(-tabletBounds.x*200,tabletBounds.z*200,0);


    }
    
    
    private void toggleVisibility(bool visible)
    {
        if (visible)
        {
            //GetComponent<MeshFilter>().mesh
            GetComponent<Renderer>().enabled = true;
            return;
        }
        GetComponent<Renderer>().enabled = false;
    }
    
    
    private bool  intersectCheck()
    {
        float x = tabletBounds.x;
        float y = tabletBounds.y;
        float z = tabletBounds.z;

        float alphaValue = (float) Math.Abs(y - 0.5);

        // all true = intersect
        if ((x >= -0.5 && x <= 0.5)&&(y >= -0.5 && y <= 0.5)&&(z >= -0.5 && z <= 0.5))
        {
            //Debug.Log("in the cube, Alphavalue: "+alphaValue);
            //Adjust Alpha
            changeAlpha(GetComponent<Renderer>().material, alphaValue);
            //todo Adjust size
            return true;
        }
        return false;
    }

    void changeAlpha(Material mat, float alphaVal)
    {
        Color oldColor = mat.color;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alphaVal);
        mat.SetColor("_Color", newColor);

    }





    /*
    
    
    // Start is called before the first frame update
    void Start()
    {
        Velocity = Vector3.zero;
        smoothTime = 0.01f;
        rotationSpeed = 1.5f;
    }

    // Update is called once per frame
    void Update()
    { 
        //sets the target
        targetToFollow = GameObject.Find("TabletInputHandler").GetComponent<InputHandler>().curCanvas;
        
        //todo get size for offset calculation
        //size = targetToFollow.GetComponent<Renderer>().bounds.size;
        
        //gets the position of the indicator relative to the bound cube (0,0,0) = center x= left/right z=bottom/top y=distance [-0.5,0.5] <-inside
        tabletBounds = GameObject.Find("PenTipTracker").GetComponent<CoordsInsideBounds>().current;

        getToCenterOfCanvas();
        toggleVisibility(intersectCheck());
    }

    private void toggleVisibility(bool visible)
    {
        if (visible)
        {
            //GetComponent<MeshFilter>().mesh
            GetComponent<Renderer>().enabled = true;
            return;
        }
        GetComponent<Renderer>().enabled = false;
    }

    private bool  intersectCheck()
    {
        float x = tabletBounds.x;
        float y = tabletBounds.y;
        float z = tabletBounds.z;

        float alphaValue = (float) Math.Abs(y - 0.5);

        // all true = intersect
        if ((x >= -0.5 && x <= 0.5)&&(y >= -0.5 && y <= 0.5)&&(z >= -0.5 && z <= 0.5))
        {
            //Debug.Log("in the cube, Alphavalue: "+alphaValue);
            //Adjust Alpha
            changeAlpha(GetComponent<Renderer>().material, alphaValue);
            //todo Adjust size
            return true;
        }
        return false;
    }

    void changeAlpha(Material mat, float alphaVal)
    {
        Color oldColor = mat.color;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alphaVal);
        mat.SetColor("_Color", newColor);

    }
    
    
    private void getToCenterOfCanvas()
    {
        
        targetToFollow = GameObject.Find("TabletInputHandler").GetComponent<InputHandler>().curCanvas; 
            
        //gets position and rotation of gameobject to follow
        targetPosition = new Vector3(targetToFollow.transform.position.x, targetToFollow.transform.position.y, targetToFollow.transform.position.z);
        targetRotation = new Vector4(targetToFollow.transform.rotation.x, targetToFollow.transform.rotation.y, targetToFollow.transform.rotation.z,targetToFollow.transform.rotation.w);
        
        //gets position and rotation of the object itself
        currentPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        currentRotation = new Vector4(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        
          
        //smooth the position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref Velocity, smoothTime );
        //smooth the rotation
        transform.rotation = Quaternion.Slerp(transform.rotation,targetToFollow.transform.rotation, rotationSpeed);
        
        
       
        
        
    }
    
    */
    
}
