using UnityEngine;

public class TimeAnchorLockoutTrigger : MonoBehaviour
{
    [SerializeField] private TimeAnchor anchorToDisable;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (anchorToDisable != null)
        {
            // Desactivar interacción
            anchorToDisable.gameObject.layer = LayerMask.NameToLayer("IgnoreInteractable");
            Debug.Log("[Lockout] TimeAnchor deshabilitado.");
        }
    }
}
