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
        SetupPuzzle();
    }

    void SetupPuzzle()
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
            curPuzzleParent = Instantiate(puzzlePrefabs[0], puzzleSpawnPoint.position, puzzlePrefabs[0].transform.rotation);
            totalPieces = 3;
        }
        else if (curPuzzleID == 2) {  //2054 Puzzle
            curPuzzleParent = Instantiate(puzzlePrefabs[1], puzzleSpawnPoint.position, puzzlePrefabs[1].transform.rotation);
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
                SwitchPuzzle();
            }
        }
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
        yield return new WaitForEndOfFrame();
        SetupPuzzle();
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
            if (piece != null && piecesWaypoint[i % piecesWaypoint.Count] != null) {
                piece.transform.position = piecesWaypoint[i % piecesWaypoint.Count].transform.position + new Vector3(0, piece.transform.localScale.y/2, 0);
                i++;
                piece.SetActive(true);
            }
        }
        foreach(var target in piecesTargetPoint)
        {
            if(target)
                target.GetComponentInChildren<MeshRenderer>().enabled = true;
        }
        inventory = true;
    }
}
