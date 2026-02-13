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
        //drawn acrds to add too??
        public void ResolveOnDraw()
        {
            bool shattered = ProcessMadness();
            if (!shattered) //do rest of stuff if not shattered
            {
                ProcessSpecial();
                ProcessPendingStats();
                ProcessPeeking();
                Debug.Log("Madness " + cm.madness + 
                ", pending damage " + PendingDamage +  ", pending defense " + PendingDefense + ", pending strength " + PendingStrength + ", pending poison " + PendingPoison);

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
            AttackBonus = 0;
            AttackNum = 0;
        }

        private bool ProcessMadness() //adds madness of card to player's madness
        {
            cm.madness += cm.lastDrawnCard.madness;
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
        private void ProcessSpecial() //trigger special effects based on card ID
        {
            Debug.Log("Special effect processed: " + cm.lastDrawnCard.specialID);
        }
        private void ProcessPendingStats() //calculates what the new attack, defense, etc. will be now
        {
            ProcessPendingStrength();
            ProcessPendingPoison();
            ProcessPendingDamage();
            ProcessPendingDefense();
        }
        private void ProcessPeeking() //if card allows you to peek, trigger that
        {
            if (cm.lastDrawnCard.peek > 0)
            {
                Debug.Log("Peek");
                cm.PushNewState("Peeking");
            }
        }
        private void ProcessPendingDamage() //deals with damage bonus too
        {
            PendingDamage += cm.lastDrawnCard.damage;
            AttackNum++;
            AttackBonus = AttackNum * (cm.strength + PendingStrength);
        }
        private void ProcessPendingDefense ()
        {
            PendingDefense += cm.lastDrawnCard.defense;   
        }
        private void ProcessPendingStrength()
        {
            PendingStrength += cm.lastDrawnCard.strength;
        }
        private void ProcessPendingPoison()
        {
            PendingPoison += cm.lastDrawnCard.poison;
        }
         private IEnumerator DelayedShatter()
        {
            yield return new WaitForEndOfFrame(); 
            cm.MoveToNewState("Shattering");
        }

    }
}

