using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uWintab;

public class QuickAccessButton : MonoBehaviour
{

    bool touching = false;
    public bool onlyExecuteOncePls = true;
    public bool locationFreezeRelToCanvas;
    Material originalMaterial;
    

   
    // Start is called before the first frame update
    void Start()
    {
        originalMaterial = this.GetComponent<Renderer>().material;
        locationFreezeRelToCanvas = GameObject.Find("CanvasSeeker").GetComponent<CanvasSeeker>().locationFreezeRelToCanvas;
        //setContainer();
        toggleLeds();
    }

    // Update is called once per frame
    void Update()
    {



        if (touching && !GameObject.Find("TabletInputHandler").GetComponent<InputHandler>().canvasScalingRunning)
        {
            //HoloDraw Buttons
           
            
            //no buttons on static canvas
            if (GameObject.Find("TabletInputHandler").GetComponent<InputHandler>().holoToggle){
                
            //----------Freeze of Location and Rotation----------//
            if(name == "FreezeLocationButton")
            {
                if (onlyExecuteOncePls)
                {
                    onlyExecuteOncePls = false;
                    GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().locationFreezeRelToTablet = !GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().locationFreezeRelToTablet;
                    
                    //turn off all other functions
                    //2
                    GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().rotationFreeze = false;
                    //3
                    locationFreezeRelToCanvas = false;
                    GameObject.Find("CanvasSeeker").GetComponent<CanvasSeeker>().locationFreezeRelToCanvas = locationFreezeRelToCanvas;
                    GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().locationFreezeRelToCanvas = locationFreezeRelToCanvas;
                    setContainerFreeze();
                    
                    
                    toggleLeds();
                    
                }

            }
            if(name == "FreezeRotationButton")
            {
                if (onlyExecuteOncePls)
                {
                    onlyExecuteOncePls = false;
                    GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().rotationFreeze = !GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().rotationFreeze;
                   
                    
                    //turn off all other functions
                    //1
                    GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().locationFreezeRelToTablet = false;
                    //3
                    locationFreezeRelToCanvas = false;
                    GameObject.Find("CanvasSeeker").GetComponent<CanvasSeeker>().locationFreezeRelToCanvas = locationFreezeRelToCanvas;
                    GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().locationFreezeRelToCanvas = locationFreezeRelToCanvas;
                    setContainerFreeze();
                    
                    
                    toggleLeds();
                    
                }

            }
            
            if(name == "FreezePositionButton")
            {
                if (onlyExecuteOncePls)
                {
                    onlyExecuteOncePls = false;
                    //toggle the bool on and of
                    
                    
                   //toggle bool in smooth script
                    GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>()
                        .locationFreezeRelToCanvas = !GameObject.Find("MotionStabilizer")
                        .GetComponent<SmoothFollowPositionAndRotation>().locationFreezeRelToCanvas; 
                    //set state in canvasseeker object
                    GameObject.Find("CanvasSeeker").GetComponent<CanvasSeeker>().locationFreezeRelToCanvas = GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>()
                        .locationFreezeRelToCanvas;
                    
                    
                    //swap parents depending on state
                    setContainerFreeze();
                    
                    
                    //turn off all other functions
                    //1
                    GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().locationFreezeRelToTablet = false;
                    //2
                    GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().rotationFreeze = false;
                    
                    toggleLeds();
                    
                }

            }
            }
            //Toggle for canvas behaviour
            //deprecated, moved to inputhandler for hardware button
            //is now button for Quantization
            if(name == "HoloDrawButton")
            {
                if (onlyExecuteOncePls)
                { 
                    onlyExecuteOncePls = false;
                    //toggle in smoothscript
                    GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().quantization =
                        !GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>()
                            .quantization;
                    //update in inputhandler
                    GameObject.Find("TabletInputHandler").GetComponent<InputHandler>().quantization = GameObject
                        .Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().quantization;


                   // GameObject.Find("TabletInputHandler").GetComponent<InputHandler>().snappingDistAndSize();

                    toggleLeds();
                    
                }
            }
            
            //side buttons
            if(name == "side1")
            {
                if (Input.GetKeyDown(KeyCode.PageUp))
                {
                    GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>()
                        .cycleRotationClamp();
                   }

            }
            
            if(name == "side2")
            {
                if (Input.GetKeyDown(KeyCode.PageUp))
                {
                    GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>()
                        .cyclePositionClamp();
                }

            }
            
            if(name == "side3")
            {
                if (Input.GetKeyDown(KeyCode.PageUp))
                {
                    GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().culling =
                        !(GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().culling);
                    GameObject.Find("TabletInputHandler").GetComponent<InputHandler>().checkCulling();
                        
                }

            }
            
            
            
            
            //----------Rotation of Canvas----------//
            //Angle up
            if(name == "CanvasAngleUpButton")
            {
                if (Input.GetKeyDown(KeyCode.PageUp))
                {
                   GameObject.Find("RotationCube").GetComponent<CanvasAngle>().rotateUp();
                }
            }
            //Angle down
            if(name == "CanvasAngleDownButton")
            {
                if (Input.GetKeyDown(KeyCode.PageUp))
                {
                    GameObject.Find("RotationCube").GetComponent<CanvasAngle>().rotateDown();
                }

            }
            
            
            
            
            
            
            //VRSketchIn++ Buttons
            if(name == "QuickTranslationButton")
            {
                if (Input.GetKeyDown(KeyCode.PageUp))
                {
                    GameObject.Find("TabletInputHandler").GetComponent<InputHandler>().TranslationGizmo();
                }

            }

            if (name == "QuickRotationButton")
            {
                if (Input.GetKeyDown(KeyCode.PageUp))
                {
                    GameObject.Find("TabletInputHandler").GetComponent<InputHandler>().RotationGizmo();
                }

            }

            if (name == "QuickDeletionButton")
            {
                if (Input.GetKeyDown(KeyCode.PageUp))
                {
                    GameObject.Find("TabletInputHandler").GetComponent<InputHandler>().deleteSelectedObjects();
                }

            }

            if (name == "MenuButton")
            {
                if (Input.GetKeyDown(KeyCode.PageUp))
                {
                    GameObject.Find("TabletInputHandler").GetComponent<InputHandler>().setupUI();
                }

            }

        }

        



    }

