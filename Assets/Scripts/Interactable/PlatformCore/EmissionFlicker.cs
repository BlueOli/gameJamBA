using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class EmissionFlicker : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] private Color baseEmissionColor = new Color(0.8f, 0.6f, 1f); // jacarandá
    [SerializeField] private float minIntensity = 0.2f;
    [SerializeField] private float maxIntensity = 2.0f;

    [Header("Noise")]
    [SerializeField] private float noiseSpeed = 2.0f;
    [SerializeField] private float noiseScale = 1.0f;

    [Header("Random Bursts")]
    [SerializeField] private float burstChancePerSecond = 0.5f;
    [SerializeField] private float burstIntensityMultiplier = 2.5f;
    [SerializeField] private float burstDuration = 0.12f;

    private Renderer rend;
    private Material matInstance;
    private static readonly int EmissionID = Shader.PropertyToID("_EmissionColor");

    private float burstTimer = 0f;
    private float currentBurstMultiplier = 1f;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        // Instancia propia para no modificar el material global
        matInstance = rend.material;
        matInstance.EnableKeyword("_EMISSION");
    }

    private void Update()
    {
        float time = Time.time * noiseSpeed;

        // Ruido pseudo-random suave (Perlin)
        float noise = Mathf.PerlinNoise(time, time * noiseScale);
        float baseIntensity = Mathf.Lerp(minIntensity, maxIntensity, noise);

        // Manejo de bursts aleatorios (chispazos)
        UpdateBurst();

        float finalIntensity = baseIntensity * currentBurstMultiplier;

        matInstance.SetColor(EmissionID, baseEmissionColor * finalIntensity);
    }

    private void UpdateBurst()
    {
        // Reducir multiplicador si hay burst activo
        if (currentBurstMultiplier > 1f)
        {
            burstTimer -= Time.deltaTime;
            if (burstTimer <= 0f)
            {
                currentBurstMultiplier = 1f;
            }
            return;
        }

        // Probabilidad de iniciar un burst este frame
        float probThisFrame = burstChancePerSecond * Time.deltaTime;
        if (Random.value < probThisFrame)
        {
            currentBurstMultiplier = burstIntensityMultiplier;
            burstTimer = burstDuration;
        }
    }

    private void OnDestroy()
    {
        if (Application.isPlaying && matInstance != null)
        {
            Destroy(matInstance);
        }
    }
}
