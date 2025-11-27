using UnityEngine;

public class PetalItem : Interactable
{
    private bool isHeld = false;
    private Transform holder;

    [SerializeField] private Vector3 holdOffset = new Vector3(0, 1f, 0.5f);

    private int originalLayer;

    [Header("Animación")]
    [SerializeField] private Animator animator;
    private static readonly int IsOnPedestalID = Animator.StringToHash("IsOnPedestal");

    private void Awake()
    {
        originalLayer = gameObject.layer;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    public override void Interact()
    {
        if (!isHeld)
        {
            PickUp();
        }
        else
        {
            Drop();
        }
    }

    private void PickUp()
    {
        isHeld = true;

        // que no sea interactuable mientras lo llevás
        gameObject.layer = LayerMask.NameToLayer("IgnoreInteractable");

        // asegurate de que el jugador tiene tag "Player"
        holder = GameObject.FindWithTag("Player").transform;
        transform.SetParent(holder);
        transform.localPosition = holdOffset;
        transform.localEulerAngles = Vector3.zero;

        // si lo sacás del pedestal, apagamos flotación
        //SetOnPedestal(false);
    }

    public void Drop()
    {
        isHeld = false;

        gameObject.layer = originalLayer;

        transform.SetParent(null);

        // al soltar en el mundo también debería dejar de estar "en pedestal"
        //SetOnPedestal(false);
    }

    public void SetOnPedestal(bool on)
    {
        if (animator != null)
        {
            animator.SetBool(IsOnPedestalID, on);
        }
    }

    public bool IsHeld => isHeld;
}
