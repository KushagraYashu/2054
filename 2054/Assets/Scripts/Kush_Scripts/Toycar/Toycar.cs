using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Toycar : MonoBehaviour
{
    [Header("Mesh Reference")]
    public Transform toycarTrans;

    [Header("Waypoints and Variables")]
    [SerializeField] private float distThreshold = 0.2f;
    [SerializeField] private float speed = 2f;
    public Transform curWaypoint;
    [SerializeField] private List<Transform> waypoints = new(); //no need to change this, it will be set up automatically in Start()

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
        if (other.gameObject.CompareTag("Player"))
        {
            canPlay = true;
            isPlaying = false;
        }
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
    }

    private void Update()
    {
        if (canPlay && !finished)
        {
            if (Input.GetKey(KeyCode.E))
            {
                isPlaying = true;
                StartCoroutine(nameof(MoveToycar));
                if (PlayerBehaviour.instance.currentPlayerState != PlayerBehaviour.PlayerState.PLAYING_WITH_CAR)
                    PlayerBehaviour.instance.currentPlayerState = PlayerBehaviour.PlayerState.PLAYING_WITH_CAR;

            }

            if(Input.GetKeyUp(KeyCode.E) && isPlaying)
            {
                isPlaying = false;
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
                Vector3 direction = curWaypoint.position - toycarTrans.position;
                direction.y = 0;
                if (direction != Vector3.zero)
                {
                    toycarTrans.rotation = Quaternion.LookRotation(direction);
                }
                distance = Vector3.Distance(toycarTrans.position, curWaypoint.position);
                StartCoroutine(nameof(UpdatePosition));

                if (distance <= distThreshold)
                {
                    i++;
                    if(i >= waypoints.Count)
                    {
                        SolveState();
                        curWaypoint = null;
                    }
                    else
                    {
                        curWaypoint = waypoints[i];
                    }
                }

                yield return null;
            }
        }
    }

    void SolveState()
    {
        canPlay = false;
        isPlaying = false;
        finished = true;
        GetComponent<BoxCollider>().enabled = false;
        PlayerBehaviour.instance.currentPlayerState = PlayerBehaviour.PlayerState.EXPLORING;
        toycarTrans.GetComponent<Rigidbody>().isKinematic = false;

        //Do memory stuff

        //remove this line later
        GetComponentInChildren<Canvas>().GetComponentInChildren<TextMeshProUGUI>().text += "\nShow Memory";
    }

    IEnumerator UpdatePosition()
    {
        while (distance > distThreshold && isPlaying)
        {
            var position = curWaypoint.position;
            position.y = toycarTrans.position.y;
            toycarTrans.position = Vector3.MoveTowards(toycarTrans.position, position, (speed/10) * Time.deltaTime);

            yield return null;
            //yield return new WaitForSeconds(.5f);
        }
    }
}
