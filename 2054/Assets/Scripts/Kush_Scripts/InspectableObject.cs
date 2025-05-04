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
    [SerializeField] private bool dragable = false;
    [SerializeField] private bool isVolcanoKit = false;

    [Header("Target Point")]
    [SerializeField] Transform targetPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!dragable)
            {
                canInspect = true;

                GameManager.instance.StopGlitching();
            }

            MouseLookAround.instance.SetMouseLock(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!dragable)
            {
                canInspect = false;
                inspecting = false;

                GameManager.instance.StartGlitching();
            }

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
    float YOffset;
    Vector3 ZOffset;
    Vector3 pos;
    bool isDragging = false;
    float iniY;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        iniY = transform.position.y;
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
        }else if (dragable)
        {
            ZOffset = PlayerBehaviour.instance.transform.forward * 0.1f;

            if (cam == null)
                cam = MouseLookAround.instance.GetCam();

            if (Input.GetMouseButton(0) && !isDragging)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit) &&
                    hit.transform.gameObject.GetComponent<InspectableObject>() == this)
                {
                    GameManager.instance.StopGlitching();
                    isDragging = true;
                }
            }
            else if (Input.GetMouseButtonUp(0) && isDragging)
            {
                GameManager.instance.StartGlitching();
                isDragging = false;
            }

            if (isDragging)
            {
                rb.isKinematic = true;

                UpdatePosition();
            }

            if (!isDragging)
            {
                UIManager.instance.SetHelperText();
                rb.isKinematic = false;
            }
        }
    }

    void UpdatePosition()
    {
        try
        {
            UIManager.instance.SetHelperText(UIManager.KeyType.R, UIManager.KeyType.Q, UIManager.HelpType.MOVE_UP_DOWN);
        }
        catch { }

        Vector3 targetDirection = PlayerBehaviour.instance.transform.forward;
        Quaternion rot = Quaternion.LookRotation(Vector3.Cross(Vector3.up, targetDirection), Vector3.up);
        objectMesh.rotation = rot;

        if (Input.GetKey(KeyCode.Q))
            YOffset -= 1f * Time.deltaTime;
        else if (Input.GetKey(KeyCode.R))
            YOffset += 1f * Time.deltaTime;

        Vector3 pos = PlayerBehaviour.instance.transform.position + (ZOffset * 25f) + new Vector3(0, YOffset, 0);
        pos.y = Mathf.Clamp(pos.y, iniY - 1.5f, iniY + 2f);
        transform.position = pos;

        if (Vector3.Distance(this.transform.position, targetPoint.transform.position) <= 1f)
        {
            isDragging = false;
            targetPoint.gameObject.SetActive(false);
            transform.position = targetPoint.transform.position;
            objectMesh.rotation = targetPoint.transform.rotation;
            Volcano.instance.IsMixing = true;
            this.enabled = false;
        }
    }

    IEnumerator MakeDraggable()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<BoxCollider>().size = new Vector3(7, 7, 7);
        PlayerBehaviour.instance.currentPlayerState = PlayerBehaviour.PlayerState.EXPLORING;
        MouseLookAround.instance.lookAllowed = true;
        rb.isKinematic = true;
        canInspect = false;
        inspecting = false;

        yield return new WaitForSeconds(2f);
        transform.localEulerAngles = new Vector3(0, -180, 0);
        dragable = true;
        var objs = FindObjectsByType<InspectableObject>(FindObjectsSortMode.None);
        foreach(var obj in objs)
        {
            if(obj != this)
            {
                obj.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    void InspectObject()
    {
        if (isVolcanoKit)
            StartCoroutine(MakeDraggable());

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
