
using UnityEngine;

public class CardDB : MonoBehaviour
{
    public List<CardData> cards = new LIst<CardData>();
    public void LoadCards(string filePath) //load all cards from DB into a list of cards
    {
        string[] lines = read all lines from file

        for (int i = 1; i < lines.length; i++)
        {
            string[] fields = lines[i].Split(',');

            CardData card = new CardData();

            card.cardID = fields[0]; //might not be needed, redundant with index
            card.cardName = fields[1];
            card.description = fields[2];
            card.jackpot = fields[3]; //maybe change to reference card itself later...
            card.cardType = fields[4]; //need to change it to enum
            
            card.damage = fields[5];
            card.block = fields[6];
            card.defense = fields[7];
            card.poison = fields[8];
            card.peek = fields[9];

            card.madness = fields[10];

            card.art = fields[11]; //need to change so it looks for file

            card.specialID = fields[12];
            card.specialVal = fields[13];
        }
    }
}
