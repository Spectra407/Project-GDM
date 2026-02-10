using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New card", menuName = "CardData")]
public class CardData : ScriptableObject
{
    [Header("Card Identity")]
    public int cardID; //unique ID
    public string cardName;     //cardName will be used as and ID to trigger the card's effect.
    public string description;

    [Header("Card Type for Visual Changes")]
    public int jackpot; //cardID of jackpot version of card, -1 if NA
    public List<CardType> cardType;
     public enum CardType
    {
        Damage,
        Strength,
        Defense,
        Poison,
        Peek,
        Bomb,
        Other
    }
    
    [Header("Standard Stats")]
    public int damage;
    public int block;
    public int defense;
    public int poison;
    public int peek;
    
    [Header("Madness Cost")]
    public int madness;
    [Header("Sprite for the art")]
    public Sprite art;
    [Header("Special effects")]
    public int specialID; //ID associated with special effect, -1 if NA
    public int specialVal; //used by special effect when necessary

}
