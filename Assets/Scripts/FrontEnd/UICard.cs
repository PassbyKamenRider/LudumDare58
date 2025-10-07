using UnityEngine;

public class UICard : MonoBehaviour
{
    [SerializeField]
    private CardData cardData;
    [SerializeField]
    private TMPro.TextMeshProUGUI cardNameText;
    [SerializeField]
    private TMPro.TextMeshProUGUI cardSubText;
    [SerializeField]
    UnityEngine.UI.Image cardTypeImage;
    [SerializeField]
    private TMPro.TextMeshProUGUI cardCostText;
    [SerializeField]
    private TMPro.TextMeshProUGUI cardAtkText;
    [SerializeField]
    private TMPro.TextMeshProUGUI cardDefText;
    [SerializeField]
    private TMPro.TextMeshProUGUI cardDescriptionText;
    [SerializeField]
    private UnityEngine.UI.Image cardImage;
    [SerializeField]
    private TMPro.TextMeshProUGUI cardRarityText;
    [SerializeField]
    private TMPro.TextMeshProUGUI cardIDText;

    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color goldenColor;
    [SerializeField]
    private GameObject parallelVFX;

    public void SetCardData(CardData data)
    {
        cardData = data;
        string fullName = data.cardName;
        string[] nameParts = fullName.Split(',');
        cardNameText.text = nameParts[0].Trim();
        cardSubText.text = nameParts[1].Trim();
        CardClass cardClass = data.cardClass;
        Sprite classSprite = Resources.Load<Sprite>($"KeyedIcons/{cardClass.ToString()}");
        if (classSprite != null && cardTypeImage != null)
        {
            cardTypeImage.sprite = classSprite;
        }
        cardCostText.text = data.cardCost.ToString();
        cardAtkText.text = data.cardAtk.ToString();
        cardDefText.text = data.cardDef.ToString();
        cardDescriptionText.text = data.cardDescription;
        cardImage.sprite = data.cardSprite;
        cardIDText.text = data.cardID;
        cardRarityText.text = data.cardRarity.ToString();

        if (data.cardRarity != CardRarity.GR)
        {
            cardNameText.color = normalColor;
            cardSubText.color = normalColor;
        }
        else
        {
            cardNameText.color = goldenColor;
            cardSubText.color = goldenColor;
        }

        if (data.cardRarity == CardRarity.PR)
        {
            parallelVFX.SetActive(true);
        }
    }

    public void SetCardData(TradeCard data)
    {
        cardData = data.cardDataRef;
        string fullName = data.cardName;
        string[] nameParts = fullName.Split(',');
        cardNameText.text = nameParts[0].Trim();
        cardSubText.text = nameParts[1].Trim();
        CardClass cardClass = data.cardClass;
        Sprite classSprite = Resources.Load<Sprite>($"KeyedIcons/{cardClass.ToString()}");
        if (classSprite != null && cardTypeImage != null)
        {
            cardTypeImage.sprite = classSprite;
        }
        cardCostText.text = data.cardCost.ToString();
        cardAtkText.text = data.cardAtk.ToString();
        cardDefText.text = data.cardDef.ToString();
        cardDescriptionText.text = data.cardDescription;
        cardImage.sprite = data.cardSprite;
        cardIDText.text = data.cardID;
        cardRarityText.text = data.cardRarity.ToString();

        if (data.cardRarity != CardRarity.GR)
        {
            cardNameText.color = normalColor;
            cardSubText.color = normalColor;
        }
        else
        {
            cardNameText.color = goldenColor;
            cardSubText.color = goldenColor;
        }

        if (data.cardRarity == CardRarity.PR)
        {
            parallelVFX.SetActive(true);
        }
    }
}
