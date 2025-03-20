using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectableObject : MonoBehaviour
{
    [Header("Mesh")]
    public Transform objectMesh;

    [Header("Bools")]
    [SerializeField] private bool canInspect = false;
    [SerializeField] private bool inspecting = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canInspect = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canInspect = false;
            inspecting = false;
        }
    }

    //internal variables
    Vector3 lastMousePosition = new Vector3();
    Vector3 delta;
    float rotationX;
    float rotationY;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(canInspect && Input.GetKeyDown(KeyCode.E))
        {
            inspecting = !inspecting;

            if (!inspecting)
            {
                PlayerBehaviour.instance.currentPlayerState = PlayerBehaviour.PlayerState.EXPLORING;
                MouseLookAround.instance.lookAllowed = true;
                MouseLookAround.instance.SetMouseLock();
            }
        }

        if (inspecting)
        {
            InspectObject();
        }
    }

    void InspectObject()
    {
        if (PlayerBehaviour.instance.currentPlayerState != PlayerBehaviour.PlayerState.INSPECTING)
            PlayerBehaviour.instance.currentPlayerState = PlayerBehaviour.PlayerState.INSPECTING;

        if (Input.GetMouseButton(0))
        {
            delta = Input.mousePosition - lastMousePosition;
            rotationX = delta.y * MouseLookAround.instance.rotationSpeed * Time.deltaTime;
            rotationY = -delta.x * MouseLookAround.instance.rotationSpeed * Time.deltaTime;

            objectMesh.Rotate(Vector3.up, rotationY, Space.World);
            objectMesh.Rotate(Vector3.right, -rotationX, Space.Self);

            lastMousePosition = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            lastMousePosition = Input.mousePosition;
        }
    }
}
