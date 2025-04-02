using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Camera Positions")]
    public Transform mainMenuCamPos;
    public Transform playerCamPos;

    [Header("Transistion Speed")]
    [SerializeField] float transitionSpeed = 2f;

    [Header("Canvases")]
    [SerializeField] GameObject mainMenuCanvas;

    [Header("Player")]
    [SerializeField] GameObject player;

    //internal variables
    Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = MouseLookAround.instance.GetCam();
        mainCamera.transform.position = mainMenuCamPos.position;
        MouseLookAround.instance.SetMouseLock(false);
        MouseLookAround.instance.lookAllowed = false;
    }

    IEnumerator TransitionCam()
    {
        float t = 0;
        mainMenuCamPos.transform.GetPositionAndRotation(out Vector3 startPosition, out Quaternion startRotation);
        while (t < 1)
        {
            t += Time.deltaTime * transitionSpeed;
            mainCamera.transform.SetPositionAndRotation(Vector3.Lerp(startPosition, playerCamPos.position, t), Quaternion.Lerp(startRotation, playerCamPos.rotation, t));
            yield return null;
        }

        mainCamera.transform.position = playerCamPos.position;
        yield return new WaitForSeconds(0.5f);
        MouseLookAround.instance.SetMouseLock();
        MouseLookAround.instance.lookAllowed = true;
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
