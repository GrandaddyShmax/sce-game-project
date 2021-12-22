using System.Collections.Generic;
using UnityEngine;
using System.Linq;  //needed for ToList function

//responsible for creating and storing Storage zone, extension of ZoneBehaviour
public class Zone_Storage : Zone_Behaviour
{
    public GameObject PagePrefab;       //type of prefab for Page (attached via Inspector)
    [HideInInspector] public bool skipDay = false;      //for development testing
    [HideInInspector] private GameObject Page;
    [HideInInspector] private List<Data_Card> slots;    //what is in the storage
    [HideInInspector] public int count;     //amount of full slots in the storage
    [HideInInspector] public System_DayNight time;
    private Card_Pool Pool;

    public void Awake()
    {
        time = FindObjectOfType<System_DayNight>();
        Size = 8;   //max Zone size
        InstantiateZone();

        slots = new List<Data_Card>();
        count = 0;
        Pool = ScriptableObject.CreateInstance<Card_Pool>();
    }
    private void InstantiateZone()
    {
        Page = Instantiate(PagePrefab, this.transform);              //create and instantiate Page objects in scene
        Page.name = string.Format("Cards");                             //new Page name (for displaying in Scene)
        for (int i = count; i < this.Size; i++)
        {
            GameObject newCard = Instantiate(CardPrefab, Page.transform);   //create and instantiate objects in scene
            newCard.GetComponent<Card_Display>().ClearCard();
            newCard.name = string.Format("Slot {0}", i);                    //updates name in scene
            newCard.transform.localScale -= new Vector3((CardPrefab.transform.localScale.x) / 10, (CardPrefab.transform.localScale.y) / 10, 0);
            newCard.GetComponent<CanvasGroup>().alpha = .6f;
        }
    }
    public override void EventDrop(Card_Drag cardObject)
    {
        if (AddToStorage(cardObject.card))
        {
            GameObject placeholder = cardObject.placeholder;    //get the card's placeholder in the Hand to delete it
            Destroy(placeholder);
            Destroy(cardObject.gameObject);
        }
    }
    public void RefreshZone()   //clear/add cards to zone visually
    {
        slots = slots.OrderBy(Card => Card.code).ToList();
        Card_Display[] cards = Page.gameObject.transform.GetComponentsInChildren<Card_Display>();
        for (int i = 0; i < count; i++)
        {
            cards[i].AddCard(slots[i]);
            cards[i].gameObject.name = string.Format("{0} (Card)", slots[i].name);  //updates name in scene
            if (time.isDay == false || skipDay) 
                cards[i].gameObject.GetComponent<CanvasGroup>().alpha = 1f;
            else
                cards[i].gameObject.GetComponent<CanvasGroup>().alpha = .6f;
        }
        for (int i = count; i < this.Size; i++)
        {
            cards[i].ClearCard();
            cards[i].gameObject.GetComponent<CanvasGroup>().alpha = .6f;
        }
    }

    public bool AddToStorage(Data_Card newCard)//adds card to zone at night or if given by Unit
    {
        if ((count < Size))
        {
            slots.Add(newCard);
            slots.OrderBy(Card => Card.code);
            count++;
            RefreshZone();
            return true;
        }
        return false;
    }
    public void RemoveFromStorage(Card_Display pickedCard)      //removes card from storage on click
    {
        if (time.isDay == false || skipDay)
        {
            slots.Remove(pickedCard.card);
            count--;
            RefreshZone();
        }
    }
}


