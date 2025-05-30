using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //By the orders of the Peaky Blinders, I now declare this script as the AudioManager.
    //place this script on an empty gameobject in the scene.

    public static AudioManager instance; //singleton instance
    private void Awake()
    {
        if (instance == null) instance = this; //a way of setting an instance
        else Destroy(gameObject); //if there is already an instance, destroy this one

        //instance allows us to access the AudioManager (only public items, ofcourse) from other scripts without the need to hard reference it.
    }

    [Header("Audio Events")] //this is a header for inspector (this makes things look cleaner and easier to manage)
    [SerializeField] //showing the variable the inspector, this is only useful for private variables
    private AK.Wwise.Event playerFootsteps;
    [SerializeField]//array of events for the player footstep, set this in the inspector to be the appropriate event [0th - toddler, 1st - child, 2nd - Teen, 3rd - Adult, if confused see the PlayerAge enum in PlayerBehaviour.cs], also see the PlayPlayerFootstep method.
    private AK.Wwise.Event StartButton;
    [SerializeField]
    private AK.Wwise.Event BackButton;
    [SerializeField]
    private AK.Wwise.Event QuitButton;
    [SerializeField]
    private AK.Wwise.Event DoorOpen;
    [SerializeField]
    private AK.Wwise.Event DoorClose;
    [SerializeField]
    private AK.Wwise.Event Tick;
    [SerializeField]
    private AK.Wwise.Event Vial;
    [SerializeField]
    private AK.Wwise.Event VolcanoExplosion;
    [SerializeField]
    private AK.Wwise.Event PaperPickUp;
    [SerializeField]
    private AK.Wwise.Event Wood_Floor_land;
    [SerializeField]
    private AK.Wwise.Event Carpet_land;
    [SerializeField]
    private AK.Wwise.Event Heartbeat_1;
    [SerializeField]
    private AK.Wwise.Event Keyboard;
    [SerializeField]
    private AK.Wwise.Event Musicbox;
    [SerializeField]
    private AK.Wwise.Event Weddingbells;
    [SerializeField]
    private AK.Wwise.Event Light;
    [SerializeField]
    private AK.Wwise.Event Celebration;
    [SerializeField]
    private AK.Wwise.Event Clapping;
    [SerializeField]
    private AK.Wwise.Event Correct;
    [SerializeField]
    private AK.Wwise.Event Wrong;
    [SerializeField]
    private AK.Wwise.Event Laugh;

    //I would recommend creating similar events and then their appropriate methods (make them public, so i can call them from other scripts using the singleton instance).



    //internal variables
    float timer = 0f; //timer for player footsteps
    uint heartbeatID;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public enum SoundType
    {
        START_BUTTON,
        BACK_BUTTON,
        QUIT_BUTTON,
        DOOROPEN,
        DOORCLOSE,
        VOLCANO_EXPLOSION,
        CLOCK_TICK,
        VIAL,
        PAPER_PICKUP, 
        WOOD_FLOOR_LAND,
        CARPET_LAND,
        HEARTBEAT_1,
        KEYBOARD,
        MUSICBOX,
        WEDDINGBELLS,
        LIGHTSWITCH,
        CELEBRATION,
        CLAPPING,
        CORRECT_PASSWORD,
        WRONG_PASSWORD,
        LAUGH,



    };

    public void PlaySound(SoundType soundType, GameObject obj = null)
    {
        switch (soundType)
        {
            case SoundType.START_BUTTON:
                if (obj != null) StartButton.Post(obj);
                else StartButton.Post(this.gameObject);
                break;

            case SoundType.BACK_BUTTON:
                if (obj != null) BackButton.Post(obj);
                else BackButton.Post(this.gameObject);
                break;

            case SoundType.QUIT_BUTTON:
                if (obj != null) QuitButton.Post(obj);
                else QuitButton.Post(this.gameObject);
                break;

            case SoundType.DOOROPEN:
                if ((obj != null)) DoorOpen.Post(obj);
                else DoorOpen.Post(this.gameObject);
                break;

            case SoundType.DOORCLOSE:
                if ((obj != null)) DoorClose.Post(obj);
                else DoorClose.Post(this.gameObject);
                break;

            case SoundType.VOLCANO_EXPLOSION:
                if ((obj != null)) VolcanoExplosion.Post(obj);
                else VolcanoExplosion.Post(this.gameObject);
                break;

            case SoundType.CLOCK_TICK:
                if ((obj != null)) Tick.Post(obj);
                else Tick.Post(this.gameObject);
                break;

            case SoundType.VIAL:
                if ((obj != null)) Vial.Post(obj);
                else Vial.Post(this.gameObject);
                break;

            case SoundType.PAPER_PICKUP:
                if ((obj != null)) PaperPickUp.Post(obj);
                else PaperPickUp.Post(this.gameObject);
                break;

           

            case SoundType.WOOD_FLOOR_LAND:
                if ((obj != null)) Wood_Floor_land.Post(obj);
                else Wood_Floor_land.Post(this.gameObject);
                break;

            case SoundType.CARPET_LAND:
                if ((obj != null)) Carpet_land.Post(obj);
                else Carpet_land.Post(this.gameObject);
                break;

            case SoundType.HEARTBEAT_1:
                if ((obj != null)) heartbeatID = Heartbeat_1.Post(obj);
                else Heartbeat_1.Post(this.gameObject);
                break;


            case SoundType.KEYBOARD:
                if ((obj != null)) Keyboard.Post(obj);
                else Keyboard.Post(this.gameObject);
                break;


            case SoundType.MUSICBOX:
                if ((obj != null)) Musicbox.Post(obj);
                else Musicbox.Post(this.gameObject);
                break;

            case SoundType.WEDDINGBELLS:
                if ((obj != null)) Weddingbells.Post(obj);
                else Weddingbells.Post(this.gameObject);
                break;

            case SoundType.LIGHTSWITCH:
                if ((obj != null)) Light.Post(obj);
                else Light.Post(this.gameObject);
                break;

            case SoundType.CELEBRATION:

                if ((obj != null)) Celebration.Post(obj);
                else Celebration.Post(this.gameObject);
                break;

            case SoundType.CLAPPING:

                if ((obj != null)) Clapping.Post(obj);
                else Clapping.Post(this.gameObject);
                break;

            case SoundType.CORRECT_PASSWORD:

                if ((obj != null)) Correct.Post(obj);
                else Correct.Post(this.gameObject);
                break;

            case SoundType.WRONG_PASSWORD:

                if ((obj != null)) Wrong.Post(obj);
                else Wrong.Post(this.gameObject);
                break;

            case SoundType.LAUGH:

                if ((obj != null)) Laugh.Post(obj);
                else Laugh.Post(this.gameObject);
                break;
        }
    }

    public void StopHeartSound(SoundType type, GameObject obj = null)
    {
        if(type == SoundType.HEARTBEAT_1)
        {
            Heartbeat_1.Stop(this.gameObject);
        }

    }

    /// <summary>
    /// Plays the player footstep sound
    /// </summary>
    /// <param name="moveVector">The move vector responsible for moving the player, this is used to check for movement. We only want to play sound when we move, right?</param>
    /// <param name="footstepSoundThreshold">The threshold for playing sound, we compare the timer against this. Also, we dont want to spam the footsteps, do we?</param>
    /// <param name="isGround">Bool to check if the player is grounded, we dont want to play the sound mid-air.</param>
    /// <param name="playerAge">The age of the player. We have different audio for different ages, why not use it.</param>
    /// <param name="gameObj">The gameobject to tie the footstep event to. Usually you should pass the player gameobject.</param>
    public void PlayPlayerFootstep(Vector3 moveVector, float footstepSoundThreshold, bool isGround, PlayerBehaviour.PlayerAge playerAge, GameObject gameObj) //this is a public method, so we can call it from other scripts. If you go to PLayerMovement.cs, you can see how we called it.
    {
        if(moveVector.magnitude > 0) timer += Time.deltaTime; //increasing the timer because we are moving
        
        if(timer > footstepSoundThreshold && isGround) //if we have moved for long enough, we can play the footstep sound (based on the age, ofcourse)
        {
            playerFootsteps.Post(gameObj);
            timer = 0f;
        }
    }
}
