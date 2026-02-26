using UnityEngine;
using System.Collections;
// This script will contain all the On Draw effects for every card, ex: peek and what not.
namespace Systems
{
    public class DrawEffectManager : MonoBehaviour
    {   
        public CombatManager cm;
        public int PendingDamage, PendingDefense, PendingStrength, PendingPoison;
        public int AttackNum, AttackBonus; //for strength calculations
        public int DamageMult = 1, DefenseMult = 1, StrengthMult = 1, PoisonMult = 1; //
        public void ResolveOnDraw(CardData card)
        {
            bool shattered = ProcessMadness(card);
            if (!shattered) //do rest of stuff if not shattered
            {
                Debug.Log("drew " + card.cardName);
                CardData playedCard = ProcessSpecial(card);
                ProcessPendingStats(playedCard);
                ProcessPeeking(playedCard);
                Debug.Log("Madness " + cm.madness + 
                ", pending damage " + PendingDamage +  
                ", pending defense " + PendingDefense + 
                ", pending strength " + PendingStrength + 
                ", pending poison " + PendingPoison);

            }
            else //else reset stats...
            {
                Reset();
            }
            // Check if Alice.freeze is 0, if yes increment madness before resolving any other on draw card effects, else lower FreezeMadness by 1.            
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
        }

        private bool ProcessMadness(CardData card) //adds madness of card to player's madness
        {
            cm.madness += card.madness;

            return CheckMadness();
        }

        public bool CheckMadness() //checks if madness level is exceeded and shatters accordingly. 
        {
             if (cm.madness > cm.alice.maxMadness)
            {
                Debug.Log("Shatter!"); //return state. once is shatters need to also reset this and recalculate with just the card to keep.
                cm.StartCoroutine(DelayedShatter());
                return true;
                //ig also make sure turn logic is good
            }
            else
            {
                return false;
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
        private void ProcessPeeking(CardData card) //if card allows you to peek, trigger that
        {
            if (card.peek > 0)
            {
                Debug.Log("Peek");
                cm.PushNewState("Peeking");
            }
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
        private IEnumerator DelayedShatter()
        {
            yield return new WaitForEndOfFrame(); 
            cm.MoveToNewState("Shattering");
        }

    }
}

