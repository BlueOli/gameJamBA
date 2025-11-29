using UnityEngine;

public class TimeAnchor : Interactable
{
    [SerializeField] private bool cycleForward = true;

    private bool isHeld = false;
    private Transform holder;

    [SerializeField] private Vector3 holdOffset = new Vector3(0, 1f, 0.5f);

    private int originalLayer;

    public void Awake()
    {
        originalLayer = gameObject.layer;
    }

    public override void Interact()
    {
        if (!isHeld)
        {
            PickUp();
            interactionText = "Change Time Phase";
        }
        else
        {
            if (TimePhaseManager.Instance == null) return;

            if (cycleForward)
                TimePhaseManager.Instance.NextPhase();
            else
                TimePhaseManager.Instance.SetPhase(0); // ejemplo: reset
        }        
    }

    private void PickUp()
    {
        isHeld = true;

        // que no sea interactuable mientras lo llevás
        //gameObject.layer = LayerMask.NameToLayer("IgnoreInteractable");

        // asegurate de que el jugador tiene tag "Player"
        holder = GameObject.FindWithTag("Player").transform;
        transform.SetParent(holder);
        transform.localPosition = holdOffset;
        transform.localEulerAngles = Vector3.zero;

        // si lo sacás del pedestal, apagamos flotación
        //SetOnPedestal(false);
    }

    public bool IsHeld => isHeld;
}
