using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volcano : MonoBehaviour
{
    [Header("Target Point")]
    [SerializeField] Transform kitchenSinkTargetPoint;

    [Header("Bools")]
    public bool isPlacing = false;
    [SerializeField] bool isActive = false;
    [SerializeField] bool erupting = false;

    [Header("Particle Effects and their Parameters")]
    [SerializeField] List<ParticleSystem> smokeParticleSys = new();
    [SerializeField] List<ParticleSystem> fireParticleSys = new();
    [SerializeField] Vector2 smokeSysSize = new(2, 5);
    [SerializeField] Vector2 transformSize = new(1, 3);

    //internal variables
    bool isDragging = false;
    Camera cam;
    Vector3 offset;
    Rigidbody rb;
    float growingTime = 30f;
    float eruptingTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isDragging)
        {
            MouseLookAround.instance.SetMouseLock(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MouseLookAround.instance.SetMouseLock();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlacing)
        {

            offset = PlayerBehaviour.instance.transform.forward * 0.1f;

            if (cam == null)
                cam = MouseLookAround.instance.GetCam();

            if (Input.GetMouseButton(0) && !isDragging)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit) && 
                    hit.transform.gameObject.GetComponent<Volcano>() == this)
                {
                    GameManager.instance.StopGlitching();
                    isDragging = true;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                GameManager.instance.StartGlitching();
                isDragging = false;
            }

            if (isDragging)
            {
                rb.isKinematic = true;

                UpdatePosition();
            }
        }
        
        if(!isDragging)
        {
            rb.isKinematic = false;
        }

        if (isActive)
        {
            StartSmoke();
            isActive = false;
        }

        if(erupting)
        {
            StopCoroutine(GrowSmoke());

            StartFire();
            erupting = false;
        }
    }

    void StartFire()
    {
        foreach(ParticleSystem ps in fireParticleSys)
        {
            ps.Play();
        }

        StartCoroutine(GrowFire());
    }

    IEnumerator GrowFire()
    {
        float elapsedTime = 0f;
        while(elapsedTime < eruptingTime)
        {
            foreach (var ps in fireParticleSys)
            {
                float size = Mathf.Lerp(transformSize.x, transformSize.y, elapsedTime / eruptingTime);
                ps.transform.localScale = new Vector3(size, size, size);

                elapsedTime += Time.deltaTime;

                yield return null;
            }
        }
    }

    void StartSmoke()
    {
        foreach(ParticleSystem ps in smokeParticleSys)
        {
            var main = ps.main;
            main.startSize = new ParticleSystem.MinMaxCurve(1, smokeSysSize.x);
            ps.Play();
        }

        StartCoroutine(GrowSmoke());
    }

    IEnumerator GrowSmoke()
    {
        float elapsedTime = 0f;

        while(elapsedTime < growingTime)
        {
            foreach (var ps in smokeParticleSys)
            {
                var main = ps.main;
                main.startSize = new ParticleSystem.MinMaxCurve(1, Mathf.Lerp(smokeSysSize.x, smokeSysSize.y, elapsedTime / growingTime));

                float size = Mathf.Lerp(transformSize.x, transformSize.y, elapsedTime / growingTime);
                ps.transform.localScale = new Vector3(size, size, size);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        erupting = true;
    }

    void UpdatePosition()
    {
        //offset method, player can use Q and R to move the model closer or farther
        transform.position = PlayerBehaviour.instance.transform.position + (offset * 35f);
        return;

        if (Input.GetKey(KeyCode.Q))
        {
            transform.position = GetMouseWorldPosition() - offset;
        }
        else if (Input.GetKey(KeyCode.R))
        {
            transform.position = GetMouseWorldPosition() + offset;
        }
        else
            transform.position = GetMouseWorldPosition();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = cam.WorldToScreenPoint(transform.position).z;
        return cam.ScreenToWorldPoint(mousePos);
    }
}
