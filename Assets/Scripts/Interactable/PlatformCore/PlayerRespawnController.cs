using UnityEngine;

public class PlayerRespawnController : MonoBehaviour
{
    [Header("Respawn")]
    [SerializeField] private Transform currentCheckpoint;
    [SerializeField] private float respawnYOffset = 1.0f;

    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void SetCheckpoint(Transform checkpoint)
    {
        currentCheckpoint = checkpoint;
        Debug.Log("[Checkpoint] Nuevo checkpoint: " + checkpoint.name);
    }

    public void Respawn()
    {
        if (currentCheckpoint == null)
        {
            Debug.LogWarning("No hay checkpoint, no se puede respawnear.");
            return;
        }

        Vector3 newPos = currentCheckpoint.position + Vector3.up * respawnYOffset;

        // Si tenés CharacterController, hay que desactivarlo al teletransportar
        if (controller != null)
        {
            controller.enabled = false;
            transform.position = newPos;
            controller.enabled = true;
        }
        else
        {
            transform.position = newPos;
        }

        Debug.Log("[Respawn] Teletransportado al checkpoint: " + currentCheckpoint.name);
    }
}
