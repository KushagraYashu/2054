using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Item : MonoBehaviour
{
    public BoxManager.ItemType itemType;
    public Transform targetPoint;

    public bool correctItem;
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && correctItem)
        {
            MouseLookAround.instance.SetMouseLock(false);

            GameManager.instance.StopGlitching();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") && correctItem)
        {
            MouseLookAround.instance.SetMouseLock(true);

            GameManager.instance.StartGlitching();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (correctItem)
        {
            ZOffset = PlayerBehaviour.instance.transform.forward * 0.1f;
            if (Input.GetMouseButton(0) && !isDragging)
            {
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
            else if (Input.GetMouseButtonUp(0) && isDragging)
            {
                GameManager.instance.StartGlitching();

                
                isDragging = false;
            }

            if (isDragging)
            {
                UpdatePosition();
            }

            if (!isDragging)
            {
                rb.isKinematic = false;
                UIManager.instance.SetHelperText();
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

        Vector3 pos = PlayerBehaviour.instance.transform.position + (ZOffset * 25f) + new Vector3(0, YOffset, 0);
        transform.position = pos;

        if (Vector3.Distance(this.transform.position, targetPoint.transform.position) <= 1f)
        {
            targetPoint.gameObject.SetActive(false);
            rb.isKinematic = false;
            transform.SetPositionAndRotation(targetPoint.transform.position, targetPoint.transform.rotation);
            correctItem = false;
            isDragging = false;
            UIManager.instance.SetHelperText();
            if(itemType == BoxManager.ItemType.JERRY)
            {
                BoxManager.instance.PlayMemory(itemType, this.gameObject);
            }
            else
            {
                BoxManager.instance.PlayMemory(itemType);
            }
            this.enabled = false;
        }
    }

    public void Throw(Transform thPoint, Transform taPoint)
    {
        float randX = Random.Range(BoxManager.instance.throwForceMin.x, BoxManager.instance.throwForceMax.x);
        float randY = Random.Range(BoxManager.instance.throwForceMin.y, BoxManager.instance.throwForceMax.y);
        float randZ = Random.Range(BoxManager.instance.throwForceMin.z, BoxManager.instance.throwForceMax.z);

        if (!BoxManager.instance.AddItem(itemType))
        {
            transform.position = thPoint.position;
            
            rb.isKinematic = false;

            

            rb.AddForce(randX, randY, randZ, ForceMode.Impulse);

            this.enabled = false;
        }
        else
        {
            correctItem = true;

            transform.position = taPoint.position;
            transform.GetChild(0).gameObject.SetActive(true);

            rb.AddForce(randX, randY, randZ, ForceMode.Impulse);
        }
    }
}
