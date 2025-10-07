using System.Collections.Generic;
using UnityEngine;

public class TradeCard
{
    // card attributes
    public CardData cardDataRef;
    public string cardID;
    public string cardName;
    public Sprite cardSprite;
    public Sprite[] fakeCardSprites;
    public int cardCost;
    public int cardAtk;
    public int cardDef;
    public CardClass cardClass;
    public string cardDescription;
    public CardRarity cardRarity;
    public float cardPrice;
    public bool isFake;
    public float patience;
    private List<CardRarity> existingRarities;

    public TradeCard(CardData baseCard, bool isFake, List<CardRarity> existingRarities)
    {
        cardDataRef = baseCard;
        cardID = baseCard.cardID;
        cardName = baseCard.cardName;
        cardSprite = baseCard.cardSprite;
        fakeCardSprites = baseCard.fakeCardSprites;
        cardCost = baseCard.cardCost;
        cardAtk = baseCard.cardAtk;
        cardDef = baseCard.cardDef;
        cardClass = baseCard.cardClass;
        cardDescription = baseCard.cardDescription;
        cardRarity = baseCard.cardRarity;
        cardPrice = baseCard.cardPrice;
        patience = GetPatience();
        this.existingRarities = existingRarities;

        if (isFake) ApplyFakeAttributes();

        this.isFake = isFake;
    }

    public void ApplyFakeAttributes()
    {
        List<string> attrs = new List<string> { "ID", "Sprite", "Cost", "Atk", "Def", "Class", "Rarity" };

        if (existingRarities.Count >= 3)
        {
            attrs.Remove("Rarity");
        }

        int index1 = Random.Range(0, attrs.Count);
        int index2;
        do
        {
            index2 = Random.Range(0, attrs.Count);
        } while (index2 == index1);

        string[] selected = { attrs[index1], attrs[index2] };
        // Debug.Log($"Fake Card Info: ID={cardID}, Name={cardName}, Cost={cardCost}, Atk={cardAtk}, Def={cardDef}, Class={cardClass}, Rarity={cardRarity}");
        // Debug.Log($"{attrs[index1]} and {attrs[index2]} are fake");

        foreach (var attr in selected)
        {
            switch (attr)
            {
                case "ID":
                    cardID = GetRandomCardID(cardID);
                    break;
                case "Sprite":
                    cardSprite = fakeCardSprites[Random.Range(0, fakeCardSprites.Length)];
                    break;
                case "Cost":
                    cardCost = GetFakeValue(1, 5, 1, cardCost);
                    break;
                case "Atk":
                    cardAtk = GetFakeValue(1500, 4000, 100, cardAtk);
                    break;
                case "Def":
                    cardDef = GetFakeValue(1500, 4000, 100, cardDef);
                    break;
                case "Class":
                    cardClass = GetRandomClass(cardClass);
                    break;
                case "Rarity":
                    cardRarity = GetRandomRarity(cardRarity);
                    break;
            }
        }
    }

    private int GetFakeValue(int min, int max, int step, int original)
    {
        List<int> possibleValues = new List<int>();
        for (int v = min; v <= max; v += step)
        {
            if (v != original)
                possibleValues.Add(v);
        }

        int index = Random.Range(0, possibleValues.Count);
        return possibleValues[index];
    }

    private CardClass GetRandomClass(CardClass original)
    {
        int originalInt = (int)original;
        int enumCount = System.Enum.GetValues(typeof(CardClass)).Length;

        int newInt;
        do
        {
            newInt = Random.Range(0, enumCount);
        } while (newInt == originalInt);
        return (CardClass)newInt;
    }

    private CardRarity GetRandomRarity(CardRarity original)
    {
        CardRarity[] allRarities = (CardRarity[])System.Enum.GetValues(typeof(CardRarity));
        CardRarity newRarity;
        do
        {
            newRarity = allRarities[Random.Range(0, allRarities.Length)];
        } while (existingRarities.Contains(newRarity));
        return newRarity;
    }

    private string GetRandomCardID(string originalID)
    {
        string[] uniqueIDs = new string[]
        {
            "CW01","DW01","FM01","PW01","CM01","PM01",
            "CWA01","FWA01","RWA01","RD01","PD01","BP01","BB01"
        };

        string newID;
        do
        {
            int index = Random.Range(0, uniqueIDs.Length);
            newID = uniqueIDs[index];
        } while (newID == originalID);

        return newID;
    }

    private float GetPatience()
    {
        switch (cardRarity)
        {
            case CardRarity.R:
                return 30f;
            case CardRarity.GR:
                return 25f;
            case CardRarity.PR:
                return 18f;
            default:
                return -1f;
        }
    }
}
