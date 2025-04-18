using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEffects : MonoBehaviour
{
    public static UIEffects instance;

    Image fadeImage;

    public TextMeshProUGUI[] yearDigits;
    public GameObject yearTextParent;

    //internal variables
    TextMeshProUGUI childTMPText;
    int currentYear;
    float scrollSpeed = 2f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        fadeImage = GetComponent<Image>();

        childTMPText = fadeImage.gameObject.GetComponentInChildren<TextMeshProUGUI>();

        //remove later
        //StartCoroutine(ScrollYear(1980, 2001, .5f));
    }

    public IEnumerator Fade(float startAlpha, float endAlpha, float fadeDuration, string text = "")
    {
        
        childTMPText.text = text;

        Color textColor = childTMPText.color;
        textColor.a = startAlpha;
        childTMPText.color = textColor;
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            textColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeImage.color = color;
            childTMPText.color = textColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color.a = endAlpha;
        textColor.a = endAlpha;
        fadeImage.color = color;
        childTMPText.color = textColor;
    }

    public void SetYear(int year)
    {
        currentYear = year;
        string yearStr = year.ToString();
        for (int i = 0; i < 4; i++)
        {
            yearDigits[i].text = yearStr[i].ToString();
        }
    }

    /// <summary>
    /// Scrolls the year from curYear to targetYear over a specified parameter.
    /// </summary>
    /// <param name="curYear"> Year to Start From </param>
    /// <param name="targetYear"> Target Year </param>
    /// <param name="par"> Parameter, lower is faster</param>
    /// <param name="onComplete"> Callback Function </param>
    /// <returns>Nothing</returns>
    public IEnumerator ScrollYear(int curYear, int targetYear, float par, Action onComplete = null)
    {
        yearTextParent.SetActive(true);

        currentYear = curYear;
        scrollSpeed = par;
        int startYear = curYear;
        int endYear = targetYear;

        for (int i = 0; i < yearDigits.Length; i++)
        {
            yearDigits[i].text = startYear.ToString()[i].ToString();
        }

        for (int i = startYear; i < endYear; i++)
        {
            yield return StartCoroutine(AnimateYearChange(i, i + 1));
        }

        if(onComplete != null)
        {
            yield return new WaitForSeconds(0.5f);
            onComplete.Invoke();
        }

        yearTextParent.SetActive(false);
    }

    private IEnumerator AnimateYearChange(int fromYear, int toYear)
    {
        string fromStr = fromYear.ToString();
        string toStr = toYear.ToString();

        // Create a list of coroutines to run in parallel
        List<Coroutine> coroutines = new List<Coroutine>();

        for (int i = 3; i >= 0; i--)
        {
            int fromDigit = int.Parse(fromStr[i].ToString());
            int toDigit = int.Parse(toStr[i].ToString());

            if (fromDigit != toDigit)
            {
                float influence = Mathf.Pow(0.1f, (3 - i)); // Rightmost = 1.0, then 0.1, 0.01, 0.001
                coroutines.Add(StartCoroutine(ScrollDigit(yearDigits[i], fromDigit, toDigit, influence)));
            }
        }

        // Wait for all digit scrolls to complete
        foreach (var c in coroutines)
            yield return c;

        currentYear = toYear;
    }

    private IEnumerator ScrollDigit(TextMeshProUGUI digitText, int from, int to, float influence)
    {
        float elapsedTime = 0f;
        float moveDistance = 40f;

        Vector3 originalPos = digitText.transform.localPosition;
        Vector3 upperPos = originalPos + Vector3.up * moveDistance;
        Vector3 resetPos = originalPos - Vector3.up * moveDistance;

        // First half: scroll up and fade out
        while (elapsedTime < scrollSpeed)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / scrollSpeed;

            // Odometer influence causes the digit to scroll slightly, even if it's not changing
            float influencedT = Mathf.Min(t * (1f + influence), 1f);

            digitText.transform.localPosition = Vector3.Lerp(originalPos, upperPos, influencedT);
            yield return null;
        }

        digitText.transform.localPosition = resetPos;
        digitText.text = to.ToString();

        // Reset time for the second half
        elapsedTime = 0f;
        while (elapsedTime < scrollSpeed)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / scrollSpeed;
            digitText.transform.localPosition = Vector3.Lerp(resetPos, originalPos, t);
            yield return null;
        }

        digitText.transform.localPosition = originalPos;
    }
}
