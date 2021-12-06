using UnityEngine;
using UnityEngine.UI;

//responsible for switching panels and button related functions of Cards
public class Screen_Cards : MonoBehaviour
{
    public Text Message;
    public GameObject Hand;
    public GameObject Craft;
    public GameObject Storage;
    public GameObject Book;
    public GameObject Map;
    public GameObject destroyButton;
    public GameObject creativeButton;
    public GameObject storageButton;
    public GameObject cardsButton;

    [Header("Game State:")]
    public bool SkipLogin;  //for development testing
    private Zone_Hand zHand;
    private Zone_Craft zCraft;
    private Zone_Storage zStorage;
    private Zone_Book zBook;
    private Zone_Map zMap;
    [HideInInspector] public bool Placeable;
    [HideInInspector] public bool UIDown;

    void Start()    //initilizing in case something was off
    {
        zHand = Hand.transform.GetComponent<Zone_Hand>();
        zCraft = Craft.transform.GetComponent<Zone_Craft>();
        zStorage = Storage.transform.GetComponent<Zone_Storage>();
        zBook = Book.transform.GetComponent<Zone_Book>();
        zMap = Map.transform.GetComponent<Zone_Map>();

        CloseUI();
       
        if (SkipLogin || Screen_Login.IsLogin)  
        {
            creativeButton.SetActive(true);
        }
        else
        {
            creativeButton.SetActive(false);
        }
        storageButton.SetActive(true);
        cardsButton.SetActive(true);
}
    public void TopMessage(string text)
    {
        Message.gameObject.SetActive(true);
        Message.text = text;
    }
    public void SwitchCards()
    {
        if (UIDown)
        {
            OpenUI();
        }
        else
        {
            CloseUI();
        }
    }
    private void OpenUI()
    {
        Placeable = true;

        Hand.SetActive(true);
        Craft.SetActive(true);
        Map.SetActive(false);
        destroyButton.SetActive(true);
       
        UIDown = false;
    }
    private void CloseUI()
    {
        Message.gameObject.SetActive(false);
        Placeable = false;

        Hand.SetActive(false);
        Craft.SetActive(false);
        Storage.SetActive(false);
        Book.SetActive(false);
        Map.SetActive(false);
        destroyButton.SetActive(false);

        UIDown = true;
    }
    public void SwitchCreative()
    {
        
        if (Book.activeSelf)            
        {
            CloseBook();
        }
        else                             
        {
            OpenBook();
        }
    }
    private void OpenBook()
    {
        TopMessage("Creative game mode, click cards to add to your deck");
        Placeable = false;

        Craft.SetActive(false);
        Storage.SetActive(false);
        Book.SetActive(true);
        Hand.SetActive(true);
        destroyButton.SetActive(true);

        UIDown = false;
    }
    private void CloseBook()
    {
        Message.gameObject.SetActive(false);
        Placeable = true;

        zBook.FirstPage();
        Book.SetActive(false); 
        Craft.SetActive(true);
    }
    public void SwitchStorage()       //used when clicked on TownHall
    {
        if (Storage.activeSelf)
        {
            CloseStorage();
        }
        else
        {
            OpenStorage();
        }
    }
    public void OpenStorage()      
    {
        TopMessage("Town Storage");
        Placeable = false;

        Book.SetActive(false);
        Craft.SetActive(false);
        Storage.SetActive(true);
        Hand.SetActive(true);
        destroyButton.SetActive(true);

        UIDown = false;
    }
    public void CloseStorage()
    {
        Message.gameObject.SetActive(false);
        Placeable = true;

        Storage.SetActive(false);
        Craft.SetActive(true);
    }
    public void MoveFromHand(Card_Drag pickedCard)  //move card between Hand zone and Craft on click
    {
        if (Craft.activeSelf)
        {
            if (pickedCard.transform.parent == Hand.transform)  //if card is in hand - move to craft
            {
                if (Craft.transform.childCount < zCraft.Size)
                {
                    TopMessage("Attempting to craft! Pick another card to combine");
                    Placeable = false;
                    pickedCard.transform.SetParent(Craft.transform);
                    if (Craft.transform.childCount == zCraft.Size)
                    {
                        zCraft.EventClick();
                    }
                }
            }
            else     //if card is in craft - move to hand
            {
                CraftToHand(pickedCard);
            }
        }
        else if (Storage.activeSelf)
        {
            if (pickedCard.transform.parent == Hand.transform)  //if card is in hand - move to storage
            {
                if (zStorage.AddToStorage(pickedCard.card, false))
                {
                    Destroy(pickedCard.gameObject);
                }
            }
            else    //if card is in storage - move to hand
            {
                ClickToHand(pickedCard);
            }
        }
    }
    public void CraftToHand(Card_Drag card)         //move card from Craft zone to Hand on click
    {
        if (zCraft.success)
        {
            TopMessage(string.Format("Craft succeseful! Created {0}", card.card.name));
        }
        else
        {
            TopMessage("Uh oh, craft didn't succeed");
        }
        //card.gameObject.transform.SetParent(Craft.transform);
        //[insert timer]
        card.gameObject.transform.SetParent(Hand.transform);
        Placeable = true;
    }
    public void ClickToHand(Card_Display pickedCard)//adds card to hand based on what was picked in Book/Storage
    {
        if (CreateInHand(pickedCard.card))
        {
            if (Storage.activeSelf)
                zStorage.RemoveFromStorage(pickedCard);
        }
    }
    private bool CreateInHand(Data_Card pickedCard)
    {
        if (Hand.transform.childCount < zHand.Size)
        {
            GameObject newCard = Instantiate(zHand.CardPrefab, Hand.transform);     //create and instantiate object in scene
            newCard.GetComponent<Card_Drag>().AddCard(pickedCard);                  //add cards to objects 
            newCard.name = string.Format("{0}", pickedCard.name);                   //update new card name (for displaying in Scene)
            return true;
        }
        return false;
    }
    public bool AddGathered(Data_Card pickedCard)   //Add card from Unit to Hand/Storage
    {
        if (CreateInHand(pickedCard))   
        {
            return true;
        }
        else if (zStorage.count < zStorage.Size)
        {
            zStorage.AddToStorage(pickedCard, true);
            return true;
        }
        return false;
    }
}
