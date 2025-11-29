using System;
using UnityEngine;

public class ResonanceManager : MonoBehaviour
{
    [Header("Pilares del puzzle")]
    [SerializeField] private ResonancePillar[] pillars;

    [Header("Patrón correcto")]
    [SerializeField] private int[] correctPattern; // por ejemplo [2, 0, 3]

    [Header("Feedback")]
    [SerializeField] private TreeEnergyController treeController; // Árbol del Corredor
    [SerializeField] private GameObject pathBlocked;     // raíces cerrando el paso
    [SerializeField] private GameObject pathOpened;      // raíces abiertas / pasarela

    private bool puzzleSolved = false;

    private void Awake()
    {
        // Si no asignaste pillars a mano, intentá auto-buscar
        if (pillars == null || pillars.Length == 0)
        {
            pillars = FindObjectsByType<ResonancePillar>(FindObjectsSortMode.None);
        }

        // Suscribirse al evento de cambio en cada pilar
        foreach (var p in pillars)
        {
            p.OnPillarChanged += OnPillarChanged;
        }

        // Estado inicial del camino
        if (pathBlocked != null) pathBlocked.SetActive(true);
        if (pathOpened != null) pathOpened.SetActive(false);

        // Aseguramos que el árbol esté apagado al inicio
        if (treeController != null)
        {
            treeController.SetState(false);
        }
    }

    private void OnDestroy()
    {
        foreach (var p in pillars)
        {
            p.OnPillarChanged -= OnPillarChanged;
        }
    }

    private void OnPillarChanged(ResonancePillar changedPillar)
    {

        if (changedPillar.GetCurrentLevel() == Array.IndexOf(pillars, changedPillar)) changedPillar.PlayAudio();

        if (puzzleSolved) return;

        if (CheckPattern())
        {
            SolvePuzzle();
        }
    }

    private bool CheckPattern()
    {
        if (correctPattern == null || correctPattern.Length == 0) return false;
        if (pillars == null || pillars.Length == 0) return false;
        if (correctPattern.Length != pillars.Length) return false;

        for (int i = 0; i < pillars.Length; i++)
        {
            if (pillars[i].GetCurrentLevel() != correctPattern[i])
                return false;
        }

        return true;
    }

    private void SolvePuzzle()
    {
        puzzleSolved = true;
        Debug.Log("[ResonanceManager] Puzzle 3 resuelto: corredor calibrado.");

        // Encender árbol Jacarandá
        if (treeController != null)
        {
            treeController.SetState(true);
        }

        // Abrir camino hacia el Núcleo Central
        if (pathBlocked != null) pathBlocked.SetActive(false);
        if (pathOpened != null) pathOpened.SetActive(true);

        // TODO: sonido especial, partículas, cámara, etc.
    }
}
