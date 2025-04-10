using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager instance;
    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(this);
    }

    public enum PuzzleType
    {
        JIGSAW,
        _2054,
        TOTAL
    };

    [Header("Prefabs and Gameobjects")]
    [SerializeField] private GameObject[] puzzlePrefabs;
    [SerializeField] private GameObject curPuzzleParent;
    [SerializeField] private Transform puzzleSpawnPoint;

    [Header("Puzzle Properties")]
    [SerializeField] private int totalPuzzles = 2;
    [SerializeField] private int curPuzzleID = 1;
    public PuzzleType curPuzzleType;
    [SerializeField] bool puzzleSolved = false;
    [SerializeField] int totalPieces = 3;
    public float solveThreshold = .2f;
    [SerializeField] List<GameObject> piecesGO = new();
    [SerializeField] List<GameObject> piecesWaypoint = new();
    [SerializeField] List<GameObject> piecesTargetPoint = new();
    public List<GameObject> GetTargetPoints() {  return piecesTargetPoint; }

    [Header("Bools")]
    bool canShowInventory = false;
    bool inventory = false;
    bool solving = false;

    [Header("Waypoints")]
    public List<Transform> waypointsFrom2054To = new();


    // internal variables
    bool dragginAnObject = false;
    int solved = 0;
    public bool IsDraggingObject() { return dragginAnObject; }
    public void SetDraggingObject(bool b) {  dragginAnObject = b; }

    public bool CheckSolved(bool b = false)
    {
        if (b)
        {
            solved++;
        }

        if (solved >= totalPieces)
        {
            puzzleSolved = true;
            solved = 0;
            return true;
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(RoomManager.instance.currentRoomType == RoomManager.Room.Toddler) SetupPuzzleJigsaw2054();
    }

    void SetupPuzzleJigsaw2054()
    {
        //clearing
        canShowInventory = false;
        inventory = false;
        solving = false;
        piecesWaypoint.Clear();
        piecesTargetPoint.Clear();
        piecesGO.Clear();

        //assigning
        curPuzzleType = (PuzzleType)(curPuzzleID - 1);
        if (curPuzzleID == 1) //Jigsaw Puzzle
        {
            curPuzzleParent = Instantiate(puzzlePrefabs[0], puzzleSpawnPoint);
            curPuzzleParent.transform.parent = GameObject.FindGameObjectWithTag("ToddlerRoomParent").transform;
            totalPieces = 3;
        }
        else if (curPuzzleID == 2) {  //2054 Puzzle
            curPuzzleParent = Instantiate(puzzlePrefabs[1], puzzleSpawnPoint);
            curPuzzleParent.transform.parent = GameObject.FindGameObjectWithTag("ToddlerRoomParent").transform;
            totalPieces = 4;
        }

        var waypoints = FindObjectsByType<PuzzleWaypoints>(FindObjectsSortMode.None);
        foreach (var waypoint in waypoints)
        {
            piecesWaypoint.Add(waypoint.gameObject);
        }
        var targetPoints = FindObjectsByType<PuzzleTargetPoints>(FindObjectsSortMode.None);
        foreach (var targetpoint in targetPoints)
        {
            piecesTargetPoint.Add(targetpoint.gameObject);
            targetpoint.GetComponentInChildren<MeshRenderer>().enabled = false;
        }
    }

    public void AddPiece(GameObject piece)
    {
        piecesGO.Add(piece);
        piece.SetActive(false);
        if(piecesGO.Count == totalPieces)
        {
            canShowInventory = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canShowInventory && Input.GetKeyDown(KeyCode.E)) { 
            ShowInventory();
        }

        if (inventory && Input.GetKeyDown(KeyCode.E) && !solving)
        {
            //stop glitches
            GameManager.instance.StopGlitching();

            if (PlayerBehaviour.instance.currentPlayerState != PlayerBehaviour.PlayerState.SOLVING_PUZZLE)
                PlayerBehaviour.instance.currentPlayerState = PlayerBehaviour.PlayerState.SOLVING_PUZZLE;
            MouseLookAround.instance.SetMouseLock(false);

            SolvePuzzle();
        }

        if (puzzleSolved)
        {
            puzzleSolved = false;

            if (PlayerBehaviour.instance.currentPlayerState != PlayerBehaviour.PlayerState.EXPLORING)
                PlayerBehaviour.instance.currentPlayerState = PlayerBehaviour.PlayerState.EXPLORING;
            MouseLookAround.instance.SetMouseLock();

            //do memory here
            //debug lines
            Canvas puzzleCanvas;
            puzzleCanvas = curPuzzleParent.GetComponentInChildren<Canvas>();
            if (puzzleCanvas)
            {
                puzzleCanvas.GetComponentInChildren<TextMeshProUGUI>().text += "\nShow " +
                    ((PuzzleType)(curPuzzleID - 1)).ToString() + " memory";
            }

            if (curPuzzleID < totalPuzzles) {
                //DO JIGSAW MEMORY HERE

                StartCoroutine(ShowJigsawMemory());
            }
            else
            {
                //THIS IS AFTER 2054 Puzzle (final memory and aging for toddler should go here)

                StartCoroutine(Show2054Memory());
            }
        }

        //DEBUG INPUT, REMOVE LATER
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine (ShowJigsawMemory());
        }
    }

    IEnumerator Show2054Memory()
    {
        //fade in

        //freeze player
        GameManager.instance.StopGlitching();
        FreezePlayer();

        StartCoroutine(UIEffects.instance.Fade(0, 1, 2, "Show 2054 Memory"));
        yield return new WaitForSeconds(2f);

        //play memory animation here
        //yield return new WaitForSeconds(animationDuration);
        //remove this line later (its for testing delay)
        yield return new WaitForSeconds(2f);

        //Scrolling Years (pass the callback function as well)
        StartCoroutine(UIEffects.instance.ScrollYear(1987, 1994, 0.75f, AgePlayer));
    }

    void AgePlayer() {
        //aging and guidance system (guidance should be based on time)
        PlayerBehaviour.instance.AgePlayer();
        GuidanceSystem.instance.StartSteps(waypointsFrom2054To);

        //fade out
        StartCoroutine(UIEffects.instance.Fade(1, 0, 2));

        //unfreeze player
        UnfreezePlayer();

        //allow glitching
        GameManager.instance.StartGlitching();
    }

    IEnumerator ShowJigsawMemory()
    {
        //fade in

        //freeze player
        GameManager.instance.StopGlitching();
        FreezePlayer();

        StartCoroutine(UIEffects.instance.Fade(0, 1, 2, "Show Jigsaw Memory"));
        yield return new WaitForSeconds(2f);

        //play memory animation here
        //yield return new WaitForSeconds(animationDuration);
                        //remove this line later (its for testing delay)
                        yield return new WaitForSeconds(2f);

        

        //switch puzzle now
        SwitchPuzzle();

        //fade out
        StartCoroutine(UIEffects.instance.Fade(1, 0, 2));
        yield return new WaitForSeconds(2f);

        //unfreeze player
        UnfreezePlayer();

        //start glitching
        GameManager.instance.StartGlitching();
    }

    void SwitchPuzzle()
    {
        curPuzzleID++;

        // add some delay based on memory length
        Destroy(curPuzzleParent);

        StartCoroutine(DelayedSpawn());

    }

    IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(2f);
        SetupPuzzleJigsaw2054();
    }

    void SolvePuzzle()
    {
        solving = true;
        foreach (var piece in piecesGO) {
            piece.GetComponent<PuzzlePiece>().issolving = true;
        }
    }

    void ShowInventory()
    {
        int i = 0;
        
        foreach (var piece in piecesGO) {
            if (piece != null && 
                piecesWaypoint[i % piecesWaypoint.Count] != null && 
                !piece.GetComponent<PuzzlePiece>().solved) {

                piece.transform.position = piecesWaypoint[i % piecesWaypoint.Count].transform.position;
                i++;
                piece.SetActive(true);

                foreach (GameObject t in piecesTargetPoint)
                {
                    if (t.name.Contains(piece.GetComponent<PuzzlePiece>().targetPointName))
                    {   
                        t.GetComponentInChildren<MeshRenderer>().enabled = true;
                        break;
                    }
                }
            }
        }
        
        inventory = true;
    }

    /// <summary>
    ///freeze player for either memory or aging, this function uses PlayerState.TOTAL (also freezes mouse)
    /// </summary>
    public void FreezePlayer()
    {
        PlayerBehaviour.instance.currentPlayerState = PlayerBehaviour.PlayerState.TOTAL; //TOTAL makes player stop moving, hence applied total
        MouseLookAround.instance.lookAllowed = false;
    }

    /// <summary>
    /// unfreeze player for exploring, (also unfreezes mouse)
    /// </summary>
    public void UnfreezePlayer()
    {
        PlayerBehaviour.instance.currentPlayerState = PlayerBehaviour.PlayerState.EXPLORING;
        MouseLookAround.instance.lookAllowed = true;
    }
}
