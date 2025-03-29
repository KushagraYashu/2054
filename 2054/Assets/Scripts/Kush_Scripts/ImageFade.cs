using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageFade : MonoBehaviour
{
    public static ImageFade instance;


    Image fadeImage;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        fadeImage = GetComponent<Image>();
    }

    public IEnumerator Fade(float startAlpha, float endAlpha, float fadeDuration, string text = "")
    {
        var childTMPText = fadeImage.gameObject.GetComponentInChildren<TextMeshProUGUI>();
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
}
