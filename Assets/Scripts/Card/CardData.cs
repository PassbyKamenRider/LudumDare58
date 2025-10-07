using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "LD58/Card Data")]
public class CardData : ScriptableObject
{
    public string cardID;
    public string cardName;
    public Sprite cardSprite;
    public Sprite[] fakeCardSprites;
    public int cardCost;
    public int cardAtk;
    public int cardDef;
    public string cardType;
    public CardClass cardClass;
    public string cardDescription;
    public CardRarity cardRarity;
    public float cardPrice;
}

public enum CardRarity
{
    R, GR, PR
}

public enum CardClass
{
    Warrior, Mage, Wanderer, Priest, Bard, Druid
}