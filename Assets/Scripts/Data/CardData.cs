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
    public int strength;
    public int defense;
    public int poison;
    public int peek;
    
    [Header("Madness Cost")]
    public int madness;
    [Header("Sprite for the art")]
    public Sprite art;
    [Header("Special effects")]
    //will turn it into SpecialEffect class soon
    public string effect;
    //public SpecialEffect effect; 
    //need to create dictionary (maybe in db manager??) to associate each string in excel field with a special effect and parse out other shit
    //lowkey cardview and stuff fo

    public CardData CreateCard(int ID, string name, string desc, int jp, List<CardType> type, int dmg, int str, int def, int poi, int pk, int mad, Sprite cArt, SpecialEffect eff)
    {
        CardData card = ScriptableObject.CreateInstance<CardData>();

        card.cardID = ID;
        card.cardName = name;
        card.description = desc;
        card.jackpot = jp;
        card.cardType = type;
        card.damage = dmg;
        card.strength = str;
        card.defense = def;
        card.poison = poi;
        card.peek = pk;
        card.madness = mad;
        card.art = cArt;
        card.effect = eff;

        return card;
    }
}
