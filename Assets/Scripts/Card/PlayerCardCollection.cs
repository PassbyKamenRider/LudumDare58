using System.Collections.Generic;
using UnityEngine;

public class PlayerCardCollection : MonoBehaviour
{
    private Dictionary<CardData, int> ownedCards = new Dictionary<CardData, int>();

    public void AddCard(CardData card)
    {
        if (ownedCards.ContainsKey(card))
        {
            ownedCards[card] += 1;
        }
        else
        {
            ownedCards[card] = 1;
        }
    }

    public Dictionary<CardData, int> GetAllCards()
    {
        return ownedCards;
    }
}