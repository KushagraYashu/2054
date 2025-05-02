using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class UISpriteAnimation : MonoBehaviour
{
    [Header("Key Animation Sprites")]
    public List<Texture> RKeySprites = new();
    public List<Texture> QKeySprites = new();
    public List<Texture> EKeySprites = new();
    public List<Texture> MouseKeySprites = new();

    [Header("Help Animation Sprites")]
    public List<Texture> inspectSprites = new();
    public List<Texture> moveCloseFarSprites = new();
    public List<Texture> moveUpDownSprites = new();

    public float frameDelay = .1f;

    //internal variables
    RawImage rawImg;
    bool insp = false, moveCloseFar = false, moveUpDown = false;
    bool EKey = false, RKey = false, QKey = false, MouseKey = false;

    public void SetAllBools(bool b) {
        insp = b;
        moveCloseFar = b;
        moveUpDown = b;
        EKey = b;
        RKey = b;
        QKey = b;
        MouseKey = b;
    }

    // Start is called before the first frame update
    void Start()
    {
        rawImg = GetComponent<RawImage>();

        UIManager.instance.SetHelperText();
        SetAllBools(false);
    }

    public void StartHelpAnimation(UIManager.HelpType help)
    {
        switch (help)
        {
            case UIManager.HelpType.INSPECT:
                if (!insp) { 
                    insp = true;
                    StartCoroutine(KeyAnim(inspectSprites));
                }
                break;
            case UIManager.HelpType.MOVE_UP_DOWN:
                if(!moveUpDown){
                    moveUpDown = true;
                    StartCoroutine(KeyAnim(moveUpDownSprites));
                }
                break;
            case UIManager.HelpType.MOVE_CLOSE_FAR:
                if(!moveCloseFar){
                    moveCloseFar = true;
                    StartCoroutine(KeyAnim(moveCloseFarSprites));
                    
                }
                break;
            default:
                Debug.LogError("Help animation not found");
                break;
        }
    }

    public void StartKeyAnimation(UIManager.KeyType key)
    {
        switch (key)
        {
            case UIManager.KeyType.E:
                if(!EKey){
                    EKey = true;
                    StartCoroutine(KeyAnim(EKeySprites));
                }
                break;

            case UIManager.KeyType.R:
                if(!RKey){ 
                    RKey = true;
                    StartCoroutine(KeyAnim(RKeySprites));
                }
                break;

            case UIManager.KeyType.Q:
                if(!QKey){ 
                    QKey = true;
                    StartCoroutine(KeyAnim(QKeySprites));
                }
                break;

            case UIManager.KeyType.MOUSE:
                if(!MouseKey){ 
                    MouseKey = true;
                    StartCoroutine(KeyAnim(MouseKeySprites));
                }
                break;

            default:
                Debug.LogError("Key animation not found");
                break;
        }
    }

    IEnumerator KeyAnim(List<Texture> tex)
    {
        //UIManager.instance.SetHelperText();

        int curFrame = 0;
        while (true)
        {
            curFrame = (curFrame + 1) % tex.Count;
            rawImg.texture = tex[curFrame];
            yield return new WaitForSeconds(frameDelay);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
