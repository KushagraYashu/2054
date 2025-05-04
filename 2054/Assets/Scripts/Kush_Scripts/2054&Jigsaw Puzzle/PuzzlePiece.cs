using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class PuzzlePiece : MonoBehaviour
{
    [Header("Target Point")]
    public string targetPointName = string.Empty;

    [Header("Mesh Renderer")]
    [SerializeField] MeshRenderer meshRenderer;

    [Header("Bools")]
    public bool issolving = false;
    public bool solved = false;

    //internal variables
    bool isDragging = false;
    bool added = false;
    Vector3 offset = new();
    Camera cam;
    Transform targetPoint = null;
    Material objectMaterial;
    UnityEngine.Color emissionColor;
    float intensity;
    bool firstPickup = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !added)
        {
            AudioManager.instance.PlaySound(AudioManager.SoundType.PAPER_PICKUP);

            StopAllCoroutines();
            objectMaterial.SetColor("_EmissionColor", emissionColor * 1f);

            PuzzleManager.instance.AddPiece(this.gameObject);
            added = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        objectMaterial = meshRenderer.material;

        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        float elapsedTime = 0f;
        float totalDuration = 2f;
        emissionColor = objectMaterial.GetColor("_EmissionColor");
        objectMaterial.EnableKeyword("_EMISSION");
        intensity = -1f;

        while (true)
        {
            while (elapsedTime < totalDuration)
            {
                intensity = Mathf.Lerp(-1, 5, elapsedTime / totalDuration);
                objectMaterial.SetColor("_EmissionColor", emissionColor * intensity);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            intensity = 5f;
            objectMaterial.SetColor("_EmissionColor", emissionColor * intensity);
            elapsedTime = 0f;

            while (elapsedTime < totalDuration)
            {
                intensity = Mathf.Lerp(5, -1, elapsedTime / totalDuration);
                objectMaterial.SetColor("_EmissionColor", emissionColor * intensity);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            intensity = -1f;
            objectMaterial.SetColor("_EmissionColor", emissionColor * intensity);
            elapsedTime = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (issolving)
        {
            offset = PlayerBehaviour.instance.transform.forward * 0.1f;

            if (cam == null) cam = MouseLookAround.instance.GetCam();
            if (targetPoint == null)
                foreach (GameObject t in PuzzleManager.instance.GetTargetPoints())
                {
                    if (t.name.Contains(targetPointName))
                    {
                        targetPoint = t.transform;
                        break;
                    }
                }

            if (Input.GetMouseButton(0) && !isDragging && !PuzzleManager.instance.IsDraggingObject())
            {
                firstPickup = false;
                if(firstPickup) AudioManager.instance.PlaySound(AudioManager.SoundType.PAPER_PICKUP);
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform.gameObject.GetComponent<PuzzlePiece>() == this)
                    {
                        isDragging = true;
                        PuzzleManager.instance.SetDraggingObject(isDragging);
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0)) { 
                UIManager.instance.SetHelperText();
                isDragging = false;
                PuzzleManager.instance.SetDraggingObject(isDragging);
            }

            if (isDragging)
            {
                UpdatePosition();
            }
        }

        //Comment this later, its a dev shortcut
        //if(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.F))
        //{
        //    UIManager.instance.SetHelperText(UIManager.KeyType.R, UIManager.KeyType.Q, UIManager.HelpType.MOVE_CLOSE_FAR);
        //}
    }

    void UpdatePosition()
    {
        UIManager.instance.SetHelperText(UIManager.KeyType.R, UIManager.KeyType.Q, UIManager.HelpType.MOVE_CLOSE_FAR);

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
            AudioManager.instance.PlaySound(AudioManager.SoundType.PAPER_PICKUP);
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
        if (cam == null) cam = MouseLookAround.instance.GetCam();

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = cam.WorldToScreenPoint(transform.position).z;
        return cam.ScreenToWorldPoint(mousePos);
    }
}
