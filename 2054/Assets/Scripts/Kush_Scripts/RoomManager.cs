using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;

    [Header("Rooms (Mesh Parents)")]
    [SerializeField] private List<GameObject> rooms = new();

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

    
    // Start is called before the first frame update
    void Start()
    {
        rooms[(int)currentRoomType].SetActive(true);

        jerryGO = GameObject.FindGameObjectWithTag("Jerry");
        if (jerryGO == null)
            SpawnJerry();
    }

    void SpawnJerry()
    {
        if(jerryGO != null) Destroy(jerryGO);

        jerrySpawnPoint = FindAnyObjectByType<JerrySpawnPoint>().transform;
        jerryGO = Instantiate(jerryPrefab, jerrySpawnPoint.position, jerryPrefab.transform.rotation);
    }

    void UpdateRoom()
    {
        foreach (var room in rooms)
        {
            room.SetActive(false);
        }
        rooms[(int)currentRoomType].SetActive(true);

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
}
