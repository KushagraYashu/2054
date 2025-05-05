using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class BoxManager : MonoBehaviour
{
    public static BoxManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    public enum ItemType
    {
        OTHER,
        JERRY,
        WEDDING_PHOTO,
        VOLCANO_MODEL,
        GRADUATION_SCROLL,
        TOTAL
    };

    [Header("Items Needed")]
    public List<ItemType> itemsNeeded = new();
    public List<ItemType> itemsCollected = new();

    [Header("Item Prefabs")]
    public List<GameObject> itemPrefabs = new List<GameObject>((int)ItemType.TOTAL);

    [Header("Item throw force")]
    public Vector3 throwForceMin = new Vector3(2, 2, 2);
    public Vector3 throwForceMax = new Vector3(4, 4, 4);

    [Header("Item target points")]
    public Transform[] targetPoints;

    [Header("Boxes")]
    public List<Box> boxes = new();

    [Header("Waypoints")]
    public List<Transform> FromAdultToEnd = new();

    public bool VolcanoSuccess { get; set; } = false;

    //internal variables
    int totMemPlayed = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Setup()
    {

        itemsNeeded = new List<ItemType>(4)
        {
            ItemType.JERRY,
            ItemType.WEDDING_PHOTO,
            ItemType.VOLCANO_MODEL,
            ItemType.GRADUATION_SCROLL
        };
        itemsCollected = new List<ItemType>(itemsNeeded.Count);

        Shuffle<ItemType>(itemsNeeded);

        boxes = FindObjectsByType<Box>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID).ToList<Box>();

        foreach (Box box in boxes)
        {
            PopulateBox(box);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F))
        //{
        //    GuidanceSystem.instance.StartSteps(FromAdultToEnd);
        //    RoomManager.instance.ActivateEndPhase();
        //    Door.instance.canOpen = true;
        //    this.enabled = false;
        //}
    }

    public void PlayMemory(ItemType item, GameObject gO = null)
    {
        UIManager.instance.SetHelperText();
        UIManager.instance.SetKeyToPress();

        GameManager.instance.StopGlitching();

        switch (item)
        {
            case ItemType.JERRY:
                StartCoroutine(JerryMemory(gO));
                break;

            case ItemType.WEDDING_PHOTO:
                StartCoroutine(WeddingMemory());
                break;

            case ItemType.VOLCANO_MODEL:
                StartCoroutine(VolcanoMemory());
                break;

            case ItemType.GRADUATION_SCROLL:
                StartCoroutine(GraduationMemory());
                break;

            default: 
                Debug.LogError("Item type is wrong");
                break;
        }
    }

    IEnumerator GraduationMemory()
    {
        //freeze player
        GameManager.instance.StopGlitching();
        PuzzleManager.instance.FreezePlayer();

        StartCoroutine(UIEffects.instance.Fade(0, 1, 2));
        yield return new WaitForSeconds(2f);

        //play memory animation here
        UIManager.instance.ShowMemory(UIManager.MemoryType.ADULT_GRADUATION, out float waitTime);
        yield return new WaitForSeconds(waitTime);

        //fade out
        StartCoroutine(UIEffects.instance.Fade(1, 0, 2));
        yield return new WaitForSeconds(2f);

        //unfreeze player
        PuzzleManager.instance.UnfreezePlayer();

        GameManager.instance.StartGlitching();

        totMemPlayed++;
        if (totMemPlayed >= 4)
        {
            //GuidanceSystem.instance.StartSteps(FromAdultToEnd);
            GameManager.instance.TurnOffLightsExceptBed();
            RoomManager.instance.ActivateEndPhase();
            Door.instance.canOpen = true;
            this.enabled = false;
        }
    }

    IEnumerator VolcanoMemory()
    {
        //freeze player
        GameManager.instance.StopGlitching();
        PuzzleManager.instance.FreezePlayer();

        //Fade in
        StartCoroutine(UIEffects.instance.Fade(0, 1, 2));
        yield return new WaitForSeconds(2f);

        //play memory
        float waitTime;
        //we planned on creating 2 states for volcano, failure and success, but due to time contraints we only made the success one.
        UIManager.instance.ShowMemory(UIManager.MemoryType.CHILD_VOLCANO_SUCCESS, out waitTime);
        yield return new WaitForSeconds(waitTime);

        //fade out
        StartCoroutine(UIEffects.instance.Fade(1, 0, 2));
        yield return new WaitForSeconds(2f);

        //unfreeze player
        PuzzleManager.instance.UnfreezePlayer();

        GameManager.instance.StartGlitching();

        totMemPlayed++;
        if (totMemPlayed >= 4)
        {
            //GuidanceSystem.instance.StartSteps(FromAdultToEnd);
            GameManager.instance.TurnOffLightsExceptBed();
            Door.instance.canOpen = true;
            RoomManager.instance.ActivateEndPhase();
            this.enabled = false;
        }
    }

    IEnumerator WeddingMemory()
    {
        //freeze player
        GameManager.instance.StopGlitching();
        PuzzleManager.instance.FreezePlayer();

        StartCoroutine(UIEffects.instance.Fade(0, 1, 2));
        yield return new WaitForSeconds(2f);

        //play memory animation here
        UIManager.instance.ShowMemory(UIManager.MemoryType.ADULT_WEDDING, out float waitTime);
        yield return new WaitForSeconds(waitTime);

        //fade out
        StartCoroutine(UIEffects.instance.Fade(1, 0, 2));
        yield return new WaitForSeconds(2f);

        //unfreeze player
        PuzzleManager.instance.UnfreezePlayer();

        GameManager.instance.StartGlitching();

        totMemPlayed++;
        if(totMemPlayed >= 4)
        {
            //GuidanceSystem.instance.StartSteps(FromAdultToEnd);
            GameManager.instance.TurnOffLightsExceptBed();
            RoomManager.instance.ActivateEndPhase();
            Door.instance.canOpen = true;
            this.enabled = false;
        }
    }

    IEnumerator JerryMemory(GameObject gO)
    {
        //freeze player
        GameManager.instance.StopGlitching();
        PuzzleManager.instance.FreezePlayer();

        StartCoroutine(UIEffects.instance.Fade(0, 1, 2));
        yield return new WaitForSeconds(2f);

        //play memory animation here
        UIManager.instance.ShowMemory(UIManager.MemoryType.ADULT_JERRY, out float waitTime);
        yield return new WaitForSeconds(waitTime);

        //fade out
        StartCoroutine(UIEffects.instance.Fade(1, 0, 2));
        yield return new WaitForSeconds(2f);

        Debug.Log("Unfreeze now");
        //unfreeze player
        PuzzleManager.instance.UnfreezePlayer();

        GameManager.instance.StartGlitching();

        gO.transform.GetChild(0).GetComponent<AudioSource>().enabled = true;

        totMemPlayed++;
        if (totMemPlayed >= 4)
        {
            //GuidanceSystem.instance.StartSteps(FromAdultToEnd);
            GameManager.instance.TurnOffLightsExceptBed();
            RoomManager.instance.ActivateEndPhase();
            Door.instance.canOpen = true;
            this.enabled = false;
        }
    }

    public bool AddItem(ItemType item)
    {
        if (itemsNeeded.Contains(item))
        {
            if (!itemsCollected.Contains(item))
            {
                itemsCollected.Add(item);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    int iN = 0;
    int iA = 0;
    public void PopulateBox(Box box)
    {
        box.items = new List<Item>(box.totItems);
        for (int i = 0; i < box.totItems; i++)
        {
            if(i == 0 && iA < itemPrefabs.Count - 1)
            {
                Mathf.Clamp(iN, 0, itemPrefabs.Count - 2);
                box.AddItemToBox(itemPrefabs[iN + 1], targetPoints[iN].transform);
                iA++;
            }
            else
                box.AddItemToBox(itemPrefabs[0]);
        }
        iN++;
    }

    public static void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
