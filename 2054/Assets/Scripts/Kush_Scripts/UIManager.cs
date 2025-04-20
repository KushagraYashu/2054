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

    public void SetKeyToPress(string key = "", bool append = false)
    {
        if (!append) keyToPress.text = key;
        else keyToPress.text += key;
    }

    public void SetHelperText(string text = "", bool append = false)
    {
        if (!append) helperTxt.text = text;
        else helperTxt.text += text;
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
