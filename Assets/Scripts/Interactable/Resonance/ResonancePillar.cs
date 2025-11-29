using System.Collections;
using UnityEngine;

public class ResonancePillar : Interactable
{
    [Header("Resonancia")]
    [SerializeField] private int maxLevel = 4;   // número de estados (0..maxLevel-1)
    [SerializeField] private int currentLevel = 0;

    [Header("Visuales")]
    [SerializeField] private Renderer pillarRenderer;
    [SerializeField] private Color[] levelColors; // un color por nivel
    [SerializeField] private float[] intensities; // un nivel de intensidad por nivel
    private static readonly int EmissionID = Shader.PropertyToID("_EmissionColor");
    [SerializeField] private float pulseMultiplier = 1.5f;
    [SerializeField] private float pulseDuration = 0.1f;
    private Coroutine pulseRoutine;

    [Header("Audio")]
    [SerializeField] private AudioSource humAudioSource;
    [SerializeField] private float basePitch = 0.8f;
    [SerializeField] private float pitchStep = 0.2f;

    [Header("Rotación")]
    [SerializeField] private float rotationStep = 90f;
    [SerializeField] private float rotationSpeed = 8f;

    private Quaternion targetRotation;
    private bool isRotating = false;

    // Evento para notificar al manager
    public System.Action<ResonancePillar> OnPillarChanged;

    private void Awake()
    {
        if (pillarRenderer == null)
            pillarRenderer = GetComponentInChildren<Renderer>();

        if (humAudioSource == null)
            humAudioSource = GetComponent<AudioSource>();

        ClampSetup();

        // Rotación inicial
        targetRotation = transform.rotation;

        UpdateStateVisuals();
    }

    private void Update()
    {
        // Rotación suave hacia el target
        if (isRotating)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation;
                isRotating = false;
            }

            if(gameObject.layer != LayerMask.NameToLayer("IgnoreInteractable"))
            {
                gameObject.layer = LayerMask.NameToLayer("IgnoreInteractable");
            }
        }
        else
        {
            if (gameObject.layer != LayerMask.NameToLayer("Interactable"))
            {
                gameObject.layer = LayerMask.NameToLayer("Interactable");
            }
        }
    }


    private void ClampSetup()
    {
        if (maxLevel < 1) maxLevel = 1;
        if (currentLevel < 0) currentLevel = 0;
        if (currentLevel >= maxLevel) currentLevel = maxLevel - 1;
    }

    public override void Interact()
    {
        CycleLevel();
    }

    private void CycleLevel()
    {
        currentLevel = (currentLevel + 1) % maxLevel;

        // Calcular nueva rotación objetivo
        targetRotation *= Quaternion.Euler(0f, rotationStep, 0f);
        isRotating = true;

        UpdateStateVisuals();
        PulseEmission();

        OnPillarChanged?.Invoke(this);
    }

    private void PulseEmission()
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        pulseRoutine = StartCoroutine(PulseRoutine());
    }

    private IEnumerator PulseRoutine()
    {
        if (pillarRenderer == null) yield break;

        var mat = pillarRenderer.material;

        // Guardar color actual
        Color baseColor = mat.GetColor(EmissionID);

        // Subir emisión
        Color pulsed = baseColor * pulseMultiplier;
        mat.SetColor(EmissionID, pulsed);

        yield return new WaitForSeconds(pulseDuration);

        // Volver al valor según nivel
        UpdateStateVisuals();
    }

    private void UpdateStateVisuals()
    {
        // Color / Emission según nivel
        if (pillarRenderer != null)
        {
            Color c = Color.white;

            if (levelColors != null && levelColors.Length > 0)
            {
                int idx = Mathf.Clamp(currentLevel, 0, levelColors.Length - 1);
                c = levelColors[idx] * intensities[idx];
            }

            var mat = pillarRenderer.material;
            mat.SetColor(EmissionID, c);
        }      
    }

    public int GetCurrentLevel() => currentLevel;


    public void PlayAudio()
    {
        if (humAudioSource != null)
        {
            humAudioSource.playOnAwake = false;
            humAudioSource.pitch = basePitch + pitchStep * currentLevel;

            if (!humAudioSource.isPlaying)
                humAudioSource.Play();
        }
    }
}
