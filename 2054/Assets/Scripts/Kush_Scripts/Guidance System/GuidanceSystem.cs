using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidanceSystem : MonoBehaviour
{
    public static GuidanceSystem instance;

    [Header("Waypoints")] //pass this from the function you want to call the steps from (basically have a list of waypoints after each puzzle, so that can then be used to spawn the guidance steps)
    [SerializeField] List<Transform> waypointsTrans = new();

    [Header("Footstep")]
    [SerializeField] GameObject[] footstepPrefabs = new GameObject[2];


    //internal variables
    int curIndex = 0;
    int totSteps = 0;
    GameObject curStep;
    int leftRight = 0;
    Quaternion lastRot;
    bool stepsStarted = false;

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        lastRot = footstepPrefabs[0].transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearSteps()
    {
        StartCoroutine(DelSteps());    
    }

    IEnumerator DelSteps()
    {
        Destroy(curStep);
        yield return new WaitForEndOfFrame();

        stepsStarted = false;
    }

    public void StartSteps(List<Transform> waypoints)
    {
        //ClearSteps();

        stepsStarted = true;

        waypointsTrans = waypoints;
        totSteps = waypoints.Count;
        curIndex = 0;

        StartCoroutine(Start_Steps());

    }

    IEnumerator Start_Steps()
    {
        while (stepsStarted)
        {
            for(int i=curIndex; i<totSteps; i++)
            {
                curStep = InstantiateFoot(i);
                if (i + 1 < totSteps)
                    curStep.transform.LookAt(waypointsTrans[i + 1].transform);
                else
                    curStep.transform.rotation = lastRot;

                lastRot = curStep.transform.rotation;

                yield return new WaitForSeconds(0.5f);
                Destroy(curStep);
            }

        }
    }

    public void NextStep(Quaternion lastRot, int index)
    {
        if (curIndex < totSteps - 1)
        {
            curIndex = index;
            this.lastRot = lastRot;
        }
        else
        {
            if((curIndex + 1) == totSteps)
            {
                ClearSteps();
            }
        }
    }

    GameObject InstantiateFoot(int index)
    {
        var obj = Instantiate(footstepPrefabs[leftRight % 2], this.waypointsTrans[index].transform.position, footstepPrefabs[leftRight % 2].transform.rotation, this.waypointsTrans[index].transform);
        leftRight++;

        this.waypointsTrans[index].GetComponent<Footstep>().index = index;

        return obj;
    }
}
