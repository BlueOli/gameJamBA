using UnityEngine;
using System.Collections;

public class CinematicCameraController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;   // Main Camera

    [Header("Settings")]
    public float moveSpeed = 2f;
    public float rotateSpeed = 2f;

    private bool isPlayingCinematic = false;
    private Vector3 savedPlayerCameraPos;
    private Quaternion savedPlayerCameraRot;

    private void Awake()
    {
        // Si no se asignó en el inspector, usar la MainCamera
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    public IEnumerator PlayCinematic(Transform targetPos, float duration)
    {
        if (cameraTransform == null)
        {
            Debug.LogError("[CinematicCameraController] cameraTransform es null.");
            yield break;
        }

        isPlayingCinematic = true;

        savedPlayerCameraPos = cameraTransform.position;
        savedPlayerCameraRot = cameraTransform.rotation;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;

            cameraTransform.position = Vector3.Lerp(
                cameraTransform.position,
                targetPos.position,
                Time.deltaTime * moveSpeed
            );

            cameraTransform.rotation = Quaternion.Lerp(
                cameraTransform.rotation,
                targetPos.rotation,
                Time.deltaTime * rotateSpeed
            );

            yield return null;
        }

        isPlayingCinematic = false;
    }

    public void RestorePlayerCamera()
    {
        if (cameraTransform == null) return;

        cameraTransform.position = savedPlayerCameraPos;
        cameraTransform.rotation = savedPlayerCameraRot;
    }

    public bool IsPlayingCinematic()
    {
        return isPlayingCinematic;
    }
}
