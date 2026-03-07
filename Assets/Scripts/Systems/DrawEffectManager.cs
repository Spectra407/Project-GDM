using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

// This script will contain all the On Draw effects for every card, ex: peek and what not.
namespace Systems
{
    public class DrawEffectManager : MonoBehaviour
    {   
        private bool isProcessing = false;
        public bool isreshuffling = false;
        public bool isRecalculating = false;
        

        public CombatManager cm;
        
        // [SerializeField] private GameObject calculatorPanel;
        [SerializeField] private TMP_Text damageText;
        [SerializeField] private TMP_Text blockText;
        [SerializeField] private TMP_Text poisonText;
        [SerializeField] private TMP_Text strengthText;
        
        
        public int AttackNum, AttackBonus; //for strength calculations
        //CHANGING FIELDS FOR PENDING, MULT, ETC
        public Dictionary<CardData.CardType, int> PendingStats = new()
        {
            { CardData.CardType.Damage, 0 },
            { CardData.CardType.Defense, 0 },
            { CardData.CardType.Strength, 0 },
            { CardData.CardType.Poison, 0 }
        };
        public Dictionary<CardData.CardType, int> MultStats = new()
        {
            {CardData.CardType.Damage, 1},
            {CardData.CardType.Defense, 1},
            {CardData.CardType.Strength, 1},
            {CardData.CardType.Poison, 1}
        };
        public Dictionary<CardData.CardType, int> DisStats = new()
        {
            {CardData.CardType.Damage, 1},
            {CardData.CardType.Defense, 1},
            {CardData.CardType.Strength, 1},
            {CardData.CardType.Poison, 1}
        };
        public Dictionary<CardData.CardType, int> PerStats = new()
        {
            { CardData.CardType.Damage, 0 },
            { CardData.CardType.Defense, 0 },
            { CardData.CardType.Strength, 0 },
            { CardData.CardType.Poison, 0 }
        };

        public void ResolveOnDraw(CardData card)
        {
            Debug.Log("drew " + card.cardName);
            if (cm.jackpot)
                card = card.getJackpot();

            CardData playedCard = ProcessSpecial(card);
    
            // If Copy returned a different card, process that card's special effect too
            if (playedCard != card)
            {
                if (!isRecalculating)
                    cm.lastDrawnCard = playedCard;
                playedCard = ProcessSpecial(playedCard); 
            }

            ProcessPendingStats(playedCard);
            UpdateUI();
        }

        public void ResetForJackpot() // Called after hitting Jackpot to recalculate all cards, does not reset Madness and Jackpot!
        {
            PendingStats[CardData.CardType.Damage] = 0;
            PendingStats[CardData.CardType.Defense] = 0;
            PendingStats[CardData.CardType.Poison] = 0;
            PendingStats[CardData.CardType.Strength] = 0;
            
            MultStats[CardData.CardType.Damage] = 1;
            MultStats[CardData.CardType.Defense] = 1;
            MultStats[CardData.CardType.Poison] = 1;
            MultStats[CardData.CardType.Strength] = 1;

            DisStats[CardData.CardType.Damage] = 1;
            DisStats[CardData.CardType.Defense] = 1;
            DisStats[CardData.CardType.Poison] = 1;
            DisStats[CardData.CardType.Strength] = 1;
            
            PerStats[CardData.CardType.Damage] = 0;
            PerStats[CardData.CardType.Defense] = 0;
            PerStats[CardData.CardType.Strength] = 0;
            PerStats[CardData.CardType.Poison] = 0;

            AttackBonus = 0;
            AttackNum = 0;

            damageText.gameObject.SetActive(false);
            blockText.gameObject.SetActive(false);
            poisonText.gameObject.SetActive(false);
            strengthText.gameObject.SetActive(false);
        }
        
        public void ResetForStand() // Called during ResolveOnStand in StandEffectManager and when you Shatter in HandlingCardState, resets everything properly
        {
            PendingStats[CardData.CardType.Damage] = 0;
            PendingStats[CardData.CardType.Defense] = 0;
            PendingStats[CardData.CardType.Poison] = 0;
            PendingStats[CardData.CardType.Strength] = 0;
            
            MultStats[CardData.CardType.Damage] = 1;
            MultStats[CardData.CardType.Defense] = 1;
            MultStats[CardData.CardType.Poison] = 1;
            MultStats[CardData.CardType.Strength] = 1;

            DisStats[CardData.CardType.Damage] = 1;
            DisStats[CardData.CardType.Defense] = 1;
            DisStats[CardData.CardType.Poison] = 1;
            DisStats[CardData.CardType.Strength] = 1;
            
            PerStats[CardData.CardType.Damage] = 0;
            PerStats[CardData.CardType.Defense] = 0;
            PerStats[CardData.CardType.Strength] = 0;
            PerStats[CardData.CardType.Poison] = 0;

            AttackBonus = 0;
            AttackNum = 0;

              
            cm.madness = 0;
            cm.jackpot = false;
            

            damageText.gameObject.SetActive(false);
            blockText.gameObject.SetActive(false);
            poisonText.gameObject.SetActive(false);
            strengthText.gameObject.SetActive(false);
        }

