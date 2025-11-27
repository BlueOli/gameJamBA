using UnityEngine;

public class EnergyPlatform : MonoBehaviour
{
    [Header("Estado")]
    public bool isActive = false;
    public bool isAligned = false;

    [Header("Visuales")]
    [SerializeField] private Renderer platformRenderer;
    [SerializeField] private Color offEmissionColor = Color.black;
    [SerializeField] private Color onEmissionColor = new Color(0f, 2f, 2f);
    private static readonly int EmissionID = Shader.PropertyToID("_EmissionColor");

    [Header("Movimiento")]
    [SerializeField] private Transform alignTarget;    // a qué altura debe alinearse
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float alignThreshold = 0.01f;
    [SerializeField] private bool alignOnlyY = true;

    [Header("Vibración")]
    [SerializeField] private bool enableVibration = true;
    [SerializeField] private float vibrationAmplitude = 0.05f;   // qué tanto tiembla
    [SerializeField] private float vibrationFrequency = 20f;     // qué tan rápido tiembla

    [Header("Partículas")]
    [SerializeField] private ParticleSystem liftParticles;

    [Header("Audio")]
    [SerializeField] private AudioSource liftAudio;

    private Vector3 basePosition;   // posición “ideal” sin vibración
    private float vibrationTime = 0f;

    private void Awake()
    {
        if (platformRenderer == null)
            platformRenderer = GetComponentInChildren<Renderer>();

        basePosition = transform.position;
        UpdateVisuals();
    }

    private void Update()
    {
        if (isActive && !isAligned && alignTarget != null)
        {
            MoveTowardsTarget();
            ApplyVibration();
            PlayLiftParticles();
        }
        else
        {
            StopLiftParticles();
        }

        if (isActive && !isAligned)
        {
            if (liftAudio != null && !liftAudio.isPlaying)
                liftAudio.Play();
        }
        else
        {
            if (liftAudio != null && liftAudio.isPlaying)
                liftAudio.Stop();
        }
    }

    public void SetActive(bool active)
    {
        isActive = active;

        if (!isActive)
        {
            isAligned = false;
            basePosition = transform.position;
            vibrationTime = 0f;
        }

        UpdateVisuals();
    }


    private void MoveTowardsTarget()
    {
        Vector3 current = basePosition;
        Vector3 target = alignTarget.position;

        if (alignOnlyY)
        {
            target.x = current.x;
            target.z = current.z;
        }

        // mover basePosition (sin vibración)
        basePosition = Vector3.MoveTowards(current, target, moveSpeed * Time.deltaTime);

        float dist = Vector3.Distance(basePosition, target);
        if (dist <= alignThreshold)
        {
            basePosition = target;
            isAligned = true;
            vibrationTime = 0f;
            // al alinearse, dejamos la posición final exacta sin vibración
            transform.position = basePosition;
        }
    }


    private void ApplyVibration()
    {
        if (!enableVibration)
        {
            transform.position = basePosition;
            return;
        }

        vibrationTime += Time.deltaTime;

        // vibración pequeña en X/Z o en Y, como prefieras.
        float offsetY = Mathf.Sin(vibrationTime * vibrationFrequency) * vibrationAmplitude;

        transform.position = basePosition + new Vector3(0f, offsetY, 0f);
    }

    private void UpdateVisuals()
    {
        if (platformRenderer == null) return;

        Color emission = isActive ? onEmissionColor : offEmissionColor;
        var mat = platformRenderer.material;
        mat.SetColor(EmissionID, emission);
    }

    private void PlayLiftParticles()
    {
        if (liftParticles == null) return;
        if (!liftParticles.isPlaying)
            liftParticles.Play();
    }

    private void StopLiftParticles()
    {
        if (liftParticles == null) return;
        if (liftParticles.isPlaying)
            liftParticles.Stop();
    }
}
