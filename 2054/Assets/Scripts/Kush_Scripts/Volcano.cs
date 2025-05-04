using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Volcano : MonoBehaviour
{
    public static Volcano instance;
    void Awake()
    {        if (instance == null) instance = this;        else Destroy(this);    }

    [Header("Target Point")]
    [SerializeField] Transform kitchenSinkTargetPoint;

    [Header("Bools")]
    public bool IsMixing { get; set; }
    public bool IsPlaced { get; set; } = false;
    [SerializeField] bool isPlacing = false;
    [SerializeField] bool isActive = false;
    [SerializeField] bool erupting = false;

    [Header("Particle Effects and their Parameters")]
    [SerializeField] List<ParticleSystem> smokeParticleSys = new();
    [SerializeField] List<ParticleSystem> fireParticleSys = new();
    [SerializeField] Vector2 smokeSysSize = new(2, 5);
    [SerializeField] Vector2 transformSize = new(1, 3);

    [Header("Image")]
    public Texture sciKitSiloutte;
    public Texture kitchenSinkSiloutte;

    //internal variables
    bool isDragging = false;
    bool[] chemicalsAdded = {false, false, false};
    Camera cam;
    Vector3 ZOffset;
    float YOffset;
    Rigidbody rb;
    float growingTime;
    float eruptingTime = 5f;
    float iniY;

    // Start is called before the first frame update
    void Start()
    {

        growingTime = Random.Range(15, 20);

        StartCoroutine(ShowScienceKitImg());

        IsMixing = false;

        rb = GetComponent<Rigidbody>();

        iniY = transform.position.y;
    }

    IEnumerator ShowScienceKitImg()
    {
        yield return new WaitForSeconds(5f);
        UIManager.instance.SetHelperText(sciKitSiloutte);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isDragging && isPlacing)
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
        //developer shortcut, REMOVE THIS LATER
        //if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F))
        //{
        //    ShowVolcanoMemory();
        //}

        if (IsMixing)
        {
            if (!chemicalsAdded[0])
            {
                UIManager.instance.SetHelperText("1");
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    chemicalsAdded[0] = true;
                }
            }
            else if (!chemicalsAdded[1])
            {
                UIManager.instance.SetHelperText("2");
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    chemicalsAdded[1] = true;
                }
            }
            else
            {
                UIManager.instance.SetHelperText("3");
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    chemicalsAdded[2] = true;
                }
            }

            if (chemicalsAdded[0] && chemicalsAdded[1] && chemicalsAdded[2])
            {
                isActive = true;
                isPlacing = true;
                IsMixing = false;

                kitchenSinkTargetPoint.gameObject.SetActive(true);

                MouseLookAround.instance.SetMouseLock(false);
            }
        }
        else if (isPlacing)
        {
           

            ZOffset = PlayerBehaviour.instance.transform.forward * 0.1f;

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
                rb.isKinematic = false;

                UIManager.instance.SetHelperText(kitchenSinkSiloutte);
            }
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

        ShowVolcanoMemory();
    }

    void ShowVolcanoMemory()
    {
        isPlacing = false;
        isDragging = false;

        

        if (IsPlaced)
        {
            StartCoroutine(SuccessMemory());
        }
        else
        {
            UIManager.instance.SetHelperText();
            StartCoroutine(FailMemory());
        }
    }

    IEnumerator SuccessMemory()
    {
        //fade in

        //freeze player
        GameManager.instance.StopGlitching();
        PuzzleManager.instance.FreezePlayer();

        StartCoroutine(UIEffects.instance.Fade(0, 1, 2, "Show Volcano Success Memory"));
        yield return new WaitForSeconds(2f);

        //play memory animation here
        //yield return new WaitForSeconds(animationDuration);
        //remove this line later (its for testing delay)

        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);

        StartCoroutine(UIEffects.instance.ScrollYear(1994, 2001, 0.5f, PlayerBehaviour.instance.AgePlayer));
    }


    IEnumerator FailMemory()
    {
        kitchenSinkTargetPoint.gameObject.SetActive(false);

        //fade in

        //freeze player
        GameManager.instance.StopGlitching();
        PuzzleManager.instance.FreezePlayer();

        StartCoroutine(UIEffects.instance.Fade(0, 1, 2, "Show Volcano Fail Memory"));
        yield return new WaitForSeconds(2f);

        //play memory animation here
        //yield return new WaitForSeconds(animationDuration);
        //remove this (delay) line later (its for testing delay)

        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);

        StartCoroutine(UIEffects.instance.ScrollYear(1994, 2001, 0.5f, PlayerBehaviour.instance.AgePlayer));
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

    Vector3 pos;
    void UpdatePosition()
    {
        UIManager.instance.SetHelperText(UIManager.KeyType.R, UIManager.KeyType.Q, UIManager.HelpType.MOVE_UP_DOWN);

        Vector3 targetDirection = PlayerBehaviour.instance.transform.forward;
        Quaternion rot = Quaternion.LookRotation(Vector3.Cross(Vector3.up, targetDirection), Vector3.up);
        transform.rotation = rot;

        if (Input.GetKey(KeyCode.Q))
        {
            YOffset -= 1f * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.R))
        {
            YOffset += 1f * Time.deltaTime;
        }

        pos = PlayerBehaviour.instance.transform.position + (ZOffset * 25f) + new Vector3(0, YOffset, 0);
        pos.y = Mathf.Clamp(pos.y, iniY - 1.5f, iniY + 2f);
        transform.position = pos;

        if (Vector3.Distance(this.transform.position, kitchenSinkTargetPoint.transform.position) <= 1f)
        {
            kitchenSinkTargetPoint.gameObject.SetActive(false);
            rb.isKinematic = false;
            transform.position = kitchenSinkTargetPoint.transform.position;
            IsPlaced = true;
            isPlacing = false;
            UIManager.instance.SetHelperText();
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = cam.WorldToScreenPoint(transform.position).z;
        return cam.ScreenToWorldPoint(mousePos);
    }
}
