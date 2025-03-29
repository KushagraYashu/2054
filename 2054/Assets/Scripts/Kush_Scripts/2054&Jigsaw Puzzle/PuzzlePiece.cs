using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class PuzzlePiece : MonoBehaviour
{
    [Header("Target Point")]
    public string targetPointName = string.Empty;

    [Header("Bools")]
    public bool issolving = false;
    public bool solved = false;

    //internal variables
    bool isDragging = false;
    bool added = false;
    Vector3 offset = new Vector3();
    Camera cam;
    Transform targetPoint = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !added)
        {
            PuzzleManager.instance.AddPiece(this.gameObject);
            added = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (issolving)
        {
            if(cam == null) cam = MouseLookAround.instance.GetCam();
            if(targetPoint == null)
                foreach(GameObject t in PuzzleManager.instance.GetTargetPoints())
                {
                    if (t.name.Contains(targetPointName))
                        targetPoint = t.transform;
                }

            if (Input.GetMouseButton(0) && !isDragging && !PuzzleManager.instance.IsDraggingObject())
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.gameObject.GetComponent<PuzzlePiece>() == this)
                {
                    isDragging = true;
                    PuzzleManager.instance.SetDraggingObject(isDragging);
                    offset = transform.position - GetMouseWorldPosition();
                }
            }
            else if (Input.GetMouseButtonUp(0)) { 
                isDragging = false;
                PuzzleManager.instance.SetDraggingObject(isDragging);
            }

            if (isDragging)
            {
                UpdatePosition();
            }
        }
    }

    void UpdatePosition()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position = GetMouseWorldPosition() - offset;
        }
        else if(Input.GetKey(KeyCode.R))
        {
            transform.position = GetMouseWorldPosition() + offset;
        }
        else
        {
            transform.position = GetMouseWorldPosition();
        }
        if (Vector3.Distance(this.transform.position, targetPoint.transform.position) <= PuzzleManager.instance.solveThreshold)
        {
            targetPoint.GetComponentInChildren<MeshRenderer>().enabled = false;
            transform.position = targetPoint.position;
            issolving = false;
            isDragging = false;
            solved = true;
            PuzzleManager.instance.SetDraggingObject(false);
            PuzzleManager.instance.CheckSolved(true);
            GetComponent<BoxCollider>().enabled = false;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = cam.WorldToScreenPoint(transform.position).z;
        return cam.ScreenToWorldPoint(mousePos);
    }
}
