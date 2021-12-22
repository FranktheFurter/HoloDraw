using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using uWintab;

//using NUnit.Framework.Constraints;

public class SmoothFollowPositionAndRotation : MonoBehaviour
{
    //Linked object
    public GameObject targetToFollow;
    public GameObject inputHandler;


    //Gameobjects for visibility
    public GameObject canvasSeeker;
    public GameObject smoothCube;
    public GameObject rotCube;


    //Bools for location/rotation freeze and quantization
    public bool locationFreezeRelToTablet;
    public bool locationFreezeRelToCanvas;
    public bool rotationFreeze;
    
    //culling bool
    public bool culling;

    //Quantization
    public bool quantization;
    public float positionClamp;
    public float rotationClamp;
    public int AvgSampleSize;
    public int avgCounter;


    public List<Vector3> avgPos = new List<Vector3>();
    public List<Quaternion> avgRot = new List<Quaternion>();


    //Target position values
    public Vector3 targetPosition;
    public Quaternion targetRotation;

    //own position values
    public Vector3 currentPosition;
    public Quaternion currentRotation;


    //Smooth Values
    public float smoothTime;
    public float rotationSpeed;

    public bool scalecheck;
    public float tempSmoothTime;
    public float tempRotationSpeed;


    public Vector3 Velocity;

    // Start is called before the first frame update
    private void Start()
    {
        Velocity = Vector3.zero;
        locationFreezeRelToTablet = false;
        locationFreezeRelToCanvas = false;
        rotationFreeze = false;

        positionClamp = 4f;
        rotationClamp = 4f;

        //AvgSampleSize = 10;
        avgCounter = 0;

        for (var i = 0; i < AvgSampleSize; i++)
        {
            avgPos.Insert(i, new Vector3(0, 0, 0));
            avgRot.Insert(i, new Quaternion(0, 0, 0, 0));
        }
    }

    // Update is called once per frame
    private void Update()
    {
        checkForQuantAndScale();
        
        toggleVisibility();
        cullingStatus();

        //gets position and rotation of gameobject to follow
        //quantized
        if (quantization)
        {
            avgPos.RemoveAt(avgCounter);
            avgPos.Insert(avgCounter, targetToFollow.transform.position);

            avgRot.RemoveAt(avgCounter);
            avgRot.Insert(avgCounter, targetToFollow.transform.rotation);


            //list values for avg calc
            var newVector = new Vector3(
                avgPos.Average(x => x.x),
                avgPos.Average(x => x.y),
                avgPos.Average(x => x.z));


            var newQanternion = new Quaternion(
                avgRot.Average(x => x.x),
                avgRot.Average(x => x.y),
                avgRot.Average(x => x.z),
                avgRot.Average(x => x.w));







            //offcondition
            if (positionClamp != 0.01f)
            {
                targetPosition = new Vector3(quantizePosition(newVector.x), quantizePosition(newVector.y),
                    quantizePosition(newVector.z)); 
            }
            else
            {
                targetPosition = new Vector3(targetToFollow.transform.position.x, targetToFollow.transform.position.y,
                    targetToFollow.transform.position.z);
            }



            //offcondition
            if (rotationClamp != 0.01f)
            {
                targetRotation = quantizeRotation(newQanternion);
            }
            else
            {
                targetRotation = new Quaternion(targetToFollow.transform.rotation.x, targetToFollow.transform.rotation.y,
                    targetToFollow.transform.rotation.z, targetToFollow.transform.rotation.w);
            }
           


            //resetCondition
            avgCounter++;
            if (avgCounter >= AvgSampleSize) avgCounter = 0;
        }
        //normal
        else
        {
            targetPosition = new Vector3(targetToFollow.transform.position.x, targetToFollow.transform.position.y,
                targetToFollow.transform.position.z);

            targetRotation = new Quaternion(targetToFollow.transform.rotation.x, targetToFollow.transform.rotation.y,
                targetToFollow.transform.rotation.z, targetToFollow.transform.rotation.w);
        }

        //gets position and rotation of the object itself
        currentPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        currentRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z,
            transform.rotation.w);

        //smooth the position
        if (!locationFreezeRelToTablet && !locationFreezeRelToCanvas)
            transform.position = Vector3.SmoothDamp(currentPosition, targetPosition, ref Velocity, smoothTime);
        //smooth the rotation
        if (!rotationFreeze)
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed);
    }

    private void cullingStatus()
    {
        
    }


    private void toggleVisibility()
    {
        //all off
        rotCube.GetComponent<Renderer>().enabled = false;
        smoothCube.GetComponent<Renderer>().enabled = false;
        canvasSeeker.GetComponent<Renderer>().enabled = false;

        //toggle depending on state
        if (locationFreezeRelToCanvas || scalecheck) canvasSeeker.GetComponent<Renderer>().enabled = true;
        if (locationFreezeRelToTablet || scalecheck || quantization)
        {
            rotCube.GetComponent<Renderer>().enabled = true;
            smoothCube.GetComponent<Renderer>().enabled = true;
        }

        //case static canvas all off
        if (!GameObject.Find("TabletInputHandler").GetComponent<InputHandler>().holoToggle)
        {
            rotCube.GetComponent<Renderer>().enabled = false;
            smoothCube.GetComponent<Renderer>().enabled = false;
            canvasSeeker.GetComponent<Renderer>().enabled = false;
        }
    }


    public  void instantPositionSnap()
    {
        transform.position = targetPosition;
        
    }

    public void instantRotationSnap()
    {
        transform.rotation = targetRotation;
    }
    
    
    
    