        public void ProcessJackpot()
        {
            if (isProcessing) return; // Exit if we are already in the middle of a recalculation
            isProcessing = true;

            if (cm.madness == cm.alice.maxMadness && !cm.jackpot) // Hit jackpot!
            {
                cm.jackpot = true;
                Debug.Log("You hit the jackpot!");

                ResetForJackpot();
                
                List<CardData> cards = cm.Hand.GetHandData();
                cards = cards.GetRange(0, cards.Count - 1);
                //iterate through hand, replacing all cards with jackpots and reprocess resolve on draw...
                //UNDER THE ASSUMPTION THIS IS ALL THAT CHANGES FOR CARDS. DOES NOT TAKE INTO ACCOUNT MADNESS, WEIRD EFFECTS THAT INTERUPT GAMEFLOW (DRAW ETC), SO ON
                isRecalculating = true;
                for (int i = 0; i < cards.Count; i++)
                {
                    cards[i] = cards[i].getJackpot();
                    ResolveOnDraw(cards[i]);
                }
                isRecalculating = false;
            }
            else if (cm.madness != cm.alice.maxMadness && cm.jackpot)   // Fell out of jackpot, lost jackpot status
            {
                cm.jackpot = false;
                Debug.Log("Fell out of jackpot :(");
                ResetForJackpot();

                List<CardData> cards = cm.Hand.GetHandData();
                cards = cards.GetRange(0, cards.Count - 1);
                //iterate through hand, replacing all cards with normal non-jackpot and reprocess resolve on draw...
                isRecalculating = true;
                for (int i = 0; i < cards.Count; i++)
                {
                    ResolveOnDraw(cards[i]); 
                }
                isRecalculating = false;
            }

            isProcessing = false;
            
        }
        public CardData ProcessSpecial(CardData card) //trigger special effects based on card ID
        {
            if (card.effect != null)
            {
                Debug.Log("Special effect processed");
                return card.effect.Execute(cm, card);
            }
            return card;
        }

        public void ProcessPendingStats(CardData card) //calculates what the new attack, defense, etc. will be now
        {
            ProcessPendingStrength(card);
            ProcessPendingPoison(card);
            ProcessPendingDamage(card);
            ProcessPendingDefense(card);
        }

        private void ProcessPendingDamage(CardData card)
        {
            PendingStats[CardData.CardType.Damage] += DisStats[CardData.CardType.Damage] * card.damage;

            if (DisStats[CardData.CardType.Damage] * card.damage > 0)
                AttackNum++;

            AttackBonus = AttackNum * (cm.strength + PendingStats[CardData.CardType.Strength]);
        }

        private void ProcessPendingDefense(CardData card)
        {
            PendingStats[CardData.CardType.Defense] += DisStats[CardData.CardType.Defense] * card.defense;
        }

        private void ProcessPendingStrength(CardData card)
        {
            PendingStats[CardData.CardType.Strength] += DisStats[CardData.CardType.Strength] * card.strength;
        }

        private void ProcessPendingPoison(CardData card)
        {
            PendingStats[CardData.CardType.Poison] += DisStats[CardData.CardType.Poison] * card.poison;
        }
        
        private void UpdateUI()
        {
            int handCount = cm.Hand.handCardViews.Count;
            int displayDamage = (PendingStats[CardData.CardType.Damage] + PerStats[CardData.CardType.Damage] * handCount) * MultStats[CardData.CardType.Damage];
            int displayDefense = (PendingStats[CardData.CardType.Defense] + PerStats[CardData.CardType.Defense] * handCount) * MultStats[CardData.CardType.Defense];
            int displayStrength = (PendingStats[CardData.CardType.Strength] + PerStats[CardData.CardType.Strength] * handCount) * MultStats[CardData.CardType.Strength];
            int displayPoison = (PendingStats[CardData.CardType.Poison] + PerStats[CardData.CardType.Poison] * handCount) * MultStats[CardData.CardType.Poison];


            Debug.Log("Madness " + cm.madness + 
                      ", pending damage " + displayDamage +  
                      ", pending defense " + displayDefense + 
                      ", pending strength " + displayStrength +
                      ", number of attacks " + AttackNum +
                      ", pending poison " + displayPoison);
            
            // UPDATE THE UI CALCULATOR ACCORDING TO THE NEW VALUES.
            damageText.text = displayDamage + "\\U00002694";
            blockText.text = displayDefense + "\\U0001F6E1";
            poisonText.text = displayPoison + "\\U0001F9EA";
            strengthText.text = displayStrength + "\\U0001F4AA";

            if (PendingStats[CardData.CardType.Damage] <= 0)
            {
                damageText.gameObject.SetActive(false);
            }
            else
            {
                damageText.gameObject.SetActive(true);
            }

            if (PendingStats[CardData.CardType.Defense] <= 0)
            {
                blockText.gameObject.SetActive(false);
            }
            else
            {
                blockText.gameObject.SetActive(true);
            }
            
            if (PendingStats[CardData.CardType.Poison] <= 0)
            {
                poisonText.gameObject.SetActive(false);
            }
            else
            {
                poisonText.gameObject.SetActive(true);
            }
            
            if (PendingStats[CardData.CardType.Strength] <= 0)
            {
                strengthText.gameObject.SetActive(false);
            }
            else
            {
                strengthText.gameObject.SetActive(true);
            }
        }


    }
}

