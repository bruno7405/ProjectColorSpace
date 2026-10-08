using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public Material Mat;
    public RawImage Overlay;
    public float Duration = 1.5f;

    void Awake()
    {
        Overlay.gameObject.SetActive(false);
    }

    public void LoadScene(string scene)
    {
        StartCoroutine(Run(scene));
    }
    
    IEnumerator Run(string scene)
    {
        Overlay.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();

        var snapshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        snapshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        snapshot.Apply();
        snapshot.wrapMode = TextureWrapMode.Clamp;
        snapshot.filterMode = FilterMode.Bilinear;

        Mat.SetTexture("_Snapshot", snapshot);
        Mat.SetFloat("_Progress", 0f);
        Overlay.gameObject.SetActive(true);

        yield return SceneManager.LoadSceneAsync(scene);

        for (float t = 0; t < 1f; t += Time.unscaledDeltaTime / Duration)
        {
            Mat.SetFloat("_Progress", t);
            yield return null;
        }

        Overlay.gameObject.SetActive(false);
        Destroy(snapshot);
    }
}
