using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCollider : MonoBehaviour
{
    public enum Type
    {
        RESET,
        SUCCESS
    };

    public Type type;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            if(type == Type.RESET)
            {
                StartCoroutine(TutorialReset());
            }
            else
            {
                AudioManager.instance.PlaySound(AudioManager.SoundType.LIGHTSWITCH);
                GameManager.instance.TutorialDone();

                GameManager.instance.StartGlitching();
            }
        }
    }

    IEnumerator TutorialReset()
    {
        PuzzleManager.instance.FreezePlayer();
        StartCoroutine(
        UIEffects.instance.Fade(0, 1, 1f));
        yield return new WaitForSeconds(1f);
        GameManager.instance.TutorialReset();
    }
}
