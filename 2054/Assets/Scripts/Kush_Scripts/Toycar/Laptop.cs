using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Laptop : MonoBehaviour
{
    public static Laptop instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    [Header("Bools")]
    [SerializeField] bool canEnterCode = false;
    [SerializeField] bool enterCode = false;

    [Header("UI Things")]
    [SerializeField] GameObject keyboardParent;
    [SerializeField] GameObject inputParent;
    [SerializeField] TextMeshProUGUI[] passcodeTxtFields;

    //internal variables
    string passcode;
    int curIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canEnterCode)
        {
            MouseLookAround.instance.SetMouseLock(false);
            enterCode = true;

            PlayerBehaviour.instance.currentPlayerState = PlayerBehaviour.PlayerState.USING_LAPTOP;

            MouseLookAround.instance.lookAllowed = false;

            keyboardParent.SetActive(true);
            inputParent.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && canEnterCode)
        {
            MouseLookAround.instance.SetMouseLock(false);
            enterCode = false;
        }
    }

    public void SetLaptop(bool b, string passcode = "")
    {
        canEnterCode = b;
        if (b)
        {
            this.passcode = passcode;
        }
    }

    public void CheckPasscode()
    {
        if (enterCode)
        {
            if (passcodeTxtFields.Length == passcode.Length)
            {
                string code = "";
                foreach (TextMeshProUGUI txtField in passcodeTxtFields)
                {
                    code += txtField.text;
                }

                if ((code == passcode))
                {
                    Debug.Log("Success Memory");

                    StartCoroutine(Success(true));
                }
                else
                {
                    Debug.Log("Try again");

                    StartCoroutine(Success(false));
                }
            }
            else
                Debug.Log("Something's wrong with textfield or passcode length");
        }
        else
            return;
    }

    IEnumerator Success(bool b)
    {
        if (b)
        {
            foreach(TextMeshProUGUI txt in passcodeTxtFields)
            {
                var img = txt.transform.GetChild(0).GetComponent<Image>();
                img.gameObject.SetActive(true);
                img.color = new Color(0, 1, 0, 0.5f);
            }

            yield return new WaitForSeconds(0.5f);

            foreach (TextMeshProUGUI txt in passcodeTxtFields)
            {
                var img = txt.transform.GetChild(0).GetComponent<Image>();
                img.gameObject.SetActive(false);
                img.color = new Color(0, 0, 0, 0.0f);
            }

            PlayerBehaviour.instance.currentPlayerState = PlayerBehaviour.PlayerState.EXPLORING;

            MouseLookAround.instance.SetMouseLock(true);
            MouseLookAround.instance.lookAllowed = true;

            inputParent.SetActive(false);
            keyboardParent.SetActive(false);

            UIManager.instance.SetHelperText();

            //freeze player
            GameManager.instance.StopGlitching();
            PuzzleManager.instance.FreezePlayer();

            StartCoroutine(UIEffects.instance.Fade(0, 1, 2));
            yield return new WaitForSeconds(2f);

            //play memory animation here
            UIManager.instance.ShowMemory(UIManager.MemoryType.TEEN_LAPTOP, out float waitTime);
            yield return new WaitForSeconds(waitTime);

            StartCoroutine(UIEffects.instance.ScrollYear(2001, 2018, 0.3f, PlayerBehaviour.instance.AgePlayer));

            this.enabled = false;
        }
        else
        {
            foreach (TextMeshProUGUI txt in passcodeTxtFields)
            {
                var img = txt.transform.GetChild(0).GetComponent<Image>();
                img.gameObject.SetActive(true);
                img.color = new Color(1, 0, 0, 0.5f);
            }

            yield return new WaitForSeconds(0.5f);

            foreach (TextMeshProUGUI txt in passcodeTxtFields)
            {
                var img = txt.transform.GetChild(0).GetComponent<Image>();
                img.gameObject.SetActive(false);
                img.color = new Color(0, 0, 0, 0.0f);
                txt.text = "";
                curIndex = 0;
            }
        }
    }

    public void EnterCode(string code)
    {
        AudioManager.instance.PlaySound(AudioManager.SoundType.KEYBOARD);
        if (enterCode) { passcodeTxtFields[curIndex % 5].text = code; curIndex++; }
        else return;
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
