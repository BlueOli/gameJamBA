using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Detección")]
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private float forwardDotThreshold = 0.2f;
    [SerializeField] private LayerMask interactableMask;

    [Header("UI Prompt (opcional)")]
    [SerializeField] private InteractionPromptUI interactionPromptUI;

    private Interactable currentInteractable;

    private void Update()
    {
        DetectInteractable();
        HandleInput();
    }

    private void DetectInteractable()
    {
        currentInteractable = null;

        // Buscamos alrededor del jugador
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            interactRange,
            interactableMask
        );

        float bestScore = -1f;
        Interactable best = null;

        foreach (var hit in hits)
        {
            Interactable interactable = hit.GetComponent<Interactable>();
            if (interactable == null) continue;

            // Chequeamos que esté más o menos "adelante" del jugador
            Vector3 toTarget = (hit.transform.position - transform.position);
            toTarget.y = 0f;
            float dist = toTarget.magnitude;
            if (dist < 0.01f) continue;

            Vector3 dir = toTarget.normalized;
            float dot = Vector3.Dot(transform.forward, dir);

            // dot > 0 = está al frente; usamos threshold para no incluir cosas atrás
            if (dot > forwardDotThreshold && dot > bestScore)
            {
                bestScore = dot;
                best = interactable;
            }
        }

        currentInteractable = best;

        // Actualizar UI
        if (interactionPromptUI != null)
        {
            if (currentInteractable != null)
            {
                interactionPromptUI.ShowPrompt(currentInteractable.interactionText);
            }
            else
            {
                interactionPromptUI.HidePrompt();
            }
        }
    }

    private void HandleInput()
    {
        if (currentInteractable == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact();
        }
    }

    // Para ver el rango en la Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
