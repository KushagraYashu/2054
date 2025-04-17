using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour
{
    public int index { get; set; }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GuidanceSystem.instance.NextStep(this.transform.rotation, index);
        }
    }
}
