using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New card", menuName = "CardData")]
public class CardData : ScriptableObject
{
    [Header("Card Identity")]
    public string cardName;     // cardName will be used as and ID to trigger the card's effect.
    public string description;

    [Header("Card Type for Visual Changes")]
    public bool jackpot;
    public List<CardType> cardType;
    public enum CardType
    {
        Damage,
        Defense,
        Poison,
        Peek
    }
    
    [Header("Standard Stats")]
    public int damage;
    public int defense;
    public int poison;
    public int peek;
    
    [Header("Madness Cost")]
    public int madness;
    
    [Header("Sprite for the art")]
    public Sprite art;

}
