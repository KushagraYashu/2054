using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Toycar : MonoBehaviour
{
    public static Toycar instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public string[] PossibleCodes = { 
        "!",
        "\"",
        "#",
        "%",
        "^",
        "&",
        "*",
        "(",
        ")",
        "-",
        "=",
        "+",
        "{",
        "}",
        "[",
        "]",
        "@",
        ";",
        ":",
        "<",
        ">",
        "?",
        "/",
        "|",
        "~",
        "¦",
    };

    [Header("Mesh Reference")]
    public Transform toycarMeshTrans;
    public Transform toycarTargetPtTrans;

    [Header("Waypoints and Variables")]
    [SerializeField] private float distThreshold = 0.2f;
    [SerializeField] private float speed = 2f;
    public Transform curWaypoint;
    [SerializeField] private List<Transform> waypoints = new(); //no need to change this, it will be set up automatically in Start()

    [Header("Passcode")]
    public string Passcode { get; private set; } = "";

    [Header("Bools")]
    public bool canPlay = false;
    public bool isPlaying = false;

    //internal variables
    float distance = 0f;
    int i = 0;
    bool moving = false;
    bool finished = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && toycarMeshTrans.GetComponent<ToycarMesh>().canPlay)
        {
            canPlay = true;
            isPlaying = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && toycarMeshTrans.GetComponent<ToycarMesh>().canPlay)
        {
            canPlay = false;
            isPlaying = false;
        }
    }

    public void SetPlay()
    {
        toycarMeshTrans.GetComponent<Rigidbody>().isKinematic = true;
        canPlay = true;
        UIManager.instance.SetKeyToPress(UIManager.KeyType.E);
        UIManager.instance.SetHelperText();
    }

    private void Start()
    {
        if (curWaypoint != null)
        {
            waypoints.Add(curWaypoint);
            var next = curWaypoint.GetComponent<ToycarWaypoint>().nextWaypoint;
            if (next != null)
                do
                {
                    waypoints.Add(next);
                    next = next.GetComponent<ToycarWaypoint>().nextWaypoint;
                } 
                while (next != null);
        }

        StartCoroutine(ShowToycarImg());
    }

    IEnumerator ShowToycarImg()
    {
        yield return new WaitForSeconds(5f);
        UIManager.instance.SetHelperText("ToycarImg");
    }

    private void Update()
    {
        if (canPlay && !finished)
        {
            if (Input.GetKey(KeyCode.E))
            {
                isPlaying = true;
                //toycarMeshTrans.GetComponent<Rigidbody>().isKinematic = true;
                StartCoroutine(nameof(MoveToycar));
                if (PlayerBehaviour.instance.currentPlayerState != PlayerBehaviour.PlayerState.PLAYING_WITH_CAR)
                    PlayerBehaviour.instance.currentPlayerState = PlayerBehaviour.PlayerState.PLAYING_WITH_CAR;

            }

            if(Input.GetKeyUp(KeyCode.E) && isPlaying)
            {
                isPlaying = false;
                //toycarMeshTrans.GetComponent<Rigidbody>().isKinematic = false;
                PlayerBehaviour.instance.currentPlayerState = PlayerBehaviour.PlayerState.EXPLORING;
            }
        }
    }

    IEnumerator MoveToycar()
    {
        if (!moving)
        {
            moving = true;
            while (curWaypoint != null)
            {
                Vector3 direction = curWaypoint.position - toycarMeshTrans.position;
                direction.y = 0;
                if (direction != Vector3.zero)
                {
                    toycarMeshTrans.rotation = Quaternion.LookRotation(direction);
                }
                distance = Vector3.Distance(toycarMeshTrans.position, curWaypoint.position);
                StartCoroutine(nameof(UpdatePosition));

                if (distance <= distThreshold)
                {
                    i++;
                    if(i >= waypoints.Count)
                    {
                        if(i == waypoints.Count) 
                        {
                            Passcode += curWaypoint.GetComponent<ToycarWaypoint>().waypointCode;
                            curWaypoint.GetComponent<ToycarWaypoint>().SpawnCode();
                            UIManager.instance.SetHelperText(curWaypoint.GetComponent<ToycarWaypoint>().waypointCode + " ", true);
                        }
                        SolveState();
                        curWaypoint = null;
                    }
                    else
                    {
                        Passcode += curWaypoint.GetComponent<ToycarWaypoint>().waypointCode;
                        curWaypoint.GetComponent<ToycarWaypoint>().SpawnCode();
                        UIManager.instance.SetHelperText(curWaypoint.GetComponent<ToycarWaypoint>().waypointCode + " ", true);
                        curWaypoint = waypoints[i];
                    }
                }

                yield return null;
            }
        }
    }

    void SolveState()
    {
        Laptop.instance.SetLaptop(true, Passcode);

        canPlay = false;
        isPlaying = false;
        finished = true;
        UIManager.instance.SetKeyToPress();
        GetComponent<BoxCollider>().enabled = false;
        PlayerBehaviour.instance.currentPlayerState = PlayerBehaviour.PlayerState.EXPLORING;
        //toycarMeshTrans.GetComponent<Rigidbody>().isKinematic = false;

        //Do memory stuff

        //remove this line later
        //GetComponentInChildren<Canvas>().GetComponentInChildren<TextMeshProUGUI>().text += "\nShow Memory";
    }

    IEnumerator UpdatePosition()
    {
        while (distance > distThreshold && isPlaying)
        {
            var position = curWaypoint.position;
            position.y = toycarMeshTrans.position.y;
            toycarMeshTrans.position = Vector3.MoveTowards(toycarMeshTrans.position, position, (speed/100) * Time.deltaTime);

            yield return null;
            //yield return new WaitForSeconds(.5f);
        }
    }
}
