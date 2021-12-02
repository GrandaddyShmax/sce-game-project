using UnityEngine;
using UnityEngine.EventSystems;

//responsible for creating a Zone to drop the cards to, Parent of other Zones
public class ZoneBehaviour : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public int Size;  //used by other Zones
    public GameObject CardPrefab;       //type of prefab for Card (attached via Inspector)
    public GameObject MapBlank;

    public void EventDrop()
    { }

        public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            return;
        }
        CardDrag d = eventData.pointerDrag.GetComponent<CardDrag>();
        if (d != null)
        {
            d.GetComponent<CanvasGroup>().alpha = 1f;        //reset effect for card (can be changed)
            d.placeholderParent = this.transform;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            return;
        }
        CardDrag d = eventData.pointerDrag.GetComponent<CardDrag>();
        if ((d != null) && (d.placeholderParent == this.transform))
        {
            d.GetComponent<CanvasGroup>().alpha = .6f;      //effect for card when moved (can be changed)
            if (d.card.buildingPrefab == null)
            {
                d.placeholderParent = d.parentReturnTo;
            }
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        CardDrag d = eventData.pointerDrag.GetComponent<CardDrag>();
        if (d != null)
        {
            if (this.transform.GetComponentsInChildren<CardDrag>().Length < this.Size)  //switch parents only if there's extra space
            {
                if (d.card.buildingPrefab || this.transform != MapBlank.transform) //switch parents to map only if card is a building
                {
                    this.EventDrop();
                    Debug.Log("Change detected");
                    d.parentReturnTo = this.transform;  
                }
            }
        }
    }
}
