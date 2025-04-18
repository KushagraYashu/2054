using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Rotation Parameters")]
    [SerializeField] float rotationAmount = 70f;
    [SerializeField] float rotationSpeed = 2f;

    [Header("Door Collider")]
    [SerializeField] BoxCollider doorCollider;

    //internal variables
    Vector3 closedRotation;
    float threshold = 1f;

    private void Start()
    {
        closedRotation = transform.localEulerAngles;

        //debug line, remove later
        //StartCoroutine(OpenAndCloseDoor());
    }

    IEnumerator OpenAndCloseDoor()
    {
        while (transform.localEulerAngles.y < closedRotation.y + rotationAmount)
        {
            transform.localEulerAngles += new Vector3(0, rotationSpeed * Time.deltaTime, 0);
            yield return null;
        }
        doorCollider.enabled = false;
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, closedRotation.y + rotationAmount, transform.localEulerAngles.z);

        yield return new WaitForSeconds(5f);

        while(transform.localEulerAngles.y > closedRotation.y)
        {
            transform.localEulerAngles -= new Vector3(0, rotationSpeed * Time.deltaTime, 0);
            yield return null;
        }
        doorCollider.enabled = true;
        transform.localEulerAngles = closedRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Vector3.Magnitude(transform.localEulerAngles - closedRotation) < threshold)
        {
            StartCoroutine(OpenAndCloseDoor());
        }
    }
}
