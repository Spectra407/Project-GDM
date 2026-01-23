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
    public List<CardType> cardType;
    public enum CardType
    {
        Damage,
        Defense,
        Poison,
        Peek,
        Jackpot
    }
    
    [Header("Standard Stats")]
    public int damage;
    public int defense;
    public int poison;
    public int peek;
    
    [Header("Madness Cost")]
    public int madness;

    public void ChangeBanner()
    {
        // Check and change the first banner to either Damage or Peek
        // Check and change the second banner to either Defense or Poison
        // THIS FUNCTION SHOULD PROBABLY BE IN CardView ACTUALLY!
    }
    
    // Define functions later on in a separate EffectManager for all the cards.
    // ExecuteOnDraw and ExecuteOnStand functions in the EffectManager or CombatManager.


}
