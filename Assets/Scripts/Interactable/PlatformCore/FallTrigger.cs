using UnityEngine;

public class FallTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerRespawnController respawner = other.GetComponent<PlayerRespawnController>();
        if (respawner != null)
        {
            respawner.Respawn();
        }
    }
}
