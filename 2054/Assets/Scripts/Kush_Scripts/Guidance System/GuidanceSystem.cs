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

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartSteps(List<Transform> waypoints)
    {
        waypointsTrans = waypoints;
        totSteps = waypoints.Count;
        curIndex = 0;

        curStep = InstantiateFoot();
        if (curIndex + 1 < waypointsTrans.Count - 1)
            curStep.transform.LookAt(waypointsTrans[curIndex + 1].transform);
    }

    public void NextStep(Quaternion lastRot)
    {
        if (curIndex < totSteps - 1)
        {
            curIndex++;
            curStep = InstantiateFoot();
            if (curIndex + 1 < waypointsTrans.Count - 1)
                curStep.transform.LookAt(waypointsTrans[curIndex + 1].transform);
            else
                curStep.transform.rotation = lastRot;
        }
        else
            return;
    }

    GameObject InstantiateFoot()
    {
        var obj = Instantiate(footstepPrefabs[leftRight % 2], this.waypointsTrans[curIndex].transform.position, footstepPrefabs[leftRight % 2].transform.rotation, this.waypointsTrans[curIndex].transform);
        leftRight++;

        return obj;
    }
}
