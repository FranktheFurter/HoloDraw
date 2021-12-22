using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordsInsideBounds : MonoBehaviour
{
   
    public Vector3 current;
    public Vector3 size;
    
    
    //linked object
    public GameObject bounds;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        size = bounds.GetComponent<Renderer>().bounds.size;
       
    }

    // Update is called once per frame
    void Update()
    {
        Transform parentObj = bounds.transform;
        Transform nestedChild = transform;
        
        current = parentObj.InverseTransformPoint( nestedChild.position );
       


    }
}
