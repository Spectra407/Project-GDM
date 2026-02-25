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
        public void ResolveOnDraw(CardData card)
        {
            ProcessSpecial(card);
            ProcessPendingStats(card);
            Debug.Log("Madness " + cm.madness + 
            ", pending damage " + PendingDamage +  ", pending defense " + PendingDefense + ", pending strength " + PendingStrength + ", pending poison " + PendingPoison);
            
            
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
            AttackBonus = 0;
            AttackNum = 0;
            
            damageText.gameObject.SetActive(false);
            blockText.gameObject.SetActive(false);
            poisonText.gameObject.SetActive(false);
            strengthText.gameObject.SetActive(false);
        }

        private void ProcessSpecial(CardData card) //trigger special effects based on card ID
        {
            Debug.Log("Special effect processed: " + card.effect);
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
            PendingDamage += card.damage;
            AttackNum++;
            AttackBonus = AttackNum * (cm.strength + PendingStrength);
        }
        private void ProcessPendingDefense (CardData card)
        {
            PendingDefense += card.defense;   
        }
        private void ProcessPendingStrength(CardData card)
        {
            PendingStrength += card.strength;
        }
        private void ProcessPendingPoison(CardData card)
        {
            PendingPoison += card.poison;
        }
        

    }
}