    //botton switch moved to smoothposition
/*
    private void updateButtonText()
    {
        float Rc = GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().rotationClamp;
        float Pc = GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().positionClamp;
        
        
        //s1
        GameObject.Find("s1text").GetComponent<TextMesh>().text = ""+Rc;
        if (Rc == 1)
        {
            GameObject.Find("s1text").GetComponent<TextMesh>().text = "Off";
        }
        //s1
        GameObject.Find("s2text").GetComponent<TextMesh>().text = Pc.ToString();
        if (Pc == 0.01)
        {
            GameObject.Find("s2text").GetComponent<TextMesh>().text = "Off";
        }
        

    }
*/
    private void toggleLeds()
    {
        //led FRTT
        //led on
        if (GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>()
            .locationFreezeRelToTablet)
        {
            GameObject.Find("led1").GetComponent<Renderer>().enabled = true;
        }
        else
            //led off
        {
            GameObject.Find("led1").GetComponent<Renderer>().enabled = false;

        }
        
        //led FR
        //led on
        if (GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().rotationFreeze)
        {
            GameObject.Find("led3").GetComponent<Renderer>().enabled = true;
        }
        else
            //led off
        {
            GameObject.Find("led3").GetComponent<Renderer>().enabled = false;

        }
        
        //led LFRTC
        //led on
        if (GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>()
            .locationFreezeRelToCanvas)
        {
            GameObject.Find("led2").GetComponent<Renderer>().enabled = true;
        }
        else
            //led off
        {
            GameObject.Find("led2").GetComponent<Renderer>().enabled = false;

        }
        
        //led Q
        //led on
        if (GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().quantization)
        {
            GameObject.Find("led4").GetComponent<Renderer>().enabled = true;
        }
        else
            //led off
        {
            GameObject.Find("led4").GetComponent<Renderer>().enabled = false;

        }
        
        
        
        
        
        
        
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Pen")
        {
            GetComponent<Renderer>().material = (Material)Resources.Load("PreSelectionDoubleSided", typeof(Material));
            touching = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Pen")
        {
            GetComponent<Renderer>().material = originalMaterial;
            touching = false;
            Invoke("recoverHelp", 1);
           }
    }

    
    //moved to input handler
    /*
    public void setContainer()
    {
        if (holoToggle)
        {
            Debug.Log("Setting parent to RotationCube, holoToggle= "+holoToggle);
            if (locationFreezeRelToCanvas)
            {
                setContainerFreeze();
                GameObject.Find("TabletInputHandler").GetComponent<InputHandler>().holoToggle = holoToggle;
                return;
            }
            
            GameObject.Find("CanvasContainer").transform.SetParent(GameObject.Find("RotationCube").transform);
            GameObject.Find("TabletInputHandler").GetComponent<InputHandler>().holoToggle = holoToggle;
            return;
        }
        if (!holoToggle)
        {
            Debug.Log("Setting parent to StaticCanvas, holoToggle= "+holoToggle);
            GameObject.Find("CanvasContainer").transform.SetParent(GameObject.Find("StaticCanvas").transform);
            GameObject.Find("TabletInputHandler").GetComponent<InputHandler>().holoToggle = holoToggle;
            return;
        }
    }
    */
   
    public void setContainerFreeze()
    {
        if (GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>()
            .locationFreezeRelToCanvas)
        {
            Debug.Log("Setting parent to CanvasSeeker, locationFreeze= "+locationFreezeRelToCanvas);
            GameObject.Find("CanvasContainer").transform.SetParent(GameObject.Find("CanvasSeeker").transform);
        }
        if (!GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>()
            .locationFreezeRelToCanvas)
        {
            Debug.Log("Setting parent to Rotationcube, locationFreeze= "+locationFreezeRelToCanvas);
            GameObject.Find("CanvasContainer").transform.SetParent(GameObject.Find("RotationCube").transform);
        }
    }
   
    public void recoverHelp()
    { 
        onlyExecuteOncePls = true;
    }
}
