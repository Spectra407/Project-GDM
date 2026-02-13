using UnityEngine;
//lets player choose if they would like to shuffle their whole hand back into their deck
public class ShuffleHandEffect : SpecialEffect
{
    public abstract void Execute(CombatManager cm, CardData card)
    {
        //pop up prompt to shuffle or not 
        //take input 
        bool shuffling = true; //for now

        if (shuffling)
        {
            Debug.Log("Shuffling hand...");
            //can look at shatter for similar, put all cards back into hand
            //reset DrawEffectManager
        }
    }
}
