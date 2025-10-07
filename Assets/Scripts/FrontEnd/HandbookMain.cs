using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HandbookMain : MonoBehaviour
{
    [SerializeField]
    private CardDatabase cardDatabase;
    [SerializeField]
    private GameObject cardPrefab;
    [SerializeField]
    private GameObject gridView;

    [SerializeField]
    private Dictionary<CardData, UICard> cardDataToUICardMap;
    private string currRarityFilter = "All";
    private string currClassFilter = "All";

    public UnityEvent OnCardsRefreshed;

    [ContextMenu("Populate Cards")]
    public void PopulateCards()
    {
        foreach (CardData cardData in cardDatabase.realCards)
        {
            GameObject cardObj = Instantiate(cardPrefab, gridView.transform);
            UICard uiCard = cardObj.GetComponent<UICard>();
            uiCard.SetCardData(cardData);
        }
    }

    void OnEnable()
    {
        currRarityFilter = "All";
        currClassFilter = "All";
        if (cardDataToUICardMap == null) { return; }
        RefreshCards();
    }

    void Start()
    {
        cardDataToUICardMap = new Dictionary<CardData, UICard>();
        foreach (CardData cardData in cardDatabase.realCards)
        {
            GameObject cardObj = Instantiate(cardPrefab, gridView.transform);
            UICard uiCard = cardObj.GetComponent<UICard>();
            uiCard.SetCardData(cardData);
            cardDataToUICardMap[cardData] = uiCard;
        }
    }


    public void RefreshCards()
    {
        foreach (var kvp in cardDataToUICardMap)
        {
            kvp.Value.gameObject.SetActive(false);
        }

        foreach (var kvp in cardDataToUICardMap)
        {
            CardData cardData = kvp.Key;
            UICard uiCard = kvp.Value;
            if (cardData.cardRarity.ToString() != currRarityFilter && currRarityFilter != "All")
            {
                continue;
            }
            if (cardData.cardClass.ToString() != currClassFilter && currClassFilter != "All")
            {
                continue;
            }
            uiCard.gameObject.SetActive(true);
        }
        OnCardsRefreshed.Invoke();
    }

    public void FilterOnRarity(int rarityIndex)
    {
        currRarityFilter = rarityIndex > 0 ? ((CardRarity)(rarityIndex-1)).ToString() : "All";
        RefreshCards();
    }

    public void FilterOnClass(int classIndex)
    {
        currClassFilter = classIndex > 0 ? ((CardClass)(classIndex - 1)).ToString() : "All";
        RefreshCards();
    }

}
