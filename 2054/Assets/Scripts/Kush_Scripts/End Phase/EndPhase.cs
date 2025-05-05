using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPhase : MonoBehaviour
{
    public static EndPhase instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    [Header("Sarah References")]
    public GameObject handMesh;
    public Vector3 handRotation = new(45, 0, 30);

    public void ExtendSarahHand()
    {
        StartCoroutine(RotateHand());
    }

    IEnumerator RotateHand()
    {
        float t = 0;
        Vector3 startRotation = handMesh.transform.localEulerAngles;
        Vector3 finalRoation = handRotation;
        while (t < 1)
        {
            t += Time.deltaTime * .25f;
            handMesh.transform.localEulerAngles = Vector3.Lerp(startRotation, finalRoation, t);

            yield return null;
        }

        handMesh.transform.localEulerAngles = finalRoation;
    }
}
