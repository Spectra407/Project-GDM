using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data.SpecialEffects
{
    public static class EffectRegistry
    {
        //keeps track of all effects and their initialization in a dictionary
        //each effect thus has a static method that you can call to register a corresponding string (code) with its initialization (lambda function)
        //so, to parse out an effect from the string, can just call EffectRegistry.Create() to return the special effect which can be called later with execute
        //somewhere at startup, need to run static constructors:
        //var _ = typeof(MultiplierEffect);
        //can wrap in function
        private static Dictionary<string, Func<string[], SpecialEffect>> registry = new();

        public static void Register(string key, Func<string[], SpecialEffect> factory) //register a new effect
        {
            registry[key] = factory;
        }
        public static SpecialEffect Create(string key, string[] args) //create a new effect instance?
        {
            if (registry.TryGetValue(key, out var factory))
            {
                return factory(args);
            }
            else
            {
                Debug.Log("Could not get value for " + key);
                return null;
            }
        }
        public static CardData.CardType ParseType(string s)
        {
            switch (s)
            {
                case "POI":
                    return CardData.CardType.Poison;
                case "DMG":
                    return CardData.CardType.Damage;
                case "STR":
                    return CardData.CardType.Strength;
                case "DEF":
                    return CardData.CardType.Defense;
                case "PK":
                    return CardData.CardType.Peek;
                default:
                    return CardData.CardType.Other;
            }  
        }
    }
}