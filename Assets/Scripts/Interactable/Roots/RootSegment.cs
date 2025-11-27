using System.Collections.Generic;
using UnityEngine;

public class RootSegment : Interactable
{
    public enum Direction
    {
        North,
        East,
        South,
        West
    }

    [Header("Configuración de rotación")]
    [SerializeField] private float rotationStep = 90f;
    [SerializeField] private float rotationSpeed = 10f; // por si luego querés animarla
    private int currentRotationIndex = 0; // 0,1,2,3 -> 0°,90°,180°,270°

    [Header("Conexiones base (en rotación 0°)")]
    [Tooltip("Direcciones a las que conecta este segmento cuando está en rotación 0° (Y=0)")]
    [SerializeField] private List<Direction> baseConnections = new List<Direction>();

    [Header("Visuales")]
    [SerializeField] private List<Renderer> segmentsRenderer;
    [SerializeField] private Color baseEmissionColor = new Color(0f, 0.5f, 0.5f);
    [SerializeField] private Color poweredEmissionColor = new Color(0f, 2f, 2f);

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip rotateClip;

    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    private bool isPowered = false;

    // Si más adelante querés avisar al manager:
    public System.Action OnSegmentRotated;

    private bool isRotating = false;
    private Quaternion targetRotation;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void Start()
    {
        // Aseguramos que la rotación inicial coincide con currentRotationIndex = 0
        targetRotation = transform.rotation;

        if (segmentsRenderer == null || segmentsRenderer.Count == 0)
        {
            segmentsRenderer.AddRange(transform.GetComponentsInChildren<Renderer>());
        }

        UpdateEmission();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void UpdateEmission()
    {
        if (segmentsRenderer == null || segmentsRenderer.Count == 0) return;

        foreach(Renderer segment in segmentsRenderer)
        {
            Color targetColor = isPowered ? poweredEmissionColor : baseEmissionColor;
            segment.material.SetColor(EmissionColorID, targetColor);
        }
        
    }

    public void SetPowered(bool powered)
    {
        isPowered = powered;
        UpdateEmission();
        //Debug.Log(this.name + " powered state: " + isPowered);
    }


    public override void Interact()
    {
        // Llamado cuando el jugador aprieta E
        RotateSegment();
    }

    private void Update()
    {
        // Rotación suave (opcional)
        if (isRotating)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            // Si ya llegó casi a la rotación destino, frenamos
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation;
                isRotating = false;
            }
        }
    }

    private void RotateSegment()
    {
        if (audioSource != null && rotateClip != null)
        {
            audioSource.PlayOneShot(rotateClip);
        }

        // Aumentar índice de rotación (0..3)
        currentRotationIndex = (currentRotationIndex + 1) % 4;

        // Calcular la nueva rotación objetivo
        float newYRotation = currentRotationIndex * rotationStep;
        targetRotation = Quaternion.Euler(0f, newYRotation, 0f);
        isRotating = true;

        transform.localScale = originalScale * 1.1f;
        Invoke(nameof(ResetScale), 0.1f);

        // Debug
        Debug.Log($"{name} rotated. RotationIndex: {currentRotationIndex}");

        // Avisar a quien escuche (más adelante: manager de flujo)
        OnSegmentRotated?.Invoke();

        /*if (!isPowered) // solo si no está ya energizado
        {
            foreach (Renderer segment in segmentsRenderer)
            {
                segment.material.SetColor(EmissionColorID, poweredEmissionColor * 1.5f);
            }

            // Podés después de un tiempo volver a UpdateEmission, pero para MVP podemos dejarlo así.
        }*/
    }


    private void ResetScale()
    {
        transform.localScale = originalScale;
    }

    /// <summary>
    /// Devuelve las direcciones a las que conecta este segmento
    /// según su rotación actual.
    /// </summary>
    public List<Direction> GetCurrentConnections()
    {
        List<Direction> result = new List<Direction>();

        foreach (var dir in baseConnections)
        {
            result.Add(RotateDirection(dir, currentRotationIndex));
        }

        return result;
    }

    /// <summary>
    /// Rota una dirección base N/E/S/O según cuántos pasos de 90° le aplicamos.
    /// </summary>
    private Direction RotateDirection(Direction original, int steps)
    {
        int dirIndex = (int)original;
        // sumamos steps y hacemos módulo 4 (N=0,E=1,S=2,W=3)
        int rotatedIndex = (dirIndex + steps) % 4;
        return (Direction)rotatedIndex;
    }

#if UNITY_EDITOR
    // Gizmos para debug visual de conexiones
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 origin = transform.position;

        var currentConnections = Application.isPlaying
            ? GetCurrentConnections()
            : baseConnections; // en edit mode mostramos las base

        foreach (var dir in currentConnections)
        {
            Vector3 offset = Vector3.zero;

            switch (dir)
            {
                case Direction.North: offset = Vector3.forward; break;
                case Direction.East: offset = Vector3.right; break;
                case Direction.South: offset = Vector3.back; break;
                case Direction.West: offset = Vector3.left; break;
            }

            Gizmos.DrawLine(origin, origin + offset);
            Gizmos.DrawSphere(origin + offset, 0.1f);
        }
    }
#endif
}
