using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class PurchaseRecord
{
    public TradeCard card;
    public float income;
}

[System.Serializable]
public class TrueCardSchedule
{
    public int rCount;
    public int grCount;
    public int prCount;
}

public class CardMerchant : MonoBehaviour
{
    [Header("Card Data Settings")]
    [SerializeField] CardDatabase cardDatabase;
    [SerializeField] PlayerCardCollection playerCollection;
    [SerializeField] int queueSize = 5;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] int[] cardPrices;
    [SerializeField] float[] cardPriceMultipliers;
    [SerializeField] List<TrueCardSchedule> schedule;
    [Header("Customer Display")]
    [SerializeField] TextMeshProUGUI patienceText;
    [SerializeField] Image patienceIcon;
    [SerializeField] Sprite[] patienceSprites;
    [SerializeField] GameObject endOfDayCanvas;
    [SerializeField] TextMeshProUGUI dayText;
    [SerializeField] TextMeshProUGUI balanceText;
    [SerializeField] TextMeshProUGUI cardPriceText;
    [SerializeField] GameObject buyOptions;
    [SerializeField] GameObject pcOutlineSprite;
    [SerializeField] GameObject databaseCanvas;
    [Header("End Day UI")]
    [SerializeField] private TextMeshProUGUI[] purchaseCardNameTexts;
    [SerializeField] private TextMeshProUGUI[] purchaseCardPriceTexts;
    [SerializeField] TextMeshProUGUI totalIncomeText;
    [SerializeField] TextMeshProUGUI currentBalanceText;
    [SerializeField] TextMeshProUGUI finalBalanceText;
    private List<TradeCard> remainingCards = new();
    private int currentCardIdx = 0;
    private float patienceTimer = 0f;
    private float currentPatienceMax = 0f;
    private int currentDay = 0;
    private const int totalDays = 5;
    private GameObject currentCardObj;
    private List<PurchaseRecord> dailyRecords = new List<PurchaseRecord>();
    private float dailyIncome = 0f;
    private float playerBalance = 400f;
    private bool isPatiencePaused = false;

    void Start()
    {
        UpdateBalanceDisplay();
        GenerateCardsForDay();
        ShowNextCard();
    }

    void Update()
    {
        if (currentCardIdx < remainingCards.Count && !isPatiencePaused)
        {
            patienceTimer -= Time.deltaTime;
            if (patienceTimer <= 0f)
            {
                AudioManager.Instance.PlayFAudio();
                OnSkipCard();
            }

            UpdatePatienceText();
        }
    }

    public void GenerateCardsForDay()
    {
        TrueCardSchedule today = schedule[currentDay];
        remainingCards.Clear();

        remainingCards.AddRange(GenerateTrueCards(today.rCount, CardRarity.R));
        remainingCards.AddRange(GenerateTrueCards(today.grCount, CardRarity.GR));
        remainingCards.AddRange(GenerateTrueCards(today.prCount, CardRarity.PR));

        int remaining = queueSize - remainingCards.Count;
        for (int i = 0; i < remaining; i++)
        {
            remainingCards.Add(GenerateFakeCard());
        }

        remainingCards = remainingCards.OrderBy(x => Random.value).ToList();
    }

    List<TradeCard> GenerateTrueCards(int count, CardRarity rarity)
    {
        List<TradeCard> result = new List<TradeCard>();
        for (int i = 0; i < count; i++)
        {
            CardData data = cardDatabase.GetRandomCardByRarity(rarity);
            TradeCard card = new TradeCard(data, false, null);
            result.Add(card);
        }
        return result;
    }

    TradeCard GenerateFakeCard()
    {
        TradeCard card = cardDatabase.GetRandomFakeCard();
        return card;
    }

    private void ShowNextCard()
    {
        if (currentCardIdx >= remainingCards.Count)
        {
            Debug.Log("Cards sold out, end the day");
            EndDay();
            return;
        }

        TradeCard card = remainingCards[currentCardIdx];
        patienceTimer = card.patience;
        currentPatienceMax = card.patience;
        UpdatePatienceText();
        buyOptions.SetActive(true);

        //Debug.Log($"This card is {(card.isFake ? "Fake" : "Real")}");

        cardPriceText.text = "Buy -$" + cardPrices[(int)card.cardRarity].ToString();

        GameObject cardObj = Instantiate(cardPrefab);
        cardObj.GetComponent<CardDisplay>().SetCard(card);
        currentCardObj = cardObj;
    }

    private IEnumerator DelayShowNextCard(float delay)
    {
        isPatiencePaused = true;

        if (currentCardObj != null)
        {
            currentCardObj.GetComponent<CardDisplay>().CardExit();
        }

        yield return new WaitForSeconds(delay);

        ShowNextCard();
        isPatiencePaused = false;
    }

    private void UpdatePatienceText()
    {
        if (currentPatienceMax <= 0f) return;
        float percent = Mathf.Clamp01(patienceTimer / currentPatienceMax);
        patienceText.text = $"{(percent * 100f):0}%";

        if (percent > 2.0f / 3.0f)
        {
            patienceIcon.sprite = patienceSprites[0];
            patienceIcon.color = Color.green;
        }
        else if (percent > 1.0f / 3.0f)
        {
            patienceIcon.sprite = patienceSprites[1];
            patienceIcon.color = Color.yellow;
        }
        else
        {
            patienceIcon.sprite = patienceSprites[2];
            patienceIcon.color = Color.red;
        }
    }

    private void UpdateBalanceDisplay()
    {
        balanceText.text = "$" + playerBalance.ToString() + "/1500";
    }

    public void OnBuyCard()
    {
        TradeCard currentCard = remainingCards[currentCardIdx];
        if (playerBalance < cardPrices[(int)currentCard.cardRarity])
        {
            Debug.Log("Not enough money!");
            return;
        }

        AudioManager.Instance.PlayDealAudio();
        playerBalance -= cardPrices[(int)currentCard.cardRarity];
        UpdateBalanceDisplay();
        buyOptions.SetActive(false);

        if (currentCard.isFake)
        {
            dailyRecords.Add(new PurchaseRecord
            {
                card = currentCard,
                income = 0
            });
        }
        else
        {
            int rarity = (int)currentCard.cardRarity;
            dailyRecords.Add(new PurchaseRecord
            {
                card = currentCard,
                income = cardPrices[rarity] * cardPriceMultipliers[rarity]
            });

            dailyIncome += cardPrices[rarity] * cardPriceMultipliers[rarity];
            playerCollection.AddCard(currentCard.cardDataRef);
        }

        currentCardIdx++;
        StartCoroutine(DelayShowNextCard(2f));
    }


    public void OnSkipCard()
    {
        buyOptions.SetActive(false);
        TradeCard currentCard = remainingCards[currentCardIdx];

        Debug.Log($"Skipped {currentCard.cardName}");
        currentCardIdx++;
        StartCoroutine(DelayShowNextCard(2f));
    }

    public void StartDay()
    {
        // TODO: process end
        if (playerBalance <= 0)
        {
            SceneManager.LoadScene("EndScene1");
            return;
        }

        if (currentDay >= totalDays)
        {
            if (playerBalance >= 1500)
            {
                SceneManager.LoadScene("EndScene2");
            }
            else
            {
                SceneManager.LoadScene("EndScene1");
            }
            return;
        }

        endOfDayCanvas.SetActive(false);
        UpdateBalanceDisplay();
        currentCardIdx = 0;
        GenerateCardsForDay();
        StartCoroutine(DelayShowNextCard(2f));
    }

    public void EndDay()
    {
        endOfDayCanvas.SetActive(true);

        Destroy(currentCardObj);

        for (int i = 0; i < purchaseCardNameTexts.Length; i++)
        {
            purchaseCardNameTexts[i].text = "";
            purchaseCardPriceTexts[i].text = "";
        }

        for (int i = 0; i < dailyRecords.Count; i++)
        {
            TradeCard card = dailyRecords[i].card;
            purchaseCardNameTexts[i].text = $"{card.cardName}({card.cardRarity})";
            purchaseCardPriceTexts[i].text = dailyRecords[i].income > 0 ? $"+${dailyRecords[i].income}" : "Fake!";
        }

        totalIncomeText.text = $"{(dailyIncome > 0 ? "+" : "-")}${Mathf.Abs(dailyIncome)}";
        currentBalanceText.text = $"${playerBalance}";
        finalBalanceText.text = $"{(dailyIncome + playerBalance > 0 ? "" : "-")}${Mathf.Abs(dailyIncome + playerBalance)}";

        playerBalance += dailyIncome;
        dailyIncome = 0f;
        dailyRecords.Clear();

        currentDay++;
        dayText.text = $"Day {currentDay + 1}/{totalDays}";
    }

    public void OnPCHover(bool isEnabled)
    {
        pcOutlineSprite.SetActive(isEnabled);
    }

    public void OnDatabaseEnter()
    {
        AudioManager.Instance.PlayClickAudio();
        databaseCanvas.SetActive(true);
    }

    public void OnButtonClick()
    {
        AudioManager.Instance.PlayClickAudio();
    }
}