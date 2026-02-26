using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

// This script will contain all the On Draw effects for every card, ex: peek and what not.
namespace Systems
{
    public class DrawEffectManager : MonoBehaviour
    {   
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
            {CardData.CardType.Damage, 0},
            {CardData.CardType.Defense, 0},
            {CardData.CardType.Strength, 0},
            {CardData.CardType.Poison, 0}
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

        public void ResolveOnDraw(CardData card)
        {
            Debug.Log("drew " + card.cardName);
            if (cm.jackpot) //if currently jackpot and drew jackpot card, replace with jackpot card
            {
                card = card.getJackpot();
            }

            CardData playedCard = ProcessSpecial(card);
            ProcessPendingStats(playedCard);
            
            UpdateUI();
        }

        public void Reset() //reset all pending stats, etc. should be called at resolvestand?
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

            AttackBonus = 0;
            AttackNum = 0;
            cm.madness = 0;
            // cm.jackpot = false;
            
            damageText.gameObject.SetActive(false);
            blockText.gameObject.SetActive(false);
            poisonText.gameObject.SetActive(false);
            strengthText.gameObject.SetActive(false);
        }

        public void ProcessJackpot()
        {
            if (cm.jackpot) return;

            if (cm.madness == cm.alice.maxMadness && !cm.jackpot) // jackpot!
            {
                cm.jackpot = true;
                Debug.Log("You hit the jackpot!");

                Reset();
                
                List<CardData> cards = cm.Hand.GetHandData();
                //iterate through hand, replacing all cards with jackpots and reprocess resolve on draw...
                //UNDER THE ASSUMPTION THIS IS ALL THAT CHANGES FOR CARDS. DOES NOT TAKE INTO ACCOUNT MADNESS, WEIRD EFFECTS THAT INTERUPT GAMEFLOW (DRAW ETC), SO ON
                for (int i = 0; i < cards.Count; i++)
                {
                    cards[i] = cards[i].getJackpot();
                    ResolveOnDraw(cards[i]);
                }
                //need logic for if player falls out of jackpot...
            }
        }
        private CardData ProcessSpecial(CardData card) //trigger special effects based on card ID
        {
            if (card.effect != null)
            {
                Debug.Log("Special effect processed");
                return card.effect.Execute(cm, card);
            }
            return card;
            
        }
        private void ProcessPendingStats(CardData card) //calculates what the new attack, defense, etc. will be now
        {   
            ProcessPendingStrength(card);
            ProcessPendingPoison(card);
            ProcessPendingDamage(card);
            ProcessPendingDefense(card);
        }
        
        private void ProcessPendingDamage(CardData card) //deals with damage bonus too
        {
            PendingStats[CardData.CardType.Damage] += DisStats[CardData.CardType.Damage] * card.damage;

            if (DisStats[CardData.CardType.Damage] * card.damage > 0) { //add to number attacks if this added damage
                AttackNum++;
            }

            AttackBonus = AttackNum * (cm.strength + PendingStats[CardData.CardType.Strength]);
        }
        private void ProcessPendingDefense (CardData card)
        {
            PendingStats[CardData.CardType.Defense] += DisStats[CardData.CardType.Defense] * card.defense;   
            //what about losing defense...
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
            Debug.Log("Madness " + cm.madness + 
            ", pending damage " + PendingStats[CardData.CardType.Damage] +  
            ", pending defense " + PendingStats[CardData.CardType.Defense] + 
            ", pending strength " + PendingStats[CardData.CardType.Strength] + 
            ", number of attacks " + AttackNum +
            ", pending poison " + PendingStats[CardData.CardType.Poison]);

            // UDATE THE UI CALCULATOR ACCORDING TO THE NEW VALUES.
            damageText.text = PendingStats[CardData.CardType.Damage] + "\\U00002694";
            blockText.text = PendingStats[CardData.CardType.Defense] + "\\U0001F6E1";
            poisonText.text = PendingStats[CardData.CardType.Poison] + "\\U0001F9EA";
            strengthText.text = PendingStats[CardData.CardType.Strength] + "\\U0001F4AA";

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

