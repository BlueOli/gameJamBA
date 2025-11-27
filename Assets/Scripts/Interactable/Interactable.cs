using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Tooltip("Texto opcional que se muestra en el prompt de interacción")]
    public string interactionText = "Interact";

    // Llamado por el jugador cuando aprieta E
    public virtual void Interact()
    {
        Debug.Log($"Interacting with: {gameObject.name}");
        // Sobrescribir en clases hijas para comportamiento específico
    }
}
