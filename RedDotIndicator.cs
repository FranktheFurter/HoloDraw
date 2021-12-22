using System.Collections;
using System.Collections.Generic;
using uWintab;
using UnityEngine;

public class RedDotIndicator : MonoBehaviour
{
    //pen values
    public float penX;
    public float penY;
    public bool proximity;
    public bool canvasScaleRunning;
    public Vector3 sphere;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //get the pen info
        penX = GameObject.Find("Pen").GetComponent<Pen>().x;
        penY = GameObject.Find("Pen").GetComponent<Pen>().y;
        proximity = GameObject.Find("Pen").GetComponent<Pen>().proximity;
        
        //Input handler state
        canvasScaleRunning = GameObject.Find("TabletInputHandler").GetComponent<InputHandler>().canvasScalingRunning;
        
        //Position of "Drawhead" on the active canvas
        sphere = GameObject.Find("Pen").GetComponent<Pen>().sphereVector;
        
        //make indicator vissible if tip is near tabletsurface
        toggleVisibility(proximity);
        transform.position = sphere;
    }
    private void toggleVisibility(bool visible)
    {
        if (visible && !canvasScaleRunning)
        {
            //GetComponent<MeshFilter>().mesh
            GetComponent<Renderer>().enabled = true;
            return;
        }
        GetComponent<Renderer>().enabled = false;
    }
    
    
    
    
    
}
