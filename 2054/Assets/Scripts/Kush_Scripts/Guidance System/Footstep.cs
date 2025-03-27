using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GuidanceSystem.instance.NextStep(this.transform.rotation);
            Destroy(this.gameObject);
        }
    }
}
