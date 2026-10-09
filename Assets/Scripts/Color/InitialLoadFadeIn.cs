using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InitialLoadFadeIn : MonoBehaviour
{
    public Image WhitePanel;
    public float fadeDuration = 1.5f;

    static bool hasPlayed = false;
    public float holdDuration = 0.25f;

    void Awake()
    {
        if (hasPlayed)
        {
            WhitePanel.gameObject.SetActive(false);
            return;
        }
        
        Color c = Color.white;
        c.a = 1f;
        WhitePanel.gameObject.SetActive(true);
        WhitePanel.color = Color.white;
        WhitePanel.raycastTarget = true;

        hasPlayed = true;

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        yield return null;
        yield return null;
        yield return new WaitForSecondsRealtime(holdDuration);
        
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            Color c = WhitePanel.color;
            c.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            WhitePanel.color = c;
            yield return null;
        }

        WhitePanel.raycastTarget = false;
        WhitePanel.gameObject.SetActive(false);
    }
}
