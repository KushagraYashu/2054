using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public BoxManager.ItemType itemType;
    public Transform targetPoint;

    bool correctItem;
    bool isDragging = false;
    Camera cam;
    float YOffset;
    Vector3 ZOffset;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (correctItem)
        {
            if (Input.GetMouseButton(0) && !isDragging)
            {
                ZOffset = PlayerBehaviour.instance.transform.forward * 0.1f;
                if ((cam == null))
                {
                    cam = MouseLookAround.instance.GetCam();
                }
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform.gameObject.GetComponent<Item>() == this)
                    {
                        rb.isKinematic = true;

                        isDragging = true;
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                GameManager.instance.StartGlitching();

                rb.isKinematic = false;
                UIManager.instance.SetHelperText();
                isDragging = false;
            }

            if (isDragging)
            {
                UpdatePosition();
            }
        }
    }

    void UpdatePosition()
    {
        GameManager.instance.StopGlitching();

        UIManager.instance.SetHelperText("R - Move Up\nQ - Move Down");

        var objectMesh = transform.GetChild(0);
        Vector3 targetDirection = PlayerBehaviour.instance.transform.forward;
        Quaternion rot = Quaternion.LookRotation(Vector3.Cross(Vector3.up, targetDirection), Vector3.up);
        //objectMesh.rotation = rot;

        if (Input.GetKey(KeyCode.Q))
            YOffset -= 1f * Time.deltaTime;
        else if (Input.GetKey(KeyCode.R))
            YOffset += 1f * Time.deltaTime;

        Vector3 pos = PlayerBehaviour.instance.transform.position + (ZOffset * 25f) + new Vector3(0, YOffset, 0);
        transform.position = pos;

        if (Vector3.Distance(this.transform.position, targetPoint.transform.position) <= 1f)
        {
            isDragging = false;
            targetPoint.gameObject.SetActive(false);
            transform.position = targetPoint.transform.position;
            //transform.rotation = targetPoint.transform.rotation;
            UIManager.instance.SetHelperText();
            BoxManager.instance.PlayMemory(itemType);
            this.enabled = false;
        }
    }

    public void Throw(Transform thPoint, Transform taPoint)
    {
        if (!BoxManager.instance.AddItem(itemType))
        {
            transform.position = thPoint.position;

            
            rb.isKinematic = false;

            float randX = Random.Range(BoxManager.instance.throwForceMin.x, BoxManager.instance.throwForceMax.x);
            float randY = Random.Range(BoxManager.instance.throwForceMin.y, BoxManager.instance.throwForceMax.y);
            float randZ = Random.Range(BoxManager.instance.throwForceMin.z, BoxManager.instance.throwForceMax.z);

            rb.AddForce(randX, randY, randZ, ForceMode.Impulse);
        }
        else
        {
            correctItem = true;

            transform.position = taPoint.position;
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
