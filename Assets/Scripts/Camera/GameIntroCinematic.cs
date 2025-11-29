using System.Collections;
using UnityEngine;

public class GameIntroCinematic : MonoBehaviour
{
    public CinematicCameraController cinematicCam;
    public Transform cinematicStartPoint;
    public float cinematicDuration = 3f;
    public PlayerMovement player;

    private void Start()
    {
        StartCoroutine(RunIntro());
    }

    private IEnumerator RunIntro()
    {
        player.canMove = false;

        // Fade in desde negro al comienzo (opcional)
        yield return StartCoroutine(FadeController.Instance.FadeIn(1f));

        // Cinemática inicial
        yield return StartCoroutine(
            cinematicCam.PlayCinematic(cinematicStartPoint, cinematicDuration)
        );

        // Fade hacia gameplay normal
        yield return StartCoroutine(FadeController.Instance.FadeOut(0.5f));
        yield return StartCoroutine(FadeController.Instance.FadeIn(0.5f));

        cinematicCam.RestorePlayerCamera();
        player.canMove = true;
    }

}

