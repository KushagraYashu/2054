using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;

    [Header("Jerry Prefab")]
    public GameObject jerryPrefab;

    [Header("Rooms (Mesh Parents)")]
    [SerializeField] private List<GameObject> rooms = new();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    // internal variables
    GameObject jerryGO; //VERY CRUCIAL FOR THIS GAME

    
    // Start is called before the first frame update
    void Start()
    {
        jerryGO = GameObject.FindGameObjectWithTag("Jerry");
        if (jerryGO == null)
            SpawnJerry();
    }

    void SpawnJerry()
    {
        if(jerryGO != null) Destroy(jerryGO);

        var spawnPoint = FindAnyObjectByType<JerrySpawnPoint>().transform;
        jerryGO = Instantiate(jerryPrefab, spawnPoint.position, jerryPrefab.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
