using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class CardDB : MonoBehaviour
{
    public List<CardData> cards = new List<CardData>();
    public string file = "card-db.csv";

    public void Start() //need it to not destroy on load
    {
        Debug.Log("Started up DB manager");
        LoadCards(file);
        Debug.Log("Finished loading DB");
        

        for (int i = 0; i < cards.Count; i++)
        {
            CardData card = cards[i];
            Debug.Log("Cardname: " + card.cardName);
        }
        Debug.Log("Finished iterating DB");
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

            //card.art = fields[11]; 
            //need to change so it looks for file
            //placeholder art
            //maybe use addressables later
            card.art = Resources.Load<Sprite>("" + "sample-art");

            card.effect = (fields[12]);
            cards.Add(card);
        }
    }
}
