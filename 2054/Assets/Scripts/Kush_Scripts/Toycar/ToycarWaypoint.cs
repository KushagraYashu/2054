using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToycarWaypoint : MonoBehaviour
{
    public Transform nextWaypoint;

    public string waypointCode;

    private void Start()
    {
        int index = Random.Range(0, Toycar.instance.PossibleCodes.Length);
        waypointCode = Toycar.instance.PossibleCodes[index];
    }
}
