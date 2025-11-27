using UnityEngine;

public class EnergyPedestal : Interactable
{
    public bool hasPetal = false;
    public EnergyPlatform energyPlatform;

    public void Awake()
    {
        energyPlatform.SetActive(hasPetal);
    }

    public override void Interact()
    {
        PetalItem carried = FindFirstObjectByType<PetalItem>();
        if (carried != null && carried.IsHeld)
        {
            PlacePetal(carried);
        }
    }

    private void PlacePetal(PetalItem petal)
    {
        hasPetal = true;
        petal.Drop();
        petal.transform.SetParent(transform);
        petal.transform.localPosition = Vector3.up * 2f;
        petal.transform.localEulerAngles = Vector3.zero;
        gameObject.layer = LayerMask.NameToLayer("IgnoreInteractable");
        petal.gameObject.layer = LayerMask.NameToLayer("IgnoreInteractable");
        petal.GetComponent<Rigidbody>().isKinematic = true;
        petal.SetOnPedestal(true);
        energyPlatform.SetActive(hasPetal);
    }
}
