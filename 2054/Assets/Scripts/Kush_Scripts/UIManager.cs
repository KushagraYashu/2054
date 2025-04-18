using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    [Header("UI Guide Objs")]
    [SerializeField] TextMeshProUGUI keyToPress;
    [SerializeField] TextMeshProUGUI helperTxt;

    public void SetKeyToPress(string key = "")
    {
        keyToPress.text = key;
    }

    public void SetHelperText(string text = "")
    {
        helperTxt.text = text;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
