using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public static PlayerBehaviour instance;

    public enum PlayerState
    {
        EXPLORING,
        INSPECTING,
        PLAYING_WITH_CAR,
        SOLVING_PUZZLE,
        USING_LAPTOP,
        TOTAL,
    };

    [Header("Player State")]
    public PlayerState currentPlayerState = PlayerState.TOTAL;

    public enum PlayerAge { 
        Toddler,
        Child,
        Teenager,
        Adult,
        TOTAL
    }

    [Header("Player Age")]
    public PlayerAge playerAge = PlayerAge.Toddler;

    [Header("Audio Switches")]
    public Transform footstepAudioSwitchParent;

    [Header("Player Hand Things")]
    public GameObject handMesh;
    public Vector3 handRotation = new(-45, -45, 34.058f);

    // Internal Variables
    PlayerMovement movementComponent;
    float[] heights = new float[4];


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentPlayerState) { 
            case PlayerState.EXPLORING:
                movementComponent.MovePlayer();
                break;

            case PlayerState.INSPECTING:
                MouseLookAround.instance.lookAllowed = false;
                MouseLookAround.instance.SetMouseLock(false);
                break;
        }

        
    }

    void SetPrerequisites()
    {
        movementComponent = GetComponent<PlayerMovement>();

        //Player heights based on Age
        heights[0] = 0.72f;  //toddler
        heights[1] = 1.26f;  //child
        heights[2] = 1.62f;  //teen
        heights[3] = 1.80f;  //adult

    }

    public void SetPlayerHeight()
    {
        SetPrerequisites();

        for(int i=0; i<(int)PlayerAge.TOTAL; i++)
        {
            footstepAudioSwitchParent.GetChild(i).gameObject.SetActive(false);
        }
        footstepAudioSwitchParent.GetChild((int)playerAge).gameObject.SetActive(true);

        transform.localScale = new Vector3(1, heights[(int)playerAge], 1);
        if (movementComponent == null)
            movementComponent = GetComponent<PlayerMovement>();

        movementComponent.speed = movementComponent.maxSpeed * (float)(1 - (1.8 - heights[(int)playerAge]) / 1.8);
    }

    public void AgePlayer()
    {
        if(playerAge + 1 < PlayerAge.TOTAL)
        {
            playerAge += 1;
            SetPlayerHeight();
            RoomManager.instance.AgeRoom();
        }
        else
        {
            //Win condition
        }

        //fade out
        StartCoroutine(UIEffects.instance.Fade(1, 0, 2));

        //unfreeze player
        PuzzleManager.instance.UnfreezePlayer();

        //start glitching
        GameManager.instance.StartGlitching();
    }

    public void ExtendPlayerHand()
    {
        StartCoroutine(RotateHand());
    }

    IEnumerator RotateHand()
    {
        float t = 0;
        Vector3 startRotation = handMesh.transform.localEulerAngles;
        Vector3 finalRoation = handRotation;
        while (t < 1)
        {
            t += Time.deltaTime * .25f;
            handMesh.transform.localEulerAngles = Vector3.Lerp(startRotation, finalRoation, t);

            yield return null;
        }

        handMesh.transform.localEulerAngles = finalRoation;

        EndPhase.instance.ExtendSarahHand();

        //t = 0;
        //startRotation = handMesh.transform.rotation;
        //finalRoation = Quaternion.Euler(handMesh.transform.rotation.eulerAngles.x, handRotation.y, 0);
        //while (t < 1)
        //{
        //    t += Time.deltaTime * .25f;
        //    handMesh.transform.rotation = Quaternion.Lerp(startRotation, finalRoation, t);

        //    yield return null;
        //}

        //handMesh.transform.rotation = finalRoation;

        //t = 0;
        //startRotation = handMesh.transform.rotation;
        //finalRoation = Quaternion.Euler(handMesh.transform.rotation.eulerAngles.x, handMesh.transform.rotation.eulerAngles.x, handRotation.z);
        //while (t < 1)
        //{
        //    t += Time.deltaTime * .25f;
        //    handMesh.transform.rotation = Quaternion.Lerp(startRotation, finalRoation, t);

        //    yield return null;
        //}

        //handMesh.transform.rotation = finalRoation;
    }
}
