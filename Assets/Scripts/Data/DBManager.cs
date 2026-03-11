using System;
using System.Collections.Generic;
using System.IO;
using Data.SpecialEffects;
using Systems;
using UnityEngine;
using Object = UnityEngine.Object;

public class CardDB : MonoBehaviour
{
    public List<CardData> cards = new List<CardData>();
    public string file = "card-db.csv";
    
    public static CardDB Instance { get; private set; }
    void Awake() //wont destroy on load
    {
        transform.SetParent(null);  // Detach from the parent so that DontDestroyOnLoad can work even when we put it under a parent for cleanliness.
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void Start() //need it to not destroy on load
    {
        if (cards.Count > 0) return;    // Skip if we alrdy loaded before
            
        Debug.Log("Started up DB manager");
        LoadCards(file);
        Debug.Log("Finished loading DB");
        
    }
    public void LoadCards(string file) //load all cards from DB into a list of cards
    {
        string path = Path.Combine(Application.streamingAssetsPath, file);
        string[] lines =  File.ReadAllLines(path);

        for (int i = 1; i < lines.Length; i++)
        {
            CardData card = parseLine(lines[i]);
            cards.Add(card);
        }
    }
    
    private CardData parseLine(string line)
    {
        string[] fields = line.Split(',');

        int cardID = Int32.Parse(fields[0]); //might not be needed, redundant with index
        string cardName = fields[1];
        string description = fields[2];
        int jackpot = Int32.Parse(fields[3]); //maybe change to reference card itself later...
        List<CardData.CardType> cardType = new List<CardData.CardType>();
        cardType.Add((CardData.CardType)Enum.Parse(typeof(CardData.CardType), fields[4])); 
        int damage = Int32.Parse(fields[5]);
        int strength = Int32.Parse(fields[6]);
        int defense = Int32.Parse(fields[7]);
        int poison = Int32.Parse(fields[8]);
        int peek = Int32.Parse(fields[9]);
        int madness = Int32.Parse(fields[10]);

        //maybe use addressables later
        Sprite art = Resources.Load<Sprite>("CardArt/" + fields[11]); 

        SpecialEffect effect = parseEffect(fields[12]);

        CardData card = CardData.CreateCard(cardID, cardName, description, jackpot, cardType, damage, strength, defense, poison, peek, madness, art, effect);   
        return card;
    }
    
    private SpecialEffect parseEffect(string line) //uses effect registry to get special effect from string 
    {
        string[] fields = line.Split("_");
        if (!string.IsNullOrWhiteSpace(fields[0]))
        {
            SpecialEffect effect = EffectRegistry.Create(fields[0], fields[1..]); //need to figure out what to do if no additional args
            return effect;

        } 
        else return null;
    }
}
