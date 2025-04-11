using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake(){        if (instance == null) instance = this;        else Destroy(this);    }

    [Header("Camera Positions")]
    public Transform mainMenuCamPos;
    public Transform playerCamPos;

    [Header("Transistion Speed")]
    [SerializeField] float transitionSpeed = 2f;

    [Header("Lights")]
    [SerializeField]
    GameObject[] allLights;
    [SerializeField] GameObject[] livingRoomLights;
    [SerializeField] Material lightBoxMaterial;

    [Header("Canvases")]
    [SerializeField] GameObject mainMenuCanvas;

    [Header("Player")]
    [SerializeField] GameObject player;

    [Header("Tutorial Things")]
    [SerializeField] GameObject tutorialLight;
    [SerializeField] GameObject tutorialCollider;
    [SerializeField] Transform tutorialSpawnPt;
    [SerializeField] List<Transform> tutorialWaypoints = new();

    [Header("Glitch Mechanism")]
    public bool glitchAllowed = true;
    public Camera glitchCamera;
    [SerializeField] Vector2 waitTime = new(10f, 50f);
    [SerializeField] Vector2 glitchTime = new(0.2f, 1f);

    //internal variables
    Camera mainCamera;
    float t;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = MouseLookAround.instance.GetCam();
        mainCamera.transform.position = mainMenuCamPos.position;
        MouseLookAround.instance.SetMouseLock(false);
        MouseLookAround.instance.lookAllowed = false;

        StartCoroutine(Glitching());
    }

    public void StopGlitching()
    {
        glitchAllowed = false;

        StopAllCoroutines();

        glitchCamera.gameObject.SetActive(false);
        MouseLookAround.instance.GetCam().gameObject.SetActive(true);
        PuzzleManager.instance.UnfreezePlayer();
    }

    public void StartGlitching()
    {
        StopAllCoroutines();

        glitchAllowed = true;
        StartCoroutine(Glitching());
    }

    IEnumerator Glitching()
    {
        if (glitchAllowed)
        {
            float wait = Random.Range(waitTime.x, waitTime.y);

            yield return new WaitForSeconds(wait);

            StartCoroutine(DoGlitch());
        }
    }

    IEnumerator DoGlitch()
    {
        if (glitchAllowed)
        {
            float glitchTime = Random.Range(this.glitchTime.x, this.glitchTime.y);

            StartCoroutine(UIEffects.instance.Fade(1, 0, .5f));
            PuzzleManager.instance.FreezePlayer();

            glitchCamera.gameObject.SetActive(true);
            MouseLookAround.instance.GetCam().gameObject.SetActive(false);

            yield return new WaitForSeconds(glitchTime);

            glitchCamera.gameObject.SetActive(false);
            MouseLookAround.instance.GetCam().gameObject.SetActive(true);

            PuzzleManager.instance.UnfreezePlayer();
            StartCoroutine(UIEffects.instance.Fade(1, 0, .5f));

            if (glitchAllowed)
                StartCoroutine(Glitching());
        }
    }

    IEnumerator TransitionCam()
    {
        t = 0;
        mainMenuCamPos.transform.GetPositionAndRotation(out Vector3 startPosition, out Quaternion startRotation);
        while (t < 1)
        {
            t += Time.deltaTime * transitionSpeed;
            mainCamera.transform.SetPositionAndRotation(Vector3.Lerp(startPosition, playerCamPos.position, t), Quaternion.Lerp(startRotation, playerCamPos.rotation, t));

            Debug.Log("Moving Camera");

            yield return null;
        }

        mainCamera.transform.position = playerCamPos.position;
        yield return new WaitForSeconds(0.5f);
        MouseLookAround.instance.SetMouseLock();
        PuzzleManager.instance.UnfreezePlayer();

        LightTutorial();
    }

    public void StartGame()
    {
        StartCoroutine(TransitionCam());
        mainMenuCanvas.SetActive(false);
        ActivatePlayer();
    }

    void ActivatePlayer()
    {
        player.GetComponent<CharacterController>().enabled = true;
        player.GetComponent<PlayerBehaviour>().enabled = true;
        player.GetComponent<PlayerMovement>().enabled = true;
    }

    void LightTutorial()
    {
        lightBoxMaterial.DisableKeyword("_EMISSION");

        foreach(GameObject light in allLights)
        {
            if(!livingRoomLights.Contains(light))
            {
                light.GetComponent<Light>().enabled = false;
            }
        }

        tutorialLight.GetComponent<Light>().enabled = true;

        tutorialCollider.SetActive(true);

        GuidanceSystem.instance.StartSteps(tutorialWaypoints);
    }

    public void TutorialReset()
    {
        StartCoroutine(TutReset());
    }

    IEnumerator TutReset()
    {
        player.GetComponent<CharacterController>().enabled = false;
        player.GetComponent<PlayerBehaviour>().enabled = false;
        player.GetComponent<PlayerMovement>().enabled = false;
        player.transform.SetPositionAndRotation(tutorialSpawnPt.position, tutorialSpawnPt.rotation);
        StartCoroutine(UIEffects.instance.Fade(1, 0, 1f));
        yield return new WaitForSeconds(1f);
        player.GetComponent<CharacterController>().enabled = true;
        player.GetComponent<PlayerBehaviour>().enabled = true;
        player.GetComponent<PlayerMovement>().enabled = true;
        PuzzleManager.instance.UnfreezePlayer();
    }

    public void TutorialDone()
    {
        lightBoxMaterial.EnableKeyword("_EMISSION");

        foreach (GameObject light in allLights)
        {
            light.GetComponent<Light>().enabled = true;
        }

        tutorialLight.GetComponent<Light>().enabled = false;

        tutorialCollider.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
