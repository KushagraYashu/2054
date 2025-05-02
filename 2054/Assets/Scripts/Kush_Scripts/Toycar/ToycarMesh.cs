using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToycarMesh : MonoBehaviour
{
    [Header("Bools")]
    public bool canPlay = false;
    public bool isDragging = false;
    public bool isDragable = true;

    //internal variables
    bool inRange = false;
    Vector3 ZOffset = new();
    float YOffset = 0f;
    Camera cam;
    Rigidbody rb;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && isDragable)
        {
            inRange = true;
            MouseLookAround.instance.SetMouseLock(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isDragable)
        {
            inRange = false;
            isDragging = false;

            UIManager.instance.SetHelperText();
            MouseLookAround.instance.SetMouseLock();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isDragable && inRange)
        {
            ZOffset = PlayerBehaviour.instance.transform.forward * 0.1f;

            if (cam == null)
                cam = MouseLookAround.instance.GetCam();

            if (Input.GetMouseButton(0) && !isDragging)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit) &&
                    hit.transform.gameObject.GetComponent<ToycarMesh>() == this)
                {
                    GameManager.instance.StopGlitching();
                    isDragging = true;
                }
            }
            if (Input.GetMouseButtonUp(0))
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
        UIManager.instance.SetHelperText(UIManager.KeyType.R, UIManager.KeyType.Q, UIManager.HelpType.MOVE_UP_DOWN);

        Vector3 targetDirection = PlayerBehaviour.instance.transform.forward;
        Quaternion rot = Quaternion.LookRotation(Vector3.Cross(Vector3.up, targetDirection), Vector3.up);
        transform.rotation = rot;

        if (Input.GetKey(KeyCode.Q))
            YOffset -= 1f * Time.deltaTime;
        else if (Input.GetKey(KeyCode.R))
            YOffset += 1f * Time.deltaTime;

        Vector3 pos = PlayerBehaviour.instance.transform.position + (ZOffset * 15f) + new Vector3(0, YOffset, 0);
        transform.position = pos;

        var targetPoint = Toycar.instance.toycarTargetPtTrans;
        if (Vector3.Distance(this.transform.position, targetPoint.transform.position) <= 1f)
        {
            isDragging = false;
            inRange = false;
            isDragable = false;
            targetPoint.gameObject.SetActive(false);
            transform.position = targetPoint.transform.position;
            transform.rotation = targetPoint.transform.rotation;
            Toycar.instance.SetPlay();
            this.enabled = false;

            MouseLookAround.instance.SetMouseLock();
        }
    }
}
