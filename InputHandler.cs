using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

namespace uWintab
{

    public class InputHandler : MonoBehaviour
    {

        Tablet tablet_;
        TabletInformation tabletInformation;
        Pen pen;
        GameObject inputField;
        ButtonFactory buttonFactory;
        List<CustomButton> customButtons;
        public bool[] buttons;
        bool[] penButtons;
        public GameObject curCanvas, curCamera;
        public GameObject curPortal;
        Transform trackerTransform;
        public GameObject canvasMaster;
        public GameObject tabletCanvas;
        public SelectedObjects data;
        public GameObject helperGrid;


        private float keyTimer;
        private float keyTimerOld = 0f;
        public bool selectkeyPressed;
        bool rayToggeled = false;
        public bool selectionMode = false;

        //Not implemented at the moment
        private bool leftHandMode = false;

        public bool uiActive = false;


        //HoloDraw
        public bool holoToggle = true;
        public bool quantization = false;
        public float savedScale;
        public float savedDistance;
        public bool canvasScalingRunning = false;
        public GameObject rCube;
        
        public GameObject cSeeker;

        
        
        //private List<GameObject> selectedGameObjects;
        //private List<GameObject> selectedGameObjects;
        //private List<Material> selectedGameObjectMaterials;

        /// <summary>
        /// ButtonReferences
        /// </summary>

        public List<GameObject> UIbuttonRef = new List<GameObject>();

        private Dictionary<GameObject, GameObject> cameraDict;
        private Dictionary<GameObject, GameObject> portalCameraDict;
        private GameObject gizmoCamera;


        public bool draggingGizmo = false;

        void Start()
        {
            savedDistance = 0.25f;
            savedScale = 0.25f;
            //setContainer();

           
        

            //leftHandMode = true;

            helperGrid.SetActive(false);

            gizmoCamera = GameObject.Find("GizmoCamera");

            data.currentSelectedObject = null;
            data.selectedGameObjects = new List<GameObject>();
            data.selectedGameObjectMaterials = new List<Material>();

            cameraDict = new Dictionary<GameObject, GameObject>();
            portalCameraDict = new Dictionary<GameObject, GameObject>();
            tablet_ = FindObjectOfType<Tablet>();
            tabletInformation = FindObjectOfType<TabletInformation>();
            pen = FindObjectOfType<Pen>();
            inputField = GameObject.Find("Input");
            //Dictionary<int, float[]> buttonInformation = new Dictionary<int, float[]>();
            //buttonInformation.Add(0, new float[] { 0f, 0.7f, 0.3f, 0.3f });
            //buttonInformation.Add(1, new float[] { 0f, 0.5f, 0.3f, 0.2f });
            //buttonFactory = new ButtonFactory(inputField, inputField.GetComponent<Renderer>().bounds.size, 2, buttonInformation);
            //customButtons = buttonFactory.buttons;

            if (!tablet_) return;
            // buttons = new bool[tablet_.expKeyNum];
            buttons = new bool[4];
            penButtons = new bool[2];

            curCanvas = pen.InputCanvasParent.gameObject;
            curCamera = GameObject.Find("CanvasCamera");

            cameraDict.Add(curCanvas, curCamera);
            trackerTransform = GameObject.Find("TabletTracker").transform;

            //ugly, but does the trick
            StartCoroutine("Delay");
            holoToggle = true;
        }

        IEnumerator Delay()
        {
            yield return new WaitForSeconds(0.1f);
            canvasMaster.SetActive(false);
        }

        void Update()
        {

           
            changeColorHoloToggle();


            if (tablet_.pressure > 0)
            {

                if (GameObject.Find("StrokeSize") != null)
                    GameObject.Find("StrokeSize").GetComponent<Renderer>().enabled = false;
            }
            else
            {
                if (GameObject.Find("StrokeSize") != null)
                {
                    GameObject.Find("StrokeSize").GetComponent<Renderer>().enabled = true;
                    setGameObjectStrokeSize();
                }
            }


            gizmoCamera.transform.position = pen.transform.position - (pen.transform.up * 0.1f);
            gizmoCamera.transform.rotation = pen.transform.rotation;
            gizmoCamera.transform.Rotate(new Vector3(-90, 0, 0));

            if (!tablet_) return;
            /*if(tablet_.proximity == true)
            {
                pen.gameObject.SetActive(true);
            } else
            {
                pen.gameObject.SetActive(false);
            }*/
            InputInfo info = ConstructInputInfo();
            tabletInformation.DisplayInformation(info);
            HandleButtonInput(info);
            HandlePenInput(info);

            HandlePenButtonInput();
        }

        private void changeColorHoloToggle()
        {
            if (holoToggle)
            {
                GameObject.Find("IconHoloToggle").GetComponent<SpriteRenderer>().color = Color.green;
            }
            else
            {
                GameObject.Find("IconHoloToggle").GetComponent<SpriteRenderer>().color = Color.red;
            }
        }

        private void HandlePenButtonInput()
        {
            //Draw in space
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                pen.drawInSpace = true;
            }

            if (Input.GetKeyUp(KeyCode.PageDown))
            {
                pen.drawInSpace = false;
            }

            //for immeadiate deletion on button press
            //Destroy(pen.currentSelectedObject);

            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                var temp = data.currentSelectedObject;
                select(temp);
            }

            bool doubleClick = false;
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                keyTimer = Time.time;
                selectkeyPressed = true;
                rayToggeled = false;

                doubleClick = ((keyTimer - keyTimerOld) < 0.3) ? true : false;

                if ((doubleClick && !rayToggeled))
                {
                    /*
                    if (!pen.Cube.activeInHierarchy)
                    {
                        //pen.Cube.SetActive(true);
                    }
                    else
                    {
                        //pen.Cube.SetActive(false);
                    }*/
                    selectionMode = !selectionMode;

                    if (selectionMode)
                    {
                        pen.GetComponent<Renderer>().material = (Material)Resources.Load("HighlightOld", typeof(Material));
                        //pen.canDraw = false;
                        /*
                        if (GameObject.Find("DiminitureSpace").GetComponent<DiminitureHandler>().minaturSpaceVisuals.activeInHierarchy)
                        {
                            GameObject.Find("DiminitureSpace").GetComponent<DiminitureHandler>().minaturSpaceVisuals.GetComponent<Collider>().enabled = false;
                        }
                        */
                    }
                    else
                    {
                        pen.GetComponent<Renderer>().material = (Material)Resources.Load("Black", typeof(Material));
                        //pen.canDraw = true;
                        /*
                        if (GameObject.Find("DiminitureSpace").GetComponent<DiminitureHandler>().minaturSpaceVisuals.activeInHierarchy)
                        {
                            GameObject.Find("DiminitureSpace").GetComponent<DiminitureHandler>().minaturSpaceVisuals.GetComponent<Collider>().enabled = true;
                        }
                        */
                    }

                    //to prevent re-toggling multiple times
                    rayToggeled = true;
                }

