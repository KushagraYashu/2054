using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToycarWaypoint : MonoBehaviour
{
    public Transform nextWaypoint;

    public string waypointCode;

    public GameObject[] codeMeshes;
    public Material codeMaterial;

    GameObject codeToSpawn;

    GameObject spawnedCodeMesh;

    public void SpawnCode()
    {
        if(waypointCode == "\"")
        {
            spawnedCodeMesh = Instantiate(codeToSpawn, this.transform.position, codeToSpawn.transform.rotation, this.transform);

            var mesh1 = spawnedCodeMesh.transform.GetChild(0);
            mesh1.GetComponent<MeshRenderer>().material = codeMaterial;

            var mesh2 = spawnedCodeMesh.transform.GetChild(1);
            mesh2.GetComponent<MeshRenderer>().material = codeMaterial;
        }
        else
        {
            spawnedCodeMesh = Instantiate(codeToSpawn, this.transform.position, codeToSpawn.transform.rotation, this.transform);
            spawnedCodeMesh.GetComponent<MeshRenderer>().material = codeMaterial;
        }
    }

    private void Start()
    {
        int index = Random.Range(0, Toycar.instance.PossibleCodes.Length);
        waypointCode = Toycar.instance.PossibleCodes[index];

        codeToSpawn = codeMeshes[index];
    }
}
