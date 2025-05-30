using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;

    [Header("Rooms (Mesh Parents)")]
    [SerializeField] private List<GameObject> rooms = new();
    public GameObject GetRoom(int index)
    {
        return rooms[index];
    }

    public enum Room
    {
        Toddler,
        Child,
        Teen,
        Adult,
        TOTAL
    };

    [Header("Room")]
    public Room currentRoomType = Room.Toddler;

    [Header("End Phase")]
    public GameObject endPhaseRoom;

    [Header("Jerry Prefab")]
    public GameObject jerryPrefab;

    

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    // internal variables
    GameObject jerryGO; //VERY CRUCIAL FOR THIS GAME
    Transform jerrySpawnPoint;
    public bool endPhase = false;
    
    // Start is called before the first frame update
    void Start()
    {
        //PlayerBehaviour.instance.AgePlayer();
        //PlayerBehaviour.instance.AgePlayer();
        //PlayerBehaviour.instance.AgePlayer();
        //EndPhase();
    }

    void SpawnJerry()
    {
        if (jerryGO != null) Destroy(jerryGO);

        if (currentRoomType != Room.Adult)
        {
            jerrySpawnPoint = FindAnyObjectByType<JerrySpawnPoint>().transform;
            jerryGO = Instantiate(jerryPrefab, jerrySpawnPoint.position, jerryPrefab.transform.rotation);
        }
    }

    public void UpdateRoom()
    {
        foreach (var room in rooms)
        {
            room.SetActive(false);
        }
        rooms[(int)currentRoomType].SetActive(true);

        if(currentRoomType == Room.Adult)
        {
            BoxManager.instance.Setup();
        }

        SpawnJerry();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AgeRoom()
    {
        if (currentRoomType + 1 < Room.TOTAL)
        {
            currentRoomType += 1;
            UpdateRoom();
        }
        else Debug.LogError("Some issue with room enum and room type");
    }

    public void ActivateEndPhase()
    {
        endPhase = true;
        Door.instance.endPhaseOpen = true;
        foreach (var room in rooms)
        {
            room.SetActive(false);
        }
        currentRoomType = Room.TOTAL;
        endPhaseRoom.SetActive(true);

        SpawnJerry();
    }

    public void EndPhase()
    {
        GameManager.instance.StopGlitching();

        AudioManager.instance.PlaySound(AudioManager.SoundType.HEARTBEAT_1);

        var pos = endPhaseRoom.transform.GetChild(0).position;
        pos.y = PlayerBehaviour.instance.transform.position.y;
        PlayerBehaviour.instance.transform.SetPositionAndRotation(pos, endPhaseRoom.transform.GetChild(0).rotation);
        PlayerBehaviour.instance.transform.GetChild(0).gameObject.SetActive(true);
        PuzzleManager.instance.FreezePlayer();
        PlayerBehaviour.instance.ExtendPlayerHand();
        MouseLookAround.instance.lookAllowed = false;

        var camPos1 = endPhaseRoom.transform.GetChild(1);
        var camPos2 = endPhaseRoom.transform.GetChild(2);
        var camPos3 = endPhaseRoom.transform.GetChild(3);

        MouseLookAround.instance.EndPhaseMouseTransition(camPos1, camPos2, camPos3, .5f);
    }
}
