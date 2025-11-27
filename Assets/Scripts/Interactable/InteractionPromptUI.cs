using TMPro;
using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private GameObject rootObject;
    [SerializeField] private TextMeshProUGUI promptText;

    private void Awake()
    {
        HidePrompt();
    }

    public void ShowPrompt(string text)
    {
        if (promptText != null)
            promptText.text = $"[E] {text}";

        if (rootObject != null)
            rootObject.SetActive(true);
    }

    public void HidePrompt()
    {
        if (rootObject != null)
            rootObject.SetActive(false);
    }
}
