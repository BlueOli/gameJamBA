using System.Collections;
using UnityEngine;

public class CoreCrystal : Interactable
{
    [SerializeField] private TreeEnergyController treeController;
    [SerializeField] private GameObject finalCinematicTrigger; // opcional
    [SerializeField] private TimeAnchor anchorToDestroy;

    [Header("Visuales")]
    [SerializeField] private Renderer crystalRenderer;

    [Header("Colores / Emission")]
    [SerializeField] private Color inactiveColor = new Color(0.05f, 0.02f, 0.1f);
    [SerializeField] private Color activeColor = new Color(0.8f, 0.6f, 1f);

    [SerializeField] private float inactiveEmission = 0.2f;
    [SerializeField] private float activeEmission = 3f;

    [Header("Cinematica Final")]
    [SerializeField] private CinematicCameraController cinematicCam;
    [SerializeField] private Transform finalCinematicPoint;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private float finalDuration = 4f;
    [SerializeField] private GameObject credits;


    private static readonly int EmissionID = Shader.PropertyToID("_EmissionColor");

    private bool activated = false;

    public void Awake()
    {
        UpdateVisuals(inactiveColor, inactiveEmission);
    }

    public override void Interact()
    {
        if (activated) return;
        activated = true;

        if (treeController != null)
            treeController.SetState(true);

        if (finalCinematicTrigger != null)
            finalCinematicTrigger.SetActive(true);

        if (anchorToDestroy != null)
        {
            Destroy(anchorToDestroy.gameObject);
            Debug.Log("[CoreCrystal] TimeAnchor destruido.");
        }

        UpdateVisuals(activeColor, activeEmission);
        gameObject.layer = LayerMask.NameToLayer("IgnoreInteractable");

        Debug.Log("Core activado. Sistema del bosque calibrado.");

        player.canMove = false;

        StartCoroutine(PlayFinalCinematic());

    }

    private void UpdateVisuals(Color baseColor, float emissionIntensity)
    {
        if (crystalRenderer != null)
        {
            var mat = crystalRenderer.material;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor(EmissionID, baseColor * emissionIntensity);
        }
    }

    private IEnumerator PlayFinalCinematic()
    {
        player.canMove = false;

        // Cámara cinemática final
        yield return StartCoroutine(
            cinematicCam.PlayCinematic(finalCinematicPoint, finalDuration)
        );

        // Energizar árbol central
        treeController.SetState(true);

        // Fade final largo
        yield return StartCoroutine(FadeController.Instance.FadeOut(1.5f));

        credits.SetActive(true);

        Debug.Log("Fin del juego");
    }


}
