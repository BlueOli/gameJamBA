using UnityEngine;
using UnityEngine.SceneManagement;

public class PressAnyKey : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "MainScene";
    [SerializeField] private bool useFade = true;
    [SerializeField] private float fadeDuration = 1f;

    private bool starting = false;

    private void Update()
    {
        if (starting) return;

        // Cualquier tecla o click/joystick
        if (Input.anyKeyDown)
        {
            starting = true;
            StartGame();
        }
    }

    private void StartGame()
    {
        if (useFade && FadeController.Instance != null)
        {
            StartCoroutine(StartGameWithFade());
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private System.Collections.IEnumerator StartGameWithFade()
    {
        yield return FadeController.Instance.FadeOut(fadeDuration);
        SceneManager.LoadScene(sceneToLoad);
    }
}
