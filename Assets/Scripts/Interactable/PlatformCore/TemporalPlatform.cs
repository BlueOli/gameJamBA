using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TemporalPlatform : MonoBehaviour
{
    [Header("Fases en las que existe")]
    [Tooltip("Size debe coincidir con maxPhase en TimePhaseManager (ej: 3).")]
    [SerializeField] private bool[] activeInPhase;

    [Header("Visuales")]
    [SerializeField] private Renderer platformRenderer;

    [Header("Colores / Emission")]
    [SerializeField] private Color inactiveColor = new Color(0.05f, 0.02f, 0.1f);
    [SerializeField] private Color previewColor = new Color(0.5f, 0.35f, 0.9f);
    [SerializeField] private Color activeColor = new Color(0.8f, 0.6f, 1f);

    [SerializeField] private float inactiveEmission = 0.1f;
    [SerializeField] private float previewEmission = 0.5f;
    [SerializeField] private float activeEmission = 2f;

    private static readonly int EmissionID = Shader.PropertyToID("_EmissionColor");

    private Collider col;

    private void Awake()
    {
        if (platformRenderer == null)
            platformRenderer = GetComponentInChildren<Renderer>();

        col = GetComponent<Collider>();

        if (TimePhaseManager.Instance != null)
        {
            TimePhaseManager.Instance.RegisterPlatform(this);
        }
        else
        {
            Debug.LogWarning("[TemporalPlatform] No TimePhaseManager found in scene.");
        }
    }

    private void OnDestroy()
    {
        if (TimePhaseManager.Instance != null)
        {
            TimePhaseManager.Instance.UnregisterPlatform(this);
        }
    }

    public void ApplyPhase(int currentPhase, int nextPhase)
    {
        bool activeNow = IsActiveInPhase(currentPhase);
        bool activeNextPhase = IsActiveInPhase(nextPhase);

        if (activeNow)
        {
            // ACTIVA: sólida, pisable
            SetStateActive();
        }
        else if (activeNextPhase)
        {
            // PREVIEW: visible, fantasma, no pisable
            SetStatePreview();
        }
        else
        {
            // INACTIVA: casi invisible, no pisable
            SetStateInactive();
        }
    }

    private bool IsActiveInPhase(int phase)
    {
        if (activeInPhase == null || activeInPhase.Length == 0) return false;
        if (phase < 0 || phase >= activeInPhase.Length) return false;
        return activeInPhase[phase];
    }

    private void SetStateActive()
    {
        if (col != null) col.enabled = true;
        UpdateVisuals(activeColor, activeEmission, 2f, 1f);
    }

    private void SetStatePreview()
    {
        if (col != null) col.enabled = false;
        // un poquito más chico o hundido para que se lea “fantasma”
        UpdateVisuals(previewColor, previewEmission, 1f, 0.3f);
    }

    private void SetStateInactive()
    {
        if (col != null) col.enabled = false;
        UpdateVisuals(inactiveColor, inactiveEmission, 1f, 0.2f);
    }

    private void UpdateVisuals(Color baseColor, float emissionIntensity, float scale, float opacity)
    {
        if (platformRenderer != null)
        {
            var mat = platformRenderer.material;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor(EmissionID, baseColor * emissionIntensity);
            mat.SetColor(Shader.PropertyToID("_BaseColor"), new Color(mat.color.r, mat.color.g, mat.color.b, opacity));
        }

        // feedback geométrico simple
        Vector3 s = Vector3.one;
        s.x = scale;
        s.z = scale;
        transform.localScale = s;
    }
}
