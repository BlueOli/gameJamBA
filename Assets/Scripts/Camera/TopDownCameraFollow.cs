using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    [Header("Objetivo a seguir")]
    [SerializeField] private Transform target;

    [Header("Offset")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 12f, -12f);

    [Header("Suavizado")]
    [SerializeField] private float followSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null) return;

        // Posición deseada (target + offset en espacio de mundo)
        Vector3 desiredPosition = target.position + offset;

        // Interpolación suave
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );
    }
}