/*
    private Quaternion quantizeRotation(Quaternion targetRotation)
    {
        var rc = rotationClamp;
        var converted = targetRotation.eulerAngles;
        var clampedRotation = new Vector3();
        clampedRotation = new Vector3(clampAndOffset(converted.x, rc), clampAndOffset(converted.y, rc),
            clampAndOffset(converted.z, rc));

        //Debug.Log(converted);
        //Debug.Log(clampedRotation);


        targetRotation = Quaternion.Euler(clampedRotation);

        return targetRotation;
    }
    */
    
   /*
    private float clampAndOffset(float rot, float rc)
    {
        var offset = rc / 2;
        //rot += offset;
        rot = (float) Math.Round(rot / rc);


        // Debug.Log(rot + "nach round");
        rot = rot * rc;

        return rot;
    }

*/

    private Quaternion quantizeRotation(Quaternion targetRotation)
    {
        targetRotation = new Quaternion(quantizeQuaternion(targetRotation.x), quantizeQuaternion(targetRotation.y),
            quantizeQuaternion(targetRotation.z), quantizeQuaternion(targetRotation.w));

        return targetRotation;
    }

    private float quantizeQuaternion(float f)
    {
        f = Mathf.Round(f * rotationClamp) / rotationClamp;
        // f = (float)Math.Round(f, MidpointRounding.AwayFromZero) / positionClamp;
        //Debug.Log("F: "+f);
        return f;
    }

    private float quantizePosition(float f)
    {
        f = Mathf.Round(f * positionClamp) / positionClamp;
        // f = (float)Math.Round(f, MidpointRounding.AwayFromZero) / positionClamp;
        //Debug.Log("F: "+f);
        return f;
    }

    private float rtd2(float f)
    {
        f = Mathf.Round(f * 10.0f) * 0.1f;
        return f;
    }


    private void checkForQuantAndScale()
    {
        if (quantization || scalecheck || !GameObject.Find("TabletInputHandler").GetComponent<InputHandler>().holoToggle)
        {
            rotationSpeed = tempRotationSpeed;
            smoothTime = tempSmoothTime;
        }
        else
        {
            rotationSpeed = 0.03f;
            smoothTime = 0.3f;
        }
    }


    //off (1)
    //30
    //45
    //90
    public void cycleRotationClamp()
    {
        //offcondition
        if (rotationClamp == 0.01f)
        {
            rotationClamp = 6;
            GameObject.Find("s1text").GetComponent<TextMesh>().text = "15°";
            return;
        }
        if (rotationClamp == 6)
        {
            rotationClamp = 4;
            GameObject.Find("s1text").GetComponent<TextMesh>().text = "30°";
            return;
        }
        if (rotationClamp == 4)
        {
            rotationClamp = 2;
            GameObject.Find("s1text").GetComponent<TextMesh>().text = "45°";
            return;
        }
        if (rotationClamp == 2)
        {
            rotationClamp = 0.01f;
            GameObject.Find("s1text").GetComponent<TextMesh>().text = "Off";
            return;
        }


    }

    //off (0.01)
    //0.1 =  10
    //0.25 = 4
    //0.5 = 2
    //1 = 1
    public void cyclePositionClamp()
    {
        //offcondition
        if (positionClamp == 0.01f)
        {
            positionClamp = 10;
            GameObject.Find("s2text").GetComponent<TextMesh>().text = "0.01";
            return;
        }
        if (positionClamp == 10)
        {
            positionClamp = 4;
            GameObject.Find("s2text").GetComponent<TextMesh>().text = "0.25";
            return;
        }
        if (positionClamp == 4)
        {
            positionClamp = 2;
            GameObject.Find("s2text").GetComponent<TextMesh>().text = "0.5";
            return;
        }
        if (positionClamp == 2)
        {
            positionClamp = 1;
            GameObject.Find("s2text").GetComponent<TextMesh>().text = "1.0";
            return;
        }
        if (positionClamp == 1)
        {
            positionClamp = 0.01f;
            GameObject.Find("s2text").GetComponent<TextMesh>().text = "Off";
            return;
        }
    }
}