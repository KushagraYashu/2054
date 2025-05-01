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

    public Camera GetCam() { return cam; }

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

    public void EndPhaseMouseTransition(Transform camPos1, Transform camPos2, Transform camPos3, float transitionSpeed)
    {
        StartCoroutine(FinalTransition(camPos1, camPos2, camPos3, transitionSpeed));
    }

    IEnumerator FinalTransition(Transform camPos1, Transform camPos2, Transform camPos3, float transitionSpeed)
    {
        float t = 0;
        cam.transform.GetPositionAndRotation(out Vector3 startPosition, out Quaternion startRotation);
        while (t < 1)
        {
            t += Time.deltaTime * transitionSpeed;
            cam.transform.SetPositionAndRotation(Vector3.Lerp(startPosition, camPos1.position, t), Quaternion.Lerp(startRotation, camPos1.rotation, t));

            yield return null;
        }

        cam.transform.position = camPos1.position;

        yield return new WaitForSeconds(5f);

        t = 0;
        cam.transform.GetPositionAndRotation(out startPosition, out startRotation);
        while (t < 1)
        {
            t += Time.deltaTime * transitionSpeed;
            cam.transform.SetPositionAndRotation(Vector3.Lerp(startPosition, camPos2.position, t), Quaternion.Lerp(startRotation, camPos2.rotation, t));

            yield return null;
        }

        cam.transform.position = camPos2.position;

        yield return new WaitForSeconds(7f);

        t = 0;
        cam.transform.GetPositionAndRotation(out startPosition, out startRotation);
        while (t < 1)
        {
            t += Time.deltaTime * transitionSpeed;
            cam.transform.SetPositionAndRotation(Vector3.Lerp(startPosition, camPos3.position, t), Quaternion.Lerp(startRotation, camPos3.rotation, t));

            yield return null;
        }

        cam.transform.position = camPos3.position;

        yield return new WaitForSeconds(10f);

        StartCoroutine(UIEffects.instance.Fade(0, 1, 2f));
        yield return new WaitForSeconds(2f);

        StartCoroutine(UIEffects.instance.ScrollYear(2018, 2054, 0.35f, null, true));
    }

    void ApplyLookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerTrans.Rotate(Vector3.up, mouseX);
    }
}