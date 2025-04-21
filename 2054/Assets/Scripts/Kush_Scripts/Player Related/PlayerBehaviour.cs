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
        movementComponent = GetComponent<PlayerMovement>();

        //Player heights based on Age
        heights[0] = 0.72f;  //toddler
        heights[1] = 1.26f;  //child
        heights[2] = 1.62f;  //teen
        heights[3] = 1.80f;  //adult

        SetPlayerHeight();
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

    void SetPlayerHeight()
    {
        for(int i=0; i<(int)PlayerAge.TOTAL; i++)
        {
            footstepAudioSwitchParent.GetChild(i).gameObject.SetActive(false);
        }
        footstepAudioSwitchParent.GetChild((int)playerAge).gameObject.SetActive(true);

        transform.localScale = new Vector3(1, heights[(int)playerAge], 1);
        movementComponent.speed = 10 * (float)(1 - (1.8 - heights[(int)playerAge]) / 1.8);
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
}
