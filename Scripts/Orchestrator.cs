/*
 * Game orchestrator - manages game states, responsible for generating objects
 * 
 * author: Klaudia Fajtova 
 * login: xfajto00
 * 
 */

using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Orchestrator : MonoBehaviour {

    private float area = 50.0f;
    private float radius = 0.4f;

    private int placedCupcakes = 0;
    private int collidedCupcakes = 100;
    private int locationsNumber = 0;
    private int previousPosition = 0;
    private int tips = 1;
    private int repeats = 0;

    public bool menu = false;
    private bool end = false;
    private bool noCups = false;
    private bool waiting = false;
    private bool scanDone = false;

    private ThirdPersonCharacter ethan;
    private DirectionalIndicator directionalIndicator;
    public GameObject text;
    private GameObject floor;
    private CupcakeManager cupcakeManager;
    private List<int> usedPositions;
    private SpatialUnderstandingDllTopology.TopologyResult[] _resultsTopology;
    private Vector3 startPosition;


    void Start () {
        ethan = GetComponent<ThirdPersonCharacter>();

        floor = GameObject.Find("Floor");

        GameObject indicatorManager = GameObject.Find("DirectionalIndicatorManager");

        directionalIndicator = indicatorManager.GetComponent<DirectionalIndicator>();

        text.transform.SetParent(Camera.main.transform, true);
        text.transform.localPosition = new Vector3(0, 0, 3); 

        cupcakeManager = GameObject.Find("CupcakeManager").GetComponent<CupcakeManager>();

        startPosition = Camera.main.transform.position + Camera.main.transform.forward * 1.2f;

        usedPositions = new List<int>();

        SpatialUnderstanding.Instance.ScanStateChanged += Instance_ScanStateChanged;
        HideObjects();
	}
	
	void Update () {
        
		if (!scanDone && (CanCompleteScan() || GameObject.Find("ThirdPersonController").GetComponent<ThirdPersonHoloLensControl>().endScan))
        {
            SpatialUnderstanding.Instance.RequestFinishScan();
            scanDone = true;
        }

        if (scanDone && !menu && !end)
        {
            TextAfterScan();
            CheckCollision();
            if(placedCupcakes > 0)
                directionalIndicator.active = true;

            SpatialUnderstanding.Instance.RequestBeginScanning();

            if (ethan.transform.position.y < -3.0f)
            {
                ethan.transform.position = startPosition;
            }
        }

        if (GameObject.Find("ThirdPersonController").GetComponent<ThirdPersonHoloLensControl>().chosen)
        {
            GameObject.Find("ThirdPersonController").GetComponent<ThirdPersonHoloLensControl>().chosen = false;

            ResetLevel();
            menu = false;
        }

        if (collidedCupcakes == placedCupcakes && !end && !GameObject.Find("ThirdPersonController").GetComponent<ThirdPersonHoloLensControl>().chosen)
        {
            placedCupcakes = 0;
            collidedCupcakes = 100;

            GameObject.Find("ThirdPersonController").GetComponent<ThirdPersonHoloLensControl>().again = true;
            menu = true;
                      
            text.GetComponent<TextMesh>().text = "Congratulation!\n If you want to play again, select X";
        }


        if (end)
        {
            text.GetComponent<TextMesh>().text = "Congratulation!\n YOU BEAT THE GAME!";
            HideCupcakes();
        }

    }

    private void ResetLevel()
    {        
        //Ethan rested but he still fat
        ethan.m_JumpPower = 6f;
        ShowCupcakes();        
    }

    /*** SCANNING ***/
    private void Instance_ScanStateChanged()
    {
        if ((SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done) && SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            SpatialUnderstanding.Instance.GetComponent<SpatialUnderstandingCustomMesh>().DrawProcessedMesh = false;
            ShowObjects();
            scanDone = true;
        }
    }

    IEnumerator TipsAndTricks(float time)
    {
        waiting = true;
        yield return new WaitForSeconds(time);

        string textToShow;
        string end = "\nJust select X when you're done scanning";

        switch (tips) {
            case 1:
                textToShow = "Select RB button to show spatial meshes" + end;
                break;
            case 2:
                textToShow = "Select 'A' to jump" + end;
                break;
            case 3:
                textToShow = "Select 'B' to crouch" + end;
                break;
            case 4:
                textToShow = "Your jumping skills decrease after each cupcake" + end;
                break;
            default:
                textToShow = "Collect all cupcakes!" + end;
                break;
        }

        text.GetComponent<TextMesh>().text = tips + "/5 " + textToShow;

        if (tips != 5)
            tips++;
        else
            tips = 1;

        waiting = false;
    }

    private bool CanCompleteScan()
    {
        SpatialUnderstandingDll.Imports.PlayspaceStats stats = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStats();

        if (!waiting)
            StartCoroutine(TipsAndTricks(8));      

        if ((SpatialUnderstanding.Instance.ScanState != SpatialUnderstanding.ScanStates.Scanning) ||
            !SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            return false;
        }

        IntPtr statsPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStatsPtr();
        if (SpatialUnderstandingDll.Imports.QueryPlayspaceStats(statsPtr) == 0)
        {
            return false;
        }

        if (stats.UpSurfaceArea > area)
        {
            return true;
        }

        return false;
    }


    /*** OBJECTS ***/

    //hide
    private void HideObjects()
    {
        HideEthan();
        HideCupcakes();
    }

    private void HideEthan()
    {
        var body = GameObject.Find("EthanBody");
        var glasses = GameObject.Find("EthanGlasses");

        body.GetComponent<SkinnedMeshRenderer>().enabled = false;
        glasses.GetComponent<SkinnedMeshRenderer>().enabled = false;
    }

    private void HideCupcakes()
    {
        foreach (GameObject part in GameObject.FindGameObjectsWithTag("Part"))
        {
            part.GetComponent<MeshRenderer>().enabled = false;
        }    
    }

    //show
    private void ShowObjects()
    {
        ShowEthan();
        ShowCupcakes();
        
    }

    private void ShowEthan()
    {
        /*** Place Ethan ***/
        ethan.transform.position = startPosition;
        startPosition.y = 0;
        floor.transform.position = startPosition;


        /*** Show Ethan ***/
        GameObject.Find("EthanBody").GetComponent<SkinnedMeshRenderer>().enabled = true;
        GameObject.Find("EthanGlasses").GetComponent<SkinnedMeshRenderer>().enabled = true;

        var rigidBody = GetComponent<Rigidbody>();
        rigidBody.angularVelocity = Vector3.zero;
        rigidBody.velocity = Vector3.zero;
    }

    private void ShowCupcakes()
    {
        /*** Place ***/
        if (repeats == 0)
        {
            _resultsTopology = new SpatialUnderstandingDllTopology.TopologyResult[512];
            IntPtr resultsTopologyPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(_resultsTopology);
            locationsNumber = SpatialUnderstandingDllTopology.QueryTopology_FindPositionsSittable(0.7f, 2.4f, 0.4f, _resultsTopology.Length, resultsTopologyPtr);
        }
        
        previousPosition = UnityEngine.Random.Range(0, locationsNumber);

        if (locationsNumber > 0)
        {
            if (usedPositions.Count == locationsNumber)
            {
                end = true;
            }

            for (int i = 0; i < locationsNumber; i++) {

                if (end)
                    break;

                //set radius for each cupcake
               if (!usedPositions.Contains(i) &&
                    ((_resultsTopology[i].position.x > _resultsTopology[previousPosition].position.x + radius) || (_resultsTopology[i].position.x < _resultsTopology[previousPosition].position.x - radius))
                    && ((_resultsTopology[i].position.z > _resultsTopology[previousPosition].position.z + radius) || (_resultsTopology[i].position.z < _resultsTopology[previousPosition].position.z - radius)))
                {
                    cupcakeManager.cupcakeList[placedCupcakes].transform.position = _resultsTopology[i].position;
                    usedPositions.Add(i);
                    previousPosition = i;
                    placedCupcakes++;
                }

                if (placedCupcakes == 5)
                {
                    break;
                }
            }

            collidedCupcakes = 0;
            noCups = false;
            repeats++;
        }
        else
        {
            //no place for cupcakes
            noCups = true;
        }
                

        /*** Show ***/
        for (int i = 0; i < placedCupcakes; i++)
        {
            GameObject c = cupcakeManager.cupcakeList[i];
            c.GetComponent<CollisionManager>().enabled = true;
            Transform t = c.transform;
            foreach(Transform tr in t)
            {
                if(tr.CompareTag("Part"))
                {
                    tr.GetComponent<MeshRenderer>().enabled = true;
                }
            }
            c.GetComponent<Target>().placed = true;
            c.GetComponent<Target>().needArrowIndicator = true;
            c.tag = "Target";
        }

        foreach (GameObject c in cupcakeManager.cupcakeList)
        {
            if (c.GetComponent<Target>().placed)           
                c.GetComponent<CollisionManager>().collisionAvailable = true;
        }
    }

    private void TextAfterScan()
    {
        if (!noCups && collidedCupcakes < placedCupcakes)
            text.GetComponent<TextMesh>().text = "Score: " + collidedCupcakes + " / " + placedCupcakes;
        else if (noCups)
            text.GetComponent<TextMesh>().text = "There is no place for cupcakes. Please scan again";
    }


    /*** COLLISION ***/
    private void CheckCollision()
    {
        foreach (GameObject c in cupcakeManager.cupcakeList)
        {
            if (c.GetComponent<CollisionManager>().collided)
            {
                ethan.Move(Vector3.zero, true, false);
                GameObject.Find("EthanSkeleton").transform.localScale += new Vector3(0.03f, 0, 0.03f);
                ethan.m_JumpPower -= 0.4f;

                foreach (MeshRenderer part in c.GetComponentsInChildren<MeshRenderer>()) 
                {
                    part.enabled = false;
                }

                c.GetComponent<Target>().needArrowIndicator = false;
                collidedCupcakes++;
                c.GetComponent<CollisionManager>().collisionAvailable = false;
                c.GetComponent<CollisionManager>().collided = false;
                c.GetComponent<Target>().placed = false;
            }
        }
    }
}
