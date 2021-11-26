using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//stores the CardDisplays that appear in the Zones
public class CardDisplay : MonoBehaviour
{
    public Card card;
    public Text nameText;
    public Image artwork;

    public void AddCard(Card _card) //fill fields in card on screen according to card data
    {
        card = _card;
        nameText.text = card.name;
        artwork.sprite = card.artwork;
    }
}