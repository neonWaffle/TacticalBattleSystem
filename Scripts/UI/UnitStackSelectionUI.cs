using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnitStackSelectionUI : UnitStackUI, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] Image frameImage;

    [SerializeField] Color defaultColour = Color.white;
    [SerializeField] Color availableSlotColour = Color.green;
    [SerializeField] Color unavailableSlotColour = Color.red;

    Canvas canvas;
    CanvasGroup canvasGroup;
    BattleSelectionHandler battleSelectionHandler;

    public UnitStackUISlot UnitStackUISlot { get; private set; }
    UnitStackUISlot hoverUnitStackUISlot;

    void Awake()
    {
        canvas = transform.root.GetComponentInChildren<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        battleSelectionHandler = transform.root.GetComponentInChildren<BattleSelectionHandler>();
    }

    public void AssignSlot(UnitStackUISlot slot)
    {
        if (UnitStackUISlot != null)
        {
            UnitStackUISlot.RemoveStackUI();
        }

        UnitStackUISlot = slot;
        slot.AssignStackUI(this);
        PlaceOnSlot();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.parent = canvas.transform;
        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        var slot = eventData.pointerEnter.GetComponent<UnitStackUISlot>();
        hoverUnitStackUISlot = slot;
        if (slot != null)
        {
            frameImage.color = IsSlotAvailable(slot) || (slot.UnitStackUI != null && CanUnitStacksBeMerged(slot.UnitStackUI.UnitStack))
                ? availableSlotColour
                : unavailableSlotColour;
        }
        else
        {
            var stackUI = eventData.pointerEnter.GetComponent<UnitStackSelectionUI>();
            if (stackUI != null)
            {
                frameImage.color = CanUnitStacksBeMerged(stackUI.UnitStack) ? availableSlotColour : unavailableSlotColour;
                hoverUnitStackUISlot = stackUI.UnitStackUISlot;
            }
        }
    }

    bool IsSlotAvailable(UnitStackUISlot slot)
    {
        return slot.UnitStackUI == null && (slot.IsReserveSlot || !UnitStackUISlot.IsReserveSlot || !battleSelectionHandler.IsPlayerRosterFull());
    }

    bool CanUnitStacksBeMerged(UnitStack unitStack)
    {
        return unitStack != null && unitStack.UnitSO == UnitStack.UnitSO;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (hoverUnitStackUISlot != null && hoverUnitStackUISlot != UnitStackUISlot)
        {
            if (IsSlotAvailable(hoverUnitStackUISlot))
            {
                AssignSlot(hoverUnitStackUISlot);
            }
            else if (hoverUnitStackUISlot.UnitStackUI != null && CanUnitStacksBeMerged(hoverUnitStackUISlot.UnitStackUI.UnitStack))
            {
                UnitStack.UpdateAmount(UnitStack.Amount + hoverUnitStackUISlot.UnitStackUI.UnitStack.Amount);
                Destroy(hoverUnitStackUISlot.UnitStackUI.gameObject);
                hoverUnitStackUISlot.RemoveStackUI();
                AssignSlot(hoverUnitStackUISlot);
            }
            else
            {
                PlaceOnSlot();
            }
        }
        else
        {
            PlaceOnSlot();
        }
        hoverUnitStackUISlot = null;
        canvasGroup.blocksRaycasts = true;
        frameImage.color = defaultColour;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            //Should only be able to split stacks that aren't in reserve
            if (!UnitStackUISlot.IsReserveSlot)
            {
                SplitUnitStack();
            }
        }
    }

    void SplitUnitStack()
    {
        battleSelectionHandler.SplitUnitFormation(UnitStackUISlot);
    }

    void PlaceOnSlot()
    {
        transform.position = UnitStackUISlot.transform.position;
        transform.parent = UnitStackUISlot.transform;
        transform.SetAsLastSibling();
    }
}
