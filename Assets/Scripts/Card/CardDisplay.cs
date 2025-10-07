using TMPro;
using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    private Animator animator;
    private TradeCard currentCard;
    [SerializeField]
    private UICard uICard;

    void OnEnable()
    {
        animator = GetComponent<Animator>();
        uICard = GetComponentInChildren<UICard>();
    }

    public void SetCard(TradeCard card)
    {
        currentCard = card;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (currentCard != null && uICard != null)
        {
            uICard.SetCardData(currentCard);
        }
    }

    public void CardExit()
    {
        animator.Play("card_exit");
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void OnCardPlay()
    {
        AudioManager.Instance.PlayCardAudio();
    }
}