using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance { get; private set; }

    [SerializeField] private Image fadeImage;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        if (fadeImage == null)
        {
            fadeImage = GetComponentInChildren<Image>();
        }
    }

    public IEnumerator FadeOut(float duration)
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.SmoothStep(0f, 1f, t / duration);

            fadeImage.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }
    }

    public IEnumerator FadeIn(float duration)
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / duration);

            fadeImage.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }
    }
}