                keyTimerOld = keyTimer;
            }

            if (Input.GetKeyUp(KeyCode.PageUp))
            {
                selectkeyPressed = false;
            }

            //thickness of stroke ++
            if (Input.GetKeyDown(KeyCode.Period))
            {
                BrushManager.cursorsize += 0.01f;
                setGameObjectStrokeSize();
            }

            //thickness of stroke --
            if (Input.GetKeyDown(KeyCode.F5))
            {
                if (BrushManager.cursorsize > 0.01)
                {
                    BrushManager.cursorsize -= 0.01f;
                }
                setGameObjectStrokeSize();
            }
        }

        private void setGameObjectStrokeSize()
        {
            if (GameObject.Find("StrokeSize") != null)
                GameObject.Find("StrokeSize").transform.localScale = Vector3.one * BrushManager.cursorsize / 15;
        }



        private void select(GameObject temp)
        {
            if (temp != null && !data.selectedGameObjects.Contains(temp))
            {

                temp.GetComponent<Highlighter>().isSelected = true;

                temp.GetComponent<Renderer>().material = temp.GetComponent<Highlighter>().selectionMaterial;


                //also select clone
                if (temp.GetComponent<FlagScript>() != null)
                {

                    if (temp.GetComponent<FlagScript>().clone != null)
                    {
                        data.selectedGameObjects.Add(temp.GetComponent<FlagScript>().clone);
                        temp.GetComponent<FlagScript>().clone.GetComponent<Renderer>().material = temp.GetComponent<Highlighter>().selectionMaterial;
                        temp.GetComponent<FlagScript>().clone.GetComponent<Highlighter>().isSelected = true;
                    }
                }


                //add after clone -- keep it last selected item
                data.selectedGameObjects.Add(temp);

                //Activate translation gizmo if no gizmo is already activated and not drawing on canvas
                GizmoController controller = temp.GetComponent<GizmoController>();
                if (controller != null 
                    && !controller.position
                    && !controller.rotation
                    && !controller.scaling
                    && !tablet_.proximity)
                {
                    TranslationGizmo();
                }

            }

            //Deselect Objects if already selected
            else if (temp != null && data.selectedGameObjects.Contains(temp))
            {

                temp.GetComponent<Highlighter>().isSelected = false;
                temp.GetComponent<MeshRenderer>().material = temp.GetComponent<Highlighter>().material;
                data.selectedGameObjects.Remove(temp);

                if (temp.GetComponent<FlagScript>() != null)
                {
                    if (temp.GetComponent<FlagScript>().clone != null)
                    {
                        data.selectedGameObjects.Remove(temp.GetComponent<FlagScript>().clone);
                        temp.GetComponent<FlagScript>().clone.GetComponent<Highlighter>().isSelected = false;
                        temp.GetComponent<FlagScript>().clone.GetComponent<MeshRenderer>().material = temp.GetComponent<FlagScript>().clone.GetComponent<Highlighter>().material;
                    }
                }
            }
        }
        private InputInfo ConstructInputInfo()
        {
            for (int i = 0; i < buttons.Length; ++i)
            {
                buttons[i] = tablet_.GetExpKey(i);
            }
            for (int i = 1; i < penButtons.Length + 1; ++i)
            {
                penButtons[i - 1] = tablet_.GetButton(i);
            }
            return new InputInfo(tablet_.isAvailable, tablet_.deviceName, tablet_.version, tablet_.isPressureSupported, tablet_.isWheelSupported, tablet_.isOrientationSupported, tablet_.isExpKeysSupported, tablet_.deviceNum, tablet_.expKeyNum, tablet_.x, tablet_.y, tablet_.pressure, tablet_.wheel, tablet_.azimuth, tablet_.altitude, tablet_.twist, tablet_.penId, tablet_.cursor, tablet_.time, tablet_.proximity, buttons, penButtons);
        }

        bool heldDown;
        private void HandleButtonInput(InputInfo info)
        {

            for (int i = 0; i < info.expKeyNum; ++i)
            {

                if (info.buttons[i] == true)
                {
                    if (i == 0)
                    {
                        if (!heldDown) {
                            holoToggle = !holoToggle;
                            setContainer(false);
                        }
                        heldDown = true;

                        /*
                        //setup miniature space
                        //UL
                        if (!buttonFunctionRunning[i])
                        {
                            StartCoroutine(ButtonFunction(i));
                        }
                        */
                    }
                                    
                    if (i == 1)
                    {
                        //create new plane
                        //LL
                        /*
                        if (leftHandMode)
                        {
                            if (!buttonFunctionRunning[i])
                            {
                                StartCoroutine(ButtonFunction(i));
                            }
                        }
                        else
                        {*/

                        if (isCanvasSelected())
                        {
                            if (!buttonFunctionRunning[i])
                            {
                                StartCoroutine(ButtonFunction(i));
                            }
                        }
                        else
                        {
                            if (!canvasScalingRunning && !selectionMode && holoToggle)
                            {

                                StartCoroutine("UpdateCanvasScalingHoloDraw");


                                /*
                                //toggle entrypoint for modified behaviour
                                if (holoToggle)
                                {
                                    StartCoroutine("UpdateCanvasScalingHoloDraw");

                                }
                                else
                                {
                                    StartCoroutine("UpdateCanvasScaling");

                                }
                                */
                            }
                        }
                        //}
                    }
                   
                }
                if (info.buttons[i] == false)
                {
                    if (i == 0)
                    {
                        heldDown = false;
                    }
                }
            }

        }

        
        //----------------------HoloDraw-----------------------
        //Legacy
        private IEnumerator UpdateCanvasScaling()
        {
            Debug.Log("Update canvas scaling running holodoggle= "+holoToggle);
            float canvasScalingFactor = 0.25f, tmpScaling = 0.25f, canvasDistance = 0.25f, tmpDistance = 0.25f;
            bool touching, firstTouch = true;
            Vector2 firstTouchPos = new Vector2(-1, -1);
            if (curCamera != null)
            {
                curCamera.SetActive(false);
            }

            if (curCanvas != null)
            {
                curCanvas.GetComponentInChildren<Renderer>().material = (Material)Resources.Load("InactivePlane", typeof(Material));
                curCanvas.GetComponentInChildren<Highlighter>().material = (Material)Resources.Load("InactivePlane", typeof(Material));

            }

            canvasScalingRunning = true;
            curCanvas = Instantiate(Resources.Load("InputCanvasParent", typeof(GameObject))) as GameObject;
            curCamera = curCanvas.GetComponentInChildren<Camera>().gameObject;
            curCanvas.transform.SetParent(GameObject.Find("CanvasContainer").transform);

            //curCanvas.transform.position = new Vector3(trackerTransform.position.x + 0.1425f, trackerTransform.position.y, trackerTransform.position.z);

            //curCanvas.transform.rotation = trackerTransform.rotation;

            //curCanvas.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

            while (true)
            {

                if (leftHandMode)
                {
                    if (tablet_.proximity == false && buttons[3] == false)
                    {
                        break;
                    }
                }
                else
                {
                    if (tablet_.proximity == false && buttons[1] == false)
                    {
                        break;
                    }
                }


                touching = tablet_.pressure > 0 ? true : false;
                if (touching)
                {
                    if (firstTouch)
                    {

                        firstTouch = false;
                        firstTouchPos = new Vector2(tablet_.x, tablet_.y);
                    }
                    if (firstTouchPos.x != -1)
                    {
                        canvasScalingFactor = Mathf.Clamp(tmpScaling + tablet_.x - firstTouchPos.x, 0.1f, int.MaxValue);
                        canvasDistance = Mathf.Clamp(tmpDistance + (tablet_.y - firstTouchPos.y) * 2, 0.1f, int.MaxValue);
                        //canvasScalingFactor = map(tablet_.x, 0f, 1f, Mathf.Clamp(tmpScaling - 1, 0.1f, int.MaxValue), tmpScaling + 1);
                        //canvasDistance = map(tablet_.y, 0f, 1f, Mathf.Clamp(tmpDistance - 1, 0.1f, int.MaxValue), tmpDistance + 1);
                    }
                }
                else
                {
                    if (!firstTouch)
                    {
                        firstTouch = true;
                        firstTouchPos = new Vector2(-1, -1);
                    }
                    tmpScaling = canvasScalingFactor;
                    tmpDistance = canvasDistance;
                }


                if (curCanvas != null)
                {

                    //curCanvas.transform.position = new Vector3(trackerTransform.position.x + 0.1425f, trackerTransform.position.y, trackerTransform.position.z);
                    curCanvas.transform.position = new Vector3(tablet_.transform.position.x, tablet_.transform.position.y, tablet_.transform.position.z);
                    //Debug.Log(tablet_.transform.rotation.eulerAngles);

                    //Debug.Log((int)tablet_.transform.rotation.eulerAngles.x);

                    //5° Plane snapping
                    if ((int)tablet_.transform.rotation.eulerAngles.x % 5 == 0 || (int)tablet_.transform.rotation.eulerAngles.y % 5 == 0
                       || (int)tablet_.transform.rotation.eulerAngles.z % 5 == 0)
                    {
                        curCanvas.transform.rotation = tablet_.transform.rotation;
                        //rotate 180°, because tablet tracker does not map the canvas rotation and therefore lines appear to be mirrored because the plane is in
                        //a wrong rotation
                        curCanvas.transform.Rotate(new Vector3(0, 180, 180));

                    }

                    curCanvas.transform.localScale = new Vector3(canvasScalingFactor, canvasScalingFactor, canvasScalingFactor);
                    curCanvas.transform.Translate(-trackerTransform.up * canvasDistance, Space.World);

                    pen.RescaleCanvas(curCanvas, curCamera.GetComponent<Camera>(), canvasScalingFactor);
                }



                yield return null;
            }


            if (curCamera != null)
            {
                cameraDict.Add(curCanvas, curCamera);
                canvasScalingRunning = false;
            }

            //For debug reasons spawning first canvas
            //holoToggle = true;
            savedDistance = canvasDistance;
            savedScale = canvasScalingFactor;
        }
        //HoloDraw
        private IEnumerator UpdateCanvasScalingHoloDraw()
        {

            GameObject.Find("MotionStabilizer")
                .GetComponent<SmoothFollowPositionAndRotation>().scalecheck = true;
            
            Debug.Log("HoloToggle scaling running holodoggle= "+holoToggle);
            if (curCamera != null)
            {
              //  curCamera.SetActive(false);
                Destroy(curCamera);
            }

            if (curCanvas != null)
            {
               // curCanvas.GetComponentInChildren<Renderer>().material = (Material)Resources.Load("InactivePlane", typeof(Material));
               // curCanvas.GetComponentInChildren<Highlighter>().material = (Material)Resources.Load("InactivePlane", typeof(Material));
                Destroy(curCanvas);

            }
            
            

            float tmpScaling = 0.25f, tmpDistance = 0.25f;
            //Hope
            float canvasDistance= savedDistance;
            float canvasScalingFactor = savedScale;
            bool touching, firstTouch = true;
            Vector2 firstTouchPos = new Vector2(-1, -1);
            
            

            canvasScalingRunning = true;
            curCanvas = Instantiate(Resources.Load("InputCanvasParent", typeof(GameObject))) as GameObject;
            curCamera = curCanvas.GetComponentInChildren<Camera>().gameObject;
            
            
            curCanvas.transform.SetParent(GameObject.Find("CanvasContainer").transform);
            
            //GameObject.Find("CanvasContainer").transform.SetParent(GameObject.Find("RotationCube").transform);

            //curCanvas.transform.position = new Vector3(trackerTransform.position.x + 0.1425f, trackerTransform.position.y, trackerTransform.position.z);

            //curCanvas.transform.rotation = trackerTransform.rotation;

            //curCanvas.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

            while (true)
            {

                if (leftHandMode)
                {
                    if (tablet_.proximity == false && buttons[3] == false)
                    {
                        break;
                    }
                }
                else
                {
                    if (tablet_.proximity == false && buttons[1] == false)
                   
                    {
                        break;
                    }
                }


                touching = tablet_.pressure > 0 ? true : false;
                if (touching)
                {
                    if (firstTouch)
                    {

                        firstTouch = false;
                        firstTouchPos = new Vector2(tablet_.x, tablet_.y);
                    }
                    if (firstTouchPos.x != -1)
                    {
                        //old
                        //canvasScalingFactor = Mathf.Clamp(tmpScaling + tablet_.x - firstTouchPos.x, 0.1f, int.MaxValue);
                        //canvasDistance = Mathf.Clamp(tmpDistance + (tablet_.y - firstTouchPos.y) * 2, 0.1f, int.MaxValue);
                        
                        //trying to disable tmpscaling and distance
                        
                        canvasScalingFactor = Mathf.Clamp(tmpScaling + tablet_.x - firstTouchPos.x, 0.1f, int.MaxValue);
                        canvasDistance = Mathf.Clamp(tmpDistance + (tablet_.y - firstTouchPos.y) * 2, 0.0f, int.MaxValue);

                        if (quantization)
                        {
                            canvasScalingFactor = rtd2(canvasScalingFactor);
                            canvasDistance = rtd2(canvasDistance);
                        }
                        
                        
                        //canvasScalingFactor = map(tablet_.x, 0f, 1f, Mathf.Clamp(tmpScaling - 1, 0.1f, int.MaxValue), tmpScaling + 1);
                        //canvasDistance = map(tablet_.y, 0f, 1f, Mathf.Clamp(tmpDistance - 1, 0.1f, int.MaxValue), tmpDistance + 1);
                    }
                }
                else
                {
                    if (!firstTouch)
                    {
                        firstTouch = true;
                        firstTouchPos = new Vector2(-1, -1);
                    }
                    tmpScaling = canvasScalingFactor;
                    tmpDistance = canvasDistance;
                }


                if (curCanvas != null)
                {

                    //scale from cSeeker
                    if (GameObject.Find("MotionStabilizer")
                        .GetComponent<SmoothFollowPositionAndRotation>()
                        .locationFreezeRelToCanvas)
                    {
                        curCanvas.transform.position = new Vector3(cSeeker.transform.position.x, cSeeker.transform.position.y, cSeeker.transform.position.z);
                    
                        curCanvas.transform.rotation = cSeeker.transform.rotation;
                        
                        //doesnt need rot
                        //curCanvas.transform.Rotate(Vector3.right, 90);

                    
                        curCanvas.transform.localScale = new Vector3(canvasScalingFactor, canvasScalingFactor, canvasScalingFactor);
                       // curCanvas.transform.Translate(-trackerTransform.up * canvasDistance, Space.World);

                        pen.RescaleCanvas(curCanvas, curCamera.GetComponent<Camera>(), canvasScalingFactor);
                    }

                    //scale from rCube
                    else
                    {
                        curCanvas.transform.position = new Vector3(rCube.transform.position.x, rCube.transform.position.y, rCube.transform.position.z);
                    
                        curCanvas.transform.rotation = rCube.transform.rotation;
                        curCanvas.transform.Rotate(Vector3.right, 90);

                    
                        curCanvas.transform.localScale = new Vector3(canvasScalingFactor, canvasScalingFactor, canvasScalingFactor);
                        curCanvas.transform.Translate(-rCube.transform.up * canvasDistance, Space.World);

                        pen.RescaleCanvas(curCanvas, curCamera.GetComponent<Camera>(), canvasScalingFactor);
                    }
             
                }



                yield return null;
            }


            if (curCamera != null)
            {
                cameraDict.Add(curCanvas, curCamera);
                canvasScalingRunning = false;
            }
            savedDistance = canvasDistance;
            savedScale = canvasScalingFactor;
            setContainer(true);
            
            
            checkCulling();
            
            GameObject.Find("MotionStabilizer")
                .GetComponent<SmoothFollowPositionAndRotation>().scalecheck = false;
        }

        public void setContainer(bool calledFromRescale)
        {

            //quantization on and not called from rescale
            if (GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().quantization && !calledFromRescale)
            {
                
                if (GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().locationFreezeRelToTablet)
                {
                    //GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().instantPositionSnap();
                    GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().instantRotationSnap();
                }
                
                if (GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().locationFreezeRelToCanvas)
                {
                    //GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().instantPositionSnap();
                    GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().instantRotationSnap();
                }
                
                
                if (GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().rotationFreeze)
                {
                    GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().instantPositionSnap();
                    //GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().instantRotationSnap();
                }
                

                
                
                //all functions off
                if (!GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().locationFreezeRelToTablet &&
                    !GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().locationFreezeRelToCanvas &&
                    !GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().rotationFreeze)
                {
                    GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().instantPositionSnap();
                    GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().instantRotationSnap();
                }
                
                
                
                
            }
            
            
            if (holoToggle)
            {
                //case locationfreeze relativ to canvas
                if (GameObject.Find("MotionStabilizer")
                    .GetComponent<SmoothFollowPositionAndRotation>()
                    .locationFreezeRelToCanvas)
                {
                    GameObject.Find("CanvasContainer").transform.SetParent(GameObject.Find("CanvasSeeker").transform);
                }
                //all other cases
                else
                {
                    Debug.Log("Setting parent to RotationCube, holoToggle= " + holoToggle);
                    GameObject.Find("CanvasContainer").transform.SetParent(GameObject.Find("RotationCube").transform);
                    return;
                }
            }

            if (!holoToggle)
            {
                Debug.Log("Setting parent to StaticCanvas, holoToggle= " + holoToggle);
                GameObject.Find("CanvasContainer").transform.SetParent(GameObject.Find("StaticCanvas").transform);
            }
        }

        private float rtd(float f)
        {
            f = Mathf.Round(f * 1.0f) * 1.0f;
            return f;
        }

        private float rtd2(float f)
        {
            f = Mathf.Round(f * 10.0f) * 0.1f;
            return f;
        }


        //culling
        public void checkCulling()
        {
            if (GameObject.Find("MotionStabilizer").GetComponent<SmoothFollowPositionAndRotation>().culling)
            {
                //no culling
                curCamera.GetComponent<Camera>().nearClipPlane = -0.01f;
                curCamera.GetComponent<Camera>().farClipPlane = 0.01f;
                GameObject.Find("s3text").GetComponent<TextMesh>().text = "Off";
            }
            else
            {
                //standart
                curCamera.GetComponent<Camera>().nearClipPlane = -0.06f;
                curCamera.GetComponent<Camera>().farClipPlane = 1000f;
                GameObject.Find("s3text").GetComponent<TextMesh>().text = "On";
            }
            
            
            
        }
        
        

        //----------------------HoloDraw-----------------------




        public void setupPortal()
        {

            GameObject bt = GameObject.Find("SetupPortal");
            bt.transform.localScale *= 1.2f;

            StartCoroutine(Wait(bt));


            //if a portal is selected, don't set up a new one, but set the selected portal active
            GameObject curSelectedPortal = null;

            foreach (GameObject go in data.selectedGameObjects)
            {
                if (go.name == "Portal(Clone)")
                {
                    curSelectedPortal = go;
                }
            }

            if (curSelectedPortal != null)
            {
                resetCurrentPortal();
                return;
            }



            curPortal = Instantiate(Resources.Load("Portal", typeof(GameObject))) as GameObject;

            StartCoroutine("PositionPortal");

            //reset material of all other portals
            for (int i = 0; i < GameObject.Find("PortalContainer").transform.childCount; i++)
            {
                GameObject.Find("PortalContainer").transform.GetChild(i).GetComponent<Renderer>().material = (Material)Resources.Load("portalInactive", typeof(Material));
                GameObject.Find("PortalContainer").transform.GetChild(i).GetComponent<Highlighter>().material = (Material)Resources.Load("portalInactive", typeof(Material));
            }

            curPortal.transform.SetParent(GameObject.Find("PortalContainer").transform);




            if (curCamera != null)
            {
                curCamera.SetActive(false);
                //Close the UI and show the portal immediatelly
                setupUI();
            }


            curCamera = curPortal.GetComponentInChildren<Camera>().gameObject;
            portalCameraDict.Add(curPortal, curCamera);
        }

        private IEnumerator PositionPortal()
        {
            bool touching = true;
            while (touching)
            {
                touching = tablet_.pressure > 0 ? true : false;

                curPortal.transform.position = tablet_.transform.position;
                //adjust rotation
                curPortal.transform.rotation = tablet_.transform.rotation;
                curPortal.transform.Rotate(new Vector3(180, 0, 0));
                curPortal.transform.Translate(-trackerTransform.up / 4, Space.World);

                yield return null;
            }
        }

        public void resetCurrentPortal()
        {
            var temp = curPortal;

            if (curCamera != null)
            {
                curCamera.SetActive(false);
            }

            //search for last selected portal
            foreach (GameObject obj in data.selectedGameObjects)
            {
                if (obj.name == "Portal(Clone)")
                {
                    curPortal = obj;
                }
            }
            if (curPortal != temp)
            {
                //if (!data.selectedGameObjects.Contains(temp))
                //{
                //  temp.GetComponent<Renderer>().material = (Material)Resources.Load("portalActive", typeof(Material));
                // }



                temp.GetComponent<Renderer>().material = (Material)Resources.Load("portalInactive", typeof(Material));
                temp.GetComponent<Highlighter>().material = (Material)Resources.Load("portalInactive", typeof(Material));
            }

            if (curPortal != null)
            {
                curCamera = portalCameraDict[curPortal];
                curPortal.GetComponent<Renderer>().material = (Material)Resources.Load("portalActive", typeof(Material));
                curPortal.GetComponent<Highlighter>().material = (Material)Resources.Load("portalActive", typeof(Material));
                data.selectedGameObjects.Remove(curCanvas.gameObject);
                curPortal.GetComponent<Highlighter>().isSelected = false;
                curCamera.SetActive(true);

                //remove the portal from the selected objects
                data.selectedGameObjects.Remove(curPortal);
            }


        }



        public void deleteAll()
        {

            GameObject bt = GameObject.Find("DeleteAll");
            bt.transform.localScale *= 1.2f;

            StartCoroutine(Wait(bt));



            //Delete all Lines
            for (int j = 0; j < GameObject.Find("MeshExporter").transform.childCount; j++)
            {

                var temp = GameObject.Find("MeshExporter").transform.GetChild(j).gameObject;

                if (temp.name.Equals("GizmoShell"))
                {
                    temp = temp.transform.GetChild(0).gameObject;
                }

                if (temp.GetComponent<MeshFilter>().mesh.vertices.Length > 0)
                {


                    if (data.selectedGameObjects.Contains(temp))
                    {
                        data.selectedGameObjects.Remove(temp);
                    }



                    if (temp.transform.parent.name == "GizmoShell")
                    {
                        Destroy(temp.transform.parent.gameObject);
                    }
                    else
                    {
                        Destroy(temp);
                    }



                    //check for clone aswell
                    if (temp.GetComponent<FlagScript>().clone != null)
                    {

                        if (data.selectedGameObjects.Contains(temp.GetComponent<FlagScript>().clone))
                        {

                            data.selectedGameObjects.Remove(temp.GetComponent<FlagScript>().clone);
                        }

                        Destroy(temp.GetComponent<FlagScript>().clone.gameObject);
                    }
                }

            }

            //Delete all planes
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("drawingCanvas")) { 
                Destroy(obj);
            }

            //Delete all portals
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("portal"))
            {
                Destroy(obj);
            }

            data.selectedGameObjects.Clear();
        }



        public void deleteSelectedObjects()
        {

            GameObject bt = GameObject.Find("DeleteSelection");

            if (bt != null)
            {
                bt.transform.localScale *= 1.2f;
                StartCoroutine(Wait(bt));
            }

            foreach (GameObject obj in data.selectedGameObjects)
            {
                if (obj.name == "InputCanvas")
                {
                    Destroy(obj.transform.parent.gameObject);


                }
                else
                {
                    if (obj.transform.parent.name == "GizmoShell")
                    {
                        Destroy(obj.transform.parent.gameObject);
                    }
                    else
                    {
                        Destroy(obj);

                    }



                    if (obj.GetComponent<FlagScript>() != null)
                    {
                        if (obj.GetComponent<FlagScript>().clone != null)
                        {

                            if (obj.GetComponent<FlagScript>().clone.transform.parent.name == "GizmoShell")
                            {

                                Destroy(obj.GetComponent<FlagScript>().clone.transform.parent.gameObject);
                            }
                            else
                            {
                                Destroy(obj.GetComponent<FlagScript>().clone);
                            }

                        }
                    }


                }
            }


            data.selectedGameObjects.Clear();
            //data.selectedGameObjectMaterials.Clear();
        }

        public void toggleGizmo()
        {

            if (GameObject.Find("Toggle").GetComponent<Toggle>().isOn)
            {
                foreach (GameObject button in UIbuttonRef)
                {
                    button.SetActive(true);
                }
            }
            else
            {

                foreach (GameObject button in UIbuttonRef)
                {
                    button.SetActive(false);
                }
            }

        }

        //Somehow GizmoController properties are not turned false for lines
        public void TranslationGizmo()
        {

            GameObject bt = GameObject.Find("GizmoTranslation");

            if (bt != null)
            {

                bt.transform.localScale *= 1.2f;

                StartCoroutine(Wait(bt));

            }


            if (data.selectedGameObjects.Count > 0)
            {
                //Debug.Log(data.selectedGameObjects.Count);
                GameObject temp = data.selectedGameObjects[data.selectedGameObjects.Count - 1];
                GizmoController tempGizmo = null;
                // if (temp.name == "InputCanvas")
                // {

                if (temp.GetComponent<FlagScript>() != null)
                {

                    tempGizmo = temp.GetComponent<GizmoController>();
                }

                //tempGizmo = temp.transform.parent.gameObject.GetComponent<GizmoController>();
                //}
                //else
                //{

                /*for(int i = 0; i < tempGizmo.transform.childCount; i++)
                {
                    tempGizmo.transform.GetChild(i).rotation = Quaternion.identity;
                }*/
                //}

                if (tempGizmo != null)
                {

                    if (!tempGizmo.position)
                    {
                        tempGizmo.CreateRotationGizmo();
                        tempGizmo.CreateScalingGizmo();
                        tempGizmo.DestroyRotationGizmo();
                        tempGizmo.DestroyScalingGizmo();
                        tempGizmo.CreatePositionGizmo();
                    }
                    else
                        tempGizmo.DestroyPositionGizmo();

                }


            }
        }

        public void RotationGizmo()
        {
            GameObject bt = GameObject.Find("GizmoRotation");
            if (bt != null)
            {
                bt.transform.localScale *= 1.2f;
                StartCoroutine(Wait(bt));
            }


            if (data.selectedGameObjects.Count > 0)
            {
                GameObject temp = data.selectedGameObjects[data.selectedGameObjects.Count - 1];
                GizmoController tempGizmo = null;
                //if (temp.name == "InputCanvas")
                //  tempGizmo = temp.transform.parent.gameObject.GetComponent<GizmoController>();
                //else
                //{

                //check if object is a line
                if (temp.GetComponent<FlagScript>() != null)
                {
                    tempGizmo = temp.GetComponent<GizmoController>();
                }

                /*for (int i = 0; i < tempGizmo.transform.childCount; i++)
                {
                    tempGizmo.transform.GetChild(i).rotation = Quaternion.identity;
                }*/
                //}


                if (tempGizmo != null)
                {
                    if (!tempGizmo.rotation)
                    {
                        tempGizmo.CreatePositionGizmo();
                        tempGizmo.CreateScalingGizmo();
                        tempGizmo.DestroyPositionGizmo();
                        tempGizmo.DestroyScalingGizmo();
                        tempGizmo.CreateRotationGizmo();
                    }
                    else
                        tempGizmo.DestroyRotationGizmo();

                }

            }
        }

        public void ScalingGizmo()
        {

            GameObject bt = GameObject.Find("GizmoScaling");
            bt.transform.localScale *= 1.2f;

            StartCoroutine(Wait(bt));




            if (data.selectedGameObjects.Count > 0)
            {
                GameObject temp = data.selectedGameObjects[data.selectedGameObjects.Count - 1];
                GizmoController tempGizmo = null;
                //if (temp.name == "InputCanvas")
                //  tempGizmo = temp.transform.parent.gameObject.GetComponent<GizmoController>();
                //else
                //{

                //check if object is a line
                if (temp.GetComponent<FlagScript>() != null)
                {

                    tempGizmo = temp.GetComponent<GizmoController>();
                }

                /*for (int i = 0; i < tempGizmo.transform.childCount; i++)
                {
                    tempGizmo.transform.GetChild(i).rotation = Quaternion.identity;
                }*/
                //}

                if (tempGizmo != null)
                {

                    if (!tempGizmo.scaling)
                    {
                        tempGizmo.CreatePositionGizmo();
                        tempGizmo.CreateRotationGizmo();
                        tempGizmo.DestroyPositionGizmo();
                        tempGizmo.DestroyRotationGizmo();
                        tempGizmo.CreateScalingGizmo();
                    }
                    else
                        tempGizmo.DestroyScalingGizmo();

                }

            }
        }

        public void toggleRay()
        {
            if (pen.Cube.activeInHierarchy)
            {
                pen.Cube.SetActive(false);
                GizmoClickDetection.RAY_DISTANCE_CURRENT = GizmoClickDetection.RAY_DISTANCE_SHORT;
            }
            else
            {
                pen.Cube.SetActive(true);
                GizmoClickDetection.RAY_DISTANCE_CURRENT = GizmoClickDetection.RAY_DISTANCE_LONG;
            }

            GameObject bt = GameObject.Find("ToggleRay");
            bt.transform.localScale *= 1.2f;

            StartCoroutine(Wait(bt));


        }

        IEnumerator Wait(GameObject go)
        {

            yield return new WaitForSeconds(0.1f);
            go.transform.localScale /= 1.2f;
        }

        public void toggleHelperGrid()
        {
            if (helperGrid.activeInHierarchy)
            {
                helperGrid.SetActive(false);
            }
            else
            {
                helperGrid.SetActive(true);
            }

            GameObject bt = GameObject.Find("ToggleGrid");
            bt.transform.localScale *= 1.2f;

            StartCoroutine(Wait(bt));
        }



        public void toggleCanvas()
        {
            GameObject bt = GameObject.Find("ToggleCanvas");
            bt.transform.localScale *= 1.2f;

            StartCoroutine(Wait(bt));

            GameObject canvasContainer = GameObject.Find("CanvasContainer").gameObject;

            for (int i = 0; i < canvasContainer.transform.childCount; i++)
            {

                if (canvasContainer.transform.GetChild(i).transform.GetChild(0).GetComponent<MeshRenderer>().enabled == true)
                {
                    canvasContainer.transform.GetChild(i).transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                }
                else
                {
                    canvasContainer.transform.GetChild(i).transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                }
            }


        }

        public void copyObject()
        {
            GameObject bt = GameObject.Find("CopyLine");
            bt.transform.localScale *= 1.2f;

            StartCoroutine(Wait(bt));

            GameObject lastSelectedItem = null;

            if (data.selectedGameObjects.Count > 0)
            {
                lastSelectedItem = data.selectedGameObjects[data.selectedGameObjects.Count - 1];
            }


            if (lastSelectedItem != null)
            {

                //check if working on a Line Object
                if (lastSelectedItem.GetComponent<FlagScript>() != null)
                {
                    Vector3[] verts = lastSelectedItem.GetComponent<MeshFilter>().mesh.vertices;

                    float distortion = 0.1f;
                    if (lastSelectedItem.GetComponent<FlagScript>().drawnInMiniature)
                    {
                        //set a smaller distortion for the diminiture space
                        distortion /= (GameObject.Find("DiminitureSpace").GetComponent<DiminitureHandler>().scalingFactor / 2);
                    }

                    //Slightly move the copied object
                    for (int i = 0; i < verts.Length; i++)
                    {
                        verts[i] += (new Vector3(distortion, distortion, distortion));
                    }

                    //Copy the mesh properties
                    Mesh copiedMesh = new Mesh();
                    copiedMesh.vertices = verts;
                    copiedMesh.normals = lastSelectedItem.GetComponent<MeshFilter>().mesh.normals;
                    copiedMesh.tangents = lastSelectedItem.GetComponent<MeshFilter>().mesh.tangents;
                    copiedMesh.uv = lastSelectedItem.GetComponent<MeshFilter>().mesh.uv;
                    copiedMesh.triangles = lastSelectedItem.GetComponent<MeshFilter>().mesh.triangles;
                    copiedMesh.colors = lastSelectedItem.GetComponent<MeshFilter>().mesh.colors;

                    GameObject lineObj = Pen.DrawState.CreateLineObject(copiedMesh, lastSelectedItem.GetComponent<Renderer>().material);

                    lineObj.GetComponent<Renderer>().material = lastSelectedItem.GetComponent<Highlighter>().material;
                    lineObj.GetComponent<Highlighter>().material = lastSelectedItem.GetComponent<Highlighter>().material;

                    lineObj.GetComponent<MeshFilter>().mesh.RecalculateBounds();

                    Pen.DrawState.AddToGameObjectHirachie(pen.UndoManager, lineObj, pen.meshparent, pen.miniatureMeshParent, lastSelectedItem.GetComponent<FlagScript>().drawnInMiniature);

                    //Activate translation gizmo for copied line
                    data.selectedGameObjects.Add(lineObj);
                    TranslationGizmo();
                    //Remove the line afterwards, as it should not be selected by default
                    data.selectedGameObjects.Remove(lineObj);
                }

            }




        }




        public void extrudeObject()
        {

            GameObject bt = GameObject.Find("ExtrudeObject");
            bt.transform.localScale *= 1.2f;

            StartCoroutine(Wait(bt));




            pen.canDraw = false;
            if (data.selectedGameObjects.Count > 0)
            {
                if (data.selectedGameObjects[data.selectedGameObjects.Count - 1].GetComponent<FlagScript>() != null)
                {

                    GameObject.Find("Tablet").GetComponent<ExtrudeManager>().extrudeObject = data.selectedGameObjects[data.selectedGameObjects.Count - 1];
                    GameObject.Find("Tablet").GetComponent<ExtrudeManager>().extrudeForm = true;


                }

            }


        }

        /// <summary>
        /// Resetes the tablet view of the current/last selected canvas
        /// </summary>
        /// 


        public void resetCurrentCanvas()
        {



            var tmp = curCanvas;

            if (curCamera != null)
            {
                curCamera.SetActive(false);
            }

            foreach (GameObject obj in data.selectedGameObjects)
            {
                if (obj.name == "InputCanvas")
                {

                    curCanvas = obj.transform.parent.gameObject;

                }
            }

            if (!curCanvas.Equals(tmp))
            {
                if (!data.selectedGameObjects.Contains(tmp.transform.GetChild(0).gameObject))
                {
                    tmp.GetComponentInChildren<Renderer>().material = (Material)Resources.Load("InactivePlane", typeof(Material));
                }
                tmp.GetComponentInChildren<Highlighter>().material = (Material)Resources.Load("InactivePlane", typeof(Material));
            }

            curCamera = cameraDict[curCanvas];
            pen.RescaleCanvas(curCanvas, curCamera.GetComponent<Camera>(), curCamera.GetComponent<Camera>().orthographicSize);
            curCanvas.GetComponentInChildren<Renderer>().material = (Material)Resources.Load("ActivePlane", typeof(Material));
            curCanvas.GetComponentInChildren<Highlighter>().material = (Material)Resources.Load("ActivePlane", typeof(Material));
            //data.selectedGameObjectMaterials.RemoveAt(data.selectedGameObjects.IndexOf(curCanvas.transform.GetChild(0).gameObject));
            data.selectedGameObjects.Remove(curCanvas.transform.GetChild(0).gameObject);
            curCanvas.GetComponentInChildren<Highlighter>().isSelected = false;

            curCamera.SetActive(true);

        }



        /*private float map(float x, float in_min, float in_max, float out_min, float out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }*/

        private bool[] buttonFunctionRunning = { false, false, false, false };
        IEnumerator ButtonFunction(int i)
        {
            buttonFunctionRunning[i] = true;
            yield return new WaitUntil(() => buttons[i] == false);

            if (i == 0)
            {
                //Debug.Log(leftHandMode);
                //if (leftHandMode)
                //{
                    activateMiniatureSpace();
                //}
                //else
                //{

                //}
            }
            if (i == 1)
            {
                    setCurrentCanvas();
            }
            /* Buttons not used at the moment
            if (i == 2)
            {
                if (leftHandMode)
                {
                    setCurrentCanvas();
                }
                else
                {
                    activateMiniatureSpace();
                }
            }
            if (i == 3)
            {
                if (leftHandMode)
                {

                }
                else
                {

                }
            }*/

            buttonFunctionRunning[i] = false;
        }

        public void setupUI()
        {
            //Setup UI
            pen.drawInSpace = false;
            if (!canvasMaster.activeInHierarchy)
            {
                pen.canDraw = false;
                canvasMaster.SetActive(true);
                tabletCanvas.SetActive(false);
                uiActive = true;
            }
            else
            {
                //pen.canDraw = true;
                canvasMaster.SetActive(false);
                tabletCanvas.SetActive(true);
                uiActive = false;
            }
        }

        private void activateMiniatureSpace()
        {
            if (GameObject.Find("DiminitureSpace").GetComponent<DiminitureHandler>().diminitureWorldActive == false)
            {
                GameObject.Find("DiminitureSpace").GetComponent<DiminitureHandler>().diminitureWorldActive = true;
                GameObject.Find("DiminitureSpace").GetComponent<DiminitureHandler>().SetupDiminitureWorld();
                GameObject.Find("DiminitureSpace").GetComponent<DiminitureHandler>().showMiniatureWorld();
            }
            else if (GameObject.Find("DiminitureSpace").GetComponent<DiminitureHandler>().diminitureWorldActive == true)
            {
                GameObject.Find("DiminitureSpace").GetComponent<DiminitureHandler>().diminitureWorldActive = false;
                GameObject.Find("DiminitureSpace").GetComponent<DiminitureHandler>().fadeDiminitureWorld();
            }

        }

        private bool isCanvasSelected()
        {
            foreach (GameObject obj in data.selectedGameObjects)
            {
                if (obj.name == "InputCanvas")
                {
                    return true;
                }
            }
            return false;
        }

        private void setCurrentCanvas()
        {
            //set current canvas
            var tmp = curCanvas;

            if (curCamera != null)
            {
                curCamera.SetActive(false);
            }

            foreach (GameObject obj in data.selectedGameObjects)
            {
                if (obj.name == "InputCanvas")
                {

                    curCanvas = obj.transform.parent.gameObject;

                }
            }

            // if (!curCanvas.Equals(tmp))
            //{
            //   if (!data.selectedGameObjects.Contains(tmp.transform.GetChild(0).gameObject))
            // {

            if (tmp != null)
            {

                tmp.GetComponentInChildren<Renderer>().material = (Material)Resources.Load("InactivePlane", typeof(Material));
                tmp.GetComponentInChildren<Highlighter>().material = (Material)Resources.Load("InactivePlane", typeof(Material));
            }

            // }

            //}

            if (curCanvas != null)
            {
                curCamera = cameraDict[curCanvas];
                pen.RescaleCanvas(curCanvas, curCamera.GetComponent<Camera>(), curCamera.GetComponent<Camera>().orthographicSize);


                curCanvas.GetComponentInChildren<Renderer>().material = (Material)Resources.Load("ActivePlane", typeof(Material));
                curCanvas.GetComponentInChildren<Highlighter>().material = (Material)Resources.Load("ActivePlane", typeof(Material));
                data.selectedGameObjects.Remove(curCanvas.transform.GetChild(0).gameObject);
                curCanvas.GetComponentInChildren<Highlighter>().isSelected = false;
                curCamera.SetActive(true);

            }
        }

        private bool penDown = false;
        private bool drawing = false;


        private void HandlePenInput(InputInfo info)
        {

            bool touchedButton = false;

            if (!drawing)
            {


                /*if (info.pressure > 0)
                {
                    foreach (CustomButton cb in customButtons)
                    {
                        if ((info.x >= cb.startX && info.x <= (cb.startX + cb.sizeX)) && (info.y >= cb.startY && info.y <= (cb.startY + cb.sizeY)))
                        {
                            touchedButton = true;
                            //Debug.Log("touched cb " + cb.id + " at position: (" + info.x + "," + info.y + ")");

                            if (!penDown && !drawing)
                            {
                                penDown = true;
                                StartCoroutine(TriggerCustomButton(cb.id));
                                return;
                            }
                        }
                    }

                  
                    
                }*/

                if (!touchedButton && !canvasScalingRunning)
                {
                    drawing = pen.UpdateTrail(tablet_.pressure > 0 ? true : false);
                }
            }
            else
            {
                if (!canvasScalingRunning)
                {
                    if (tablet_.GetButton(1))
                    {
                        Debug.Log("test");
                    }
                    else
                    {
                        drawing = pen.UpdateTrail(tablet_.pressure > 0 ? true : false);
                    }
                }
            }

        }

        private IEnumerator TriggerCustomButton(int id)
        {
            yield return new WaitUntil(() => tablet_.pressure == 0);
            if ((tablet_.x >= customButtons[id].startX && tablet_.x <= (customButtons[id].startX + customButtons[id].sizeX)) && (tablet_.y >= customButtons[id].startY && tablet_.y <= (customButtons[id].startY + customButtons[id].sizeY)))
            {
                Debug.Log("Pressed button: " + id);
                if (id == 0)
                {

                }
            }
            penDown = false;
        }

        public void DisableAllGizmos()
        {
            GameObject bt = GameObject.Find("DisableAllGizmos");
            bt.transform.localScale *= 1.2f;

            StartCoroutine(Wait(bt));


            GizmoController[] allGizmoController = (GizmoController[]) Resources.FindObjectsOfTypeAll(typeof(GizmoController));
            foreach (GizmoController controller in allGizmoController)
            {
                controller.DestroyPositionGizmo();
                controller.DestroyRotationGizmo();
                controller.DestroyScalingGizmo();
            }
        }

    }

}