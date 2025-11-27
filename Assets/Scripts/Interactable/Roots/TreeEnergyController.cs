using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreeEnergyController : MonoBehaviour
{
    [Header("Visuales")]
    [SerializeField] private List<Renderer> leafRenderers;
    [SerializeField] private Color offEmissionColor = new Color(0f, 0.1f, 0.1f);
    [SerializeField] private Color onEmissionColor = new Color(0f, 2f, 2f);

    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    [Header("Partículas")]
    [SerializeField] private ParticleSystem loopParticles; // opcional, efecto constante
    [SerializeField] private ParticleSystem activationParticles;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip activationClip;



    private bool isOn = false;

    private void Awake()
    {
        if (leafRenderers == null)
        {
            leafRenderers.AddRange(transform.GetChild(1).GetComponentsInChildren<Renderer>());
        }

        UpdateVisuals();
    }

    public void SetState(bool on)
    {
        if (isOn == on) return;

        isOn = on;
        UpdateVisuals();

        if (isOn)
        {
            PlayActivationFeedback();
        }
        else
        {
            StopLoopParticles();
        }
    }

    private void UpdateVisuals()
    {
        if (leafRenderers == null) return;

        Color emission = isOn ? onEmissionColor : offEmissionColor;

        foreach (Renderer renderer in leafRenderers)
        {
            var mat = renderer.material;
            mat.SetColor(EmissionColorID, emission);
        }        
    }

    private void PlayActivationFeedback()
    {
        if (activationParticles != null)
        {
            activationParticles.Play();
        }

        if (loopParticles != null)
        {
            loopParticles.Play();
        }

        if (audioSource != null && activationClip != null)
        {
            audioSource.PlayOneShot(activationClip);
        }
    }

    private void StopLoopParticles()
    {
        if (loopParticles != null)
        {
            loopParticles.Stop();
        }
    }
}
