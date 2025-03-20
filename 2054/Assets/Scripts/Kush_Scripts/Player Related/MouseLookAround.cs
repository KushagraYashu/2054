using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookAround : MonoBehaviour
{
    public static MouseLookAround instance;

    [Header("Mouse Variables")]
    [SerializeField] float sensitivity = 10f;
    [SerializeField] float FOVDefault = 60f;
    public bool lookAllowed = true;

    [Header("Player Reference")]
    [SerializeField] Transform playerTrans;

    [Header("Inspect Mechanic Related")]
    public float rotationSpeed = 5f;

    //internal variables
    private float xRotation = 0;
    private Camera cam;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();

        //Debug Mouse lock, remove later
        SetMouseLock();
    }

    // Update is called once per frame
    void Update()
    {
        if (lookAllowed)
        {
            ApplyLookAround();
        }
    }

    // Change FOV, pass false to change, pass true to use default
    public void ChangeFOV(float fov = 0, bool fovDefault = true)
    {
        if (!fovDefault)
        {
            cam.fieldOfView = fov;
        }
        else
        {
            cam.fieldOfView = FOVDefault;
        }
    }

    public void SetMouseLock(bool state = true)
    {
        if (state)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }

    void ApplyLookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -60, 60);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerTrans.Rotate(Vector3.up, mouseX);
    }
}