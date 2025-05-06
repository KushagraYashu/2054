using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public int totItems;
    public List<Item> items;
    public Transform spawnPoint;
    public Transform throwPoint;
    public Transform takePoint;
    public List<Item> itemsThrown = new();


    //internal variables
    int curIndex = 0;
    bool opened = false;
    bool done = false;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && itemsThrown.Count < totItems && !opened)
        {
            opened = true;
            UIManager.instance.SetKeyToPress(UIManager.KeyType.E);
            GameManager.instance.StopGlitching();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            opened = false;
            UIManager.instance.SetKeyToPress();
            GameManager.instance.StartGlitching();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        throwPoint = transform.GetChild(0);
        takePoint = transform.GetChild(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (opened && !done)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ThrowItems();
            }
        }
        if (done)
        {
            UIManager.instance.SetKeyToPress();
            this.enabled = false;
        }
    }

    void ThrowItems()
    {
        if(itemsThrown.Count < totItems)
        {
            //items[curIndex].gameObject.GetComponent<MeshRenderer>().enabled = true;
            items[curIndex].transform.GetChild(0).gameObject.SetActive(true);
            items[curIndex].Throw(throwPoint, takePoint);
            itemsThrown.Add(items[curIndex]);
            curIndex++;
        }
        else
        {
            done = true;
        }
    }

    public bool Contains(BoxManager.ItemType itemType)
    {
        foreach(Item item in items)
        {
            if (item.itemType == itemType) return true;
        }

        return false;
    }

    public void AddItemToBox(GameObject itemPrefab, Transform targetPt = null)
    {
        Item item = Instantiate(itemPrefab, spawnPoint.position, itemPrefab.transform.rotation, this.gameObject.transform).GetComponent<Item>();
        item.GetComponent<Rigidbody>().isKinematic = true;
        //item.gameObject.GetComponent<MeshRenderer>().enabled = false;
        if(targetPt != null)
        {
            item.targetPoint = targetPt;
        }
        Transform child = null;
        try
        {
            child = item.transform.GetChild(0);
        }
        catch { }
        if ((child != null))
        {
            child.gameObject.SetActive(false);
        }
        items.Add(item);
        BoxManager.Shuffle<Item>(items);
    }
}
