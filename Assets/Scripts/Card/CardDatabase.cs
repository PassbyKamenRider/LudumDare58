using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "LD58/Card Database")]
public class CardDatabase : ScriptableObject
{
    public List<CardData> realCards;

    public CardData GetRandomCardByRarity(CardRarity rarity)
    {
        var list = realCards.FindAll(c => c.cardRarity == rarity);
        if (list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }

    public TradeCard GetRandomFakeCard()
    {
        CardData baseCard = realCards[Random.Range(0, realCards.Count)];

        List<CardRarity> existingRarities = new List<CardRarity>();
        foreach (var card in realCards)
        {
            if (card.cardID == baseCard.cardID)
            {
                existingRarities.Add(card.cardRarity);
            }
        }

        TradeCard fakeCard = new TradeCard(baseCard, true, existingRarities);

        return fakeCard;
    }
}