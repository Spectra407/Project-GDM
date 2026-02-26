using UnityEngine;
using System.Collections;
using TMPro;

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
        
        public int PendingDamage, PendingDefense, PendingStrength, PendingPoison;
        public int AttackNum, AttackBonus; //for strength calculations
        public int DamageMult = 1, DefenseMult = 1, StrengthMult = 1, PoisonMult = 1; //
        public void ResolveOnDraw(CardData card)
        {
            // bool shattered = ProcessMadness(card);
            // if (!shattered) //do rest of stuff if not shattered
            Debug.Log("drew " + card.cardName);
            CardData playedCard = ProcessSpecial(card);
            ProcessPendingStats(playedCard);
            ProcessPeeking(playedCard);
            Debug.Log("Madness " + cm.madness + 
            ", pending damage " + PendingDamage +  
            ", pending defense " + PendingDefense + 
            ", pending strength " + PendingStrength + 
            ", pending poison " + PendingPoison);

            // UDATE THE UI CALCULATOR ACCORDING TO THE NEW VALUES.
            damageText.text = PendingDamage + "\\U00002694";
            blockText.text = PendingDefense + "\\U0001F6E1";
            poisonText.text = PendingPoison + "\\U0001F9EA";
            strengthText.text = PendingStrength + "\\U0001F4AA";

            if (PendingDamage <= 0)
            {
                damageText.gameObject.SetActive(false);
            }
            else
            {
                damageText.gameObject.SetActive(true);
            }

            if (PendingDefense <= 0)
            {
                blockText.gameObject.SetActive(false);
            }
            else
            {
                blockText.gameObject.SetActive(true);
            }
            
            if (PendingPoison <= 0)
            {
                poisonText.gameObject.SetActive(false);
            }
            else
            {
                poisonText.gameObject.SetActive(true);
            }
            
            if (PendingStrength <= 0)
            {
                strengthText.gameObject.SetActive(false);
            }
            else
            {
                strengthText.gameObject.SetActive(true);
            }

            
            


        }

        public void Reset() //reset all pending stats, etc. should be called at resolvestand?
        {
            PendingDamage = 0;
            PendingDefense = 0;
            PendingStrength = 0;
            PendingPoison = 0;
            cm.madness = 0;

            AttackBonus = 0;
            AttackNum = 0;

            DamageMult = 1;
            DefenseMult = 1;
            StrengthMult = 1;
            PoisonMult = 1;
            
            damageText.gameObject.SetActive(false);
            blockText.gameObject.SetActive(false);
            poisonText.gameObject.SetActive(false);
            strengthText.gameObject.SetActive(false);
        }

        private void ProcessSpecial(CardData card) //trigger special effects based on card ID
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
            PendingDamage += DamageMult * card.damage;

            if (DamageMult * card.damage > 0) { //add to number attacks if this added damage
                AttackNum++;
            }

            AttackBonus = AttackNum * (cm.strength + PendingStrength);
        }
        private void ProcessPendingDefense (CardData card)
        {
            PendingDefense += DefenseMult * card.defense;   
        }
        private void ProcessPendingStrength(CardData card)
        {
            PendingStrength += StrengthMult * card.strength;
        }
        private void ProcessPendingPoison(CardData card)
        {
            PendingPoison += PoisonMult * card.poison;
        }
        

    }
}

