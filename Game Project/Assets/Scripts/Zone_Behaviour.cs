using UnityEngine;
using UnityEngine.EventSystems;

//responsible for creating a Zone to drop the cards to, Parent of other Zones that support dragging
public class Zone_Behaviour : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public int Size;  //used by other Zones
    public GameObject CardPrefab;       //type of prefab for Card (attached via Inspector)
    public GameObject ZoneUnit;         //Zone Unit (attached via Inspector)

    public virtual void EventDrop(Card_Drag cardObject){ }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            return;
        }
        Card_Drag d = eventData.pointerDrag.GetComponent<Card_Drag>();
        if (d != null && this.transform.childCount < this.Size)
        {
            //d.GetComponent<CanvasGroup>().alpha = 1f;     //reenables itself in a loop for no reason
            if (this.transform == d.hand)
            {
                d.placeholderParent = this.transform;       //only make a placeholder in Hand
            }
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            return;
        }
        Card_Drag d = eventData.pointerDrag.GetComponent<Card_Drag>();
        if ((d != null) && (d.placeholderParent == this.transform))
        {
            d.placeholderParent = d.parentReturnTo;
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        Card_Drag d = eventData.pointerDrag.GetComponent<Card_Drag>();
        if (d != null)
        {
            Card_Drag[] deck = this.transform.GetComponentsInChildren<Card_Drag>(); //walkaround to ignore placeholders and only check cards
            if (deck.Length < this.Size) 
            {
                if (d.parentReturnTo == d.unit.transform && this.transform != d.unit.transform) 
                {
                    d.unit.GetComponent<Zone_Unit>().FreeUnit();
                }
                d.parentReturnTo = this.transform;
                d.gameObject.transform.SetParent(this.transform);
                this.EventDrop(d);  //run response function to dropping a card (of the fitting Zone)
            }
        }
    }
}
