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
        TOTAL,
    };

    [Header("Player State")]
    public PlayerState currentPlayerState = PlayerState.EXPLORING;


    // Internal Variables
    PlayerMovement movementComponent;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        movementComponent = GetComponent<PlayerMovement>();
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
}
