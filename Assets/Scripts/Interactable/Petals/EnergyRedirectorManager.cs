using UnityEngine;

public class EnergyRedirectorManager : MonoBehaviour
{
    [Header("Pilares del puzzle")]
    [SerializeField] private EnergyPedestal pedestalA;
    [SerializeField] private EnergyPedestal pedestalB;

    [SerializeField] private EnergyPlatform platformA;
    [SerializeField] private EnergyPlatform platformB;

    [Header("Puente")]
    [SerializeField] private GameObject bridgeRestored;

    private bool bridgeOpened = false;

    private void Update()
    {
        bool aHasPetal = pedestalA.hasPetal;
        bool bHasPetal = pedestalB.hasPetal;

        // Plataformas se "encienden" cuando el pedestal tiene pétalo
        platformA.SetActive(aHasPetal);
        platformB.SetActive(bHasPetal);

        // Condición para abrir el puente:
        // - ambos pedestales activos
        // - plataforma A ya se alineó con el target
        if (!bridgeOpened && aHasPetal && bHasPetal && platformA.isAligned)
        {
            OpenBridge();
        }
    }

    private void OpenBridge()
    {
        bridgeOpened = true;

        if (bridgeRestored != null)
            bridgeRestored.SetActive(true);

        Debug.Log("[EnergyRedirector] Puente restaurado: condiciones cumplidas.");
    }
}
