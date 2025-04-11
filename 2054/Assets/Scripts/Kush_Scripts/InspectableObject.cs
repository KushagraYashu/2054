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

            GameManager.instance.StopGlitching();

            MouseLookAround.instance.SetMouseLock(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canInspect = false;
            inspecting = false;

            GameManager.instance.StartGlitching();

            MouseLookAround.instance.SetMouseLock();
        }
    }

    //internal variables
    Vector3 lastMousePosition = new Vector3();
    Vector3 delta;
    float rotationX;
    float rotationY;
    Camera cam;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(canInspect)
        {
            if (cam == null) cam = MouseLookAround.instance.GetCam();

            if (Input.GetMouseButton(0) && !inspecting)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit) &&
                    hit.transform.gameObject.GetComponentInParent<InspectableObject>() == this)
                {
                    rb.isKinematic = true;

                    GameManager.instance.StopGlitching();

                    inspecting = true;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                rb.isKinematic = false;

                inspecting = false;
            }

            if (!inspecting)
            {
                PlayerBehaviour.instance.currentPlayerState = PlayerBehaviour.PlayerState.EXPLORING;
                MouseLookAround.instance.lookAllowed = true;
            }

            if (inspecting)
            {
                InspectObject();
            }
        }
    }

    void InspectObject()
    {
        if (PlayerBehaviour.instance.currentPlayerState != PlayerBehaviour.PlayerState.INSPECTING)
            PlayerBehaviour.instance.currentPlayerState = PlayerBehaviour.PlayerState.INSPECTING;

        delta = Input.mousePosition - lastMousePosition;
        rotationX = delta.y * MouseLookAround.instance.rotationSpeed * Time.deltaTime;
        rotationY = -delta.x * MouseLookAround.instance.rotationSpeed * Time.deltaTime;

        objectMesh.Rotate(Vector3.up, rotationY, Space.World);
        objectMesh.Rotate(Vector3.right, -rotationX, Space.Self);

        lastMousePosition = Input.mousePosition;
    }
}
