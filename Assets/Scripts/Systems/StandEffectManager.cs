using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data.SpecialEffects;

namespace Systems
{
    public class StandEffectManager : MonoBehaviour
    {
        public CombatManager cm;

        private struct CardSnapshot
        {
            public CardView cardView;
            public int displayDamage;
            public int displayDefense;
            public int displayStrength;
            public int displayPoison;
            public bool isBomb;
        }

        public IEnumerator ResolveOnStandAnimated()
        {
            List<CardView> cards = new List<CardView>(cm.Hand.handCardViews);
            int handCount = cards.Count;

            cm.dem.HideAllUI();

            // Sort cards by priority
            List<CardView> pass1Cards = cards
                .Where(c => GetCardPriority(c, cards) < 6)
                .OrderBy(c => GetCardPriority(c, cards))
                .ToList();

            List<CardView> pass2Cards = cards
                .Where(c => GetCardPriority(c, cards) >= 6)
                .OrderBy(c => GetCardPriority(c, cards)) // 6 = normal, 7 = bombs, 8 = EqualEffect
                .ToList();

            // Sim dictionaries for display only
            var simPending = new Dictionary<CardData.CardType, int>
            {
                { CardData.CardType.Damage, 0 }, { CardData.CardType.Defense, 0 },
                { CardData.CardType.Strength, 0 }, { CardData.CardType.Poison, 0 }
            };
            var simMult = new Dictionary<CardData.CardType, int>
            {
                { CardData.CardType.Damage, 1 }, { CardData.CardType.Defense, 1 },
                { CardData.CardType.Strength, 1 }, { CardData.CardType.Poison, 1 }
            };
            var simDis = new Dictionary<CardData.CardType, int>
            {
                { CardData.CardType.Damage, 1 }, { CardData.CardType.Defense, 1 },
                { CardData.CardType.Strength, 1 }, { CardData.CardType.Poison, 1 }
            };
            var simPer = new Dictionary<CardData.CardType, int>
            {
                { CardData.CardType.Damage, 0 }, { CardData.CardType.Defense, 0 },
                { CardData.CardType.Strength, 0 }, { CardData.CardType.Poison, 0 }
            };
            var simEqual = new Dictionary<(CardData.CardType, CardData.CardType), bool>();
            int simAttackNum = 0;
            int simAttackBonus = 0;

            // PASS 1: Modifier cards, apply real effects immediately
            foreach (CardView cardView in pass1Cards)
            {
                if (cardView?.data == null) continue;
                CardData data = cm.jackpot ? cardView.data.getJackpot() : cardView.data;
                bool isBomb = data.cardType.Contains(CardData.CardType.Bomb);

                yield return cm.StartCoroutine(cardView.ShakeAndHighlight(isBomb, () =>
                {
                    if (isBomb)
                    {
                        if (data.strength > 0) AudioManager.instance.PlayGainStrength();
                        else if (data.defense != 0) AudioManager.instance.PlayGainShield();
                    }
                    else
                    {
                        if (data.damage != 0) AudioManager.instance.PlayTakeDamage();
                        else if (data.defense != 0) AudioManager.instance.PlayGainShield();
                        else if (data.strength != 0) AudioManager.instance.PlayGainStrength();
                        else if (data.poison != 0) AudioManager.instance.PlayPoisonDamage();
                        else if (data.effect is MultiplierEffect) AudioManager.instance.PlayGainStrength();
                        else if (data.effect is PerEffect) AudioManager.instance.PlayBluntDamage();
                        else if (data.effect is DisableEffect) AudioManager.instance.PlayGainShield();
                    }
                }));
                
                // Apply modifier effect to both sim and real state
                ApplySpecialToSim(data, cards, cards.IndexOf(cardView),
                    simPending, simMult, simDis, simPer, simEqual, ref simAttackNum);
                ApplySpecialToReal(data, cardView);

                // Apply base stats to sim and real game state
                if (!isBomb)
                {
                    // Sim
                    simPending[CardData.CardType.Damage] += simDis[CardData.CardType.Damage] * data.damage;
                    if (simDis[CardData.CardType.Damage] * data.damage > 0) simAttackNum++;
                    simPending[CardData.CardType.Defense] += simDis[CardData.CardType.Defense] * data.defense;
                    simPending[CardData.CardType.Defense] = Mathf.Max(0, simPending[CardData.CardType.Defense]);
                    simPending[CardData.CardType.Strength] += simDis[CardData.CardType.Strength] * data.strength;
                    simPending[CardData.CardType.Poison] += simDis[CardData.CardType.Poison] * data.poison;

                    // Real
                    if (data.strength != 0)
                    {
                        // Apply MultStats to this card's own strength if it has a multiplier effect
                        int mult = cm.dem.MultStats[CardData.CardType.Strength];
                        cm.strength += data.strength * mult;
                        cm.strength = Mathf.Max(0, cm.strength);
                        
                        CombatAnimator.Instance.PlayStrengthEffect(cardView, cm.strength);
                    }
                    if (data.defense != 0)
                    {
                        cm.tempDefense += data.defense;
                        cm.tempDefense = Mathf.Max(0, cm.tempDefense);
                        
                        CombatAnimator.Instance.PlayDefenseEffect(cardView, cm.tempDefense);
                    }
                    if (data.poison != 0)
                    {
                        cm.poison += data.poison;
                        cm.poison = Mathf.Max(0, cm.poison);
                        
                        CombatAnimator.Instance.PlayPoisonEffect(cardView, cm.poison);
                    }
                    if (data.damage != 0)
                    {
                        int cardDamage = cm.dem.DisStats[CardData.CardType.Damage] * data.damage;
                        int mult = cm.dem.MultStats[CardData.CardType.Damage];
                        if (cardDamage > 0)
                            ApplyDamage((cardDamage * mult) + cm.strength, cardView);
                    }
                }
                else
                {
                    simPending[CardData.CardType.Defense] += simDis[CardData.CardType.Defense] * data.defense;
                    simPending[CardData.CardType.Defense] = Mathf.Max(0, simPending[CardData.CardType.Defense]);
                    if (data.strength > 0)
                    {
                        cm.enemyStrength += data.strength;
                        Debug.Log($"Bomb gave enemy {data.strength} strength!");
                        
                        CombatAnimator.Instance.PlayEnemyStrengthEffect(cm.enemyStrength);
                    }
                    if (data.defense != 0)
                    {
                        cm.tempDefense += data.defense;
                        cm.tempDefense = Mathf.Max(0, cm.tempDefense);
                        
                        CombatAnimator.Instance.PlayDefenseEffect(cardView, cm.tempDefense);
                    }
                }

                

                simAttackBonus = simAttackNum * (cm.strength + simPending[CardData.CardType.Strength]);
                UpdateDisplayFromSim(simPending, simMult, simDis, simPer, simEqual, simAttackBonus, handCount);

                yield return new WaitForSeconds(0.15f);
            }

            // PASS 2: Stat cards, apply real effects individually
            foreach (CardView cardView in pass2Cards)
            {
                if (cardView?.data == null) continue;
                CardData data = cm.jackpot ? cardView.data.getJackpot() : cardView.data;
                bool isBomb = data.cardType.Contains(CardData.CardType.Bomb);

                yield return cm.StartCoroutine(cardView.ShakeAndHighlight(isBomb, () =>
                {
                    if (isBomb)
                    {
                        if (data.strength > 0) AudioManager.instance.PlayGainStrength();
                        else if (data.defense != 0) AudioManager.instance.PlayGainShield();
                    }
                    else
                    {
                        if (data.damage != 0) AudioManager.instance.PlayTakeDamage();
                        else if (data.defense != 0) AudioManager.instance.PlayGainShield();
                        else if (data.poison != 0) AudioManager.instance.PlayPoisonDamage();
                        else if (data.effect is EqualEffect) AudioManager.instance.PlayBluntDamage();
                        else if (data.effect is CopyEffect) AudioManager.instance.PlayBluntDamage();
                    }
                }));

                if (!isBomb)
                {
                    // Apply base stats to sim first
                    simPending[CardData.CardType.Damage] += simDis[CardData.CardType.Damage] * data.damage;
                    if (simDis[CardData.CardType.Damage] * data.damage > 0) simAttackNum++;
                    simPending[CardData.CardType.Defense] += simDis[CardData.CardType.Defense] * data.defense;
                    simPending[CardData.CardType.Defense] = Mathf.Max(0, simPending[CardData.CardType.Defense]);
                    simPending[CardData.CardType.Poison] += simDis[CardData.CardType.Poison] * data.poison;

                    // Apply base stats to real game state
                    int cardDamage = cm.dem.DisStats[CardData.CardType.Damage] * data.damage;
                    if (cardDamage > 0)
                        ApplyDamage(cardDamage + cm.strength, cardView); // pass cardView here
                    if (data.defense != 0)
                    {
                        cm.tempDefense += data.defense;
                        cm.tempDefense = Mathf.Max(0, cm.tempDefense);
                        
                        CombatAnimator.Instance.PlayDefenseEffect(cardView, cm.tempDefense);
                    }
                    if (data.poison != 0)
                    {
                        cm.poison += data.poison;
                        cm.poison = Mathf.Max(0, cm.poison);
                        
                        CombatAnimator.Instance.PlayPoisonEffect(cardView, cm.poison);
                    }

                    // Apply special effect
                    ApplySpecialToSim(data, cards, cards.IndexOf(cardView),
                        simPending, simMult, simDis, simPer, simEqual, ref simAttackNum);
                    ApplySpecialToReal(data, cardView);
                }
                else
                {
                    simPending[CardData.CardType.Defense] += simDis[CardData.CardType.Defense] * data.defense;
                    simPending[CardData.CardType.Defense] = Mathf.Max(0, simPending[CardData.CardType.Defense]);
                    if (data.strength > 0)
                    {
                        cm.enemyStrength += data.strength;
                        Debug.Log($"Bomb gave enemy {data.strength} strength!");
                        
                        CombatAnimator.Instance.PlayEnemyStrengthEffect(cm.enemyStrength);
                    }
                    if (data.defense != 0)
                    {
                        cm.tempDefense += data.defense;
                        cm.tempDefense = Mathf.Max(0, cm.tempDefense);
                        
                        CombatAnimator.Instance.PlayDefenseEffect(cardView, cm.tempDefense);
                    }

                    ApplySpecialToSim(data, cards, cards.IndexOf(cardView),
                        simPending, simMult, simDis, simPer, simEqual, ref simAttackNum);
                    ApplySpecialToReal(data, cardView);
                }

                simAttackBonus = simAttackNum * (cm.strength + simPending[CardData.CardType.Strength]);
                UpdateDisplayFromSim(simPending, simMult, simDis, simPer, simEqual, simAttackBonus, handCount);

                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.4f);
            
            cm.dem.ResetForStand();
        }

        // Applies modifier effects to real game state (MultStats, DisStats, PerStats etc.)
        private void ApplySpecialToReal(CardData data, CardView sourceCard = null)
        {
            if (data.effect == null) return;

            if (data.effect is MultiplierEffect mult)
            {
                var multField = typeof(MultiplierEffect).GetField("multiplier",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var typeField = typeof(MultiplierEffect).GetField("cardtype",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (multField != null && typeField != null)
                {
                    int multiplier = (int)multField.GetValue(mult);
                    CardData.CardType cardtype = (CardData.CardType)typeField.GetValue(mult);
                    cm.dem.MultStats[cardtype] *= multiplier;
                    if (cm.dem.DisStats[CardData.CardType.Damage] > 0 &&
                        cardtype == CardData.CardType.Damage)
                        cm.dem.AttackNum++;
                }
            }
            else if (data.effect is PerEffect per)
            {
                var multField = typeof(PerEffect).GetField("percard_multiplier",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var typeField = typeof(PerEffect).GetField("cardtype",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (multField != null && typeField != null)
                {
                    int perMult = (int)multField.GetValue(per);
                    CardData.CardType cardtype = (CardData.CardType)typeField.GetValue(per);
                    cm.dem.PerStats[cardtype] += perMult * cm.dem.DisStats[cardtype];

                    // Actually apply the per-card contribution to real game state
                    int handCount = cm.Hand.handCardViews.Count;
                    int perTotal = perMult * cm.dem.DisStats[cardtype] * handCount;

                    // PerEffect
                    if (cardtype == CardData.CardType.Damage && perTotal > 0)
                    {
                        ApplyDamage(perTotal + cm.strength, sourceCard);
                    }
                    else if (cardtype == CardData.CardType.Defense && perTotal > 0)
                    {
                        cm.tempDefense += perTotal;
                        cm.tempDefense = Mathf.Max(0, cm.tempDefense);
                        
                        CombatAnimator.Instance.PlayDefenseEffect(sourceCard, cm.tempDefense);
                    }
                    else if (cardtype == CardData.CardType.Poison)
                    {
                        cm.poison += perTotal;
                        cm.poison = Mathf.Max(0, cm.poison);
                        
                        CombatAnimator.Instance.PlayPoisonEffect(sourceCard, cm.poison);
                    }
                    else if (cardtype == CardData.CardType.Strength)
                    {
                        cm.strength += perTotal;
                        cm.strength = Mathf.Max(0, cm.strength);
                        
                        CombatAnimator.Instance.PlayStrengthEffect(sourceCard, cm.strength);
                    }

                    if (cm.dem.DisStats[CardData.CardType.Damage] > 0 &&
                        cardtype == CardData.CardType.Damage)
                        cm.dem.AttackNum++;
                }
            }
            else if (data.effect is DisableEffect dis)
            {
                var typeField = typeof(DisableEffect).GetField("cardType",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (typeField != null)
                {
                    CardData.CardType cardtype = (CardData.CardType)typeField.GetValue(dis);
                    cm.dem.DisStats[cardtype] = 0;
                    cm.dem.PendingStats[cardtype] = 0;
                }
            }
            else if (data.effect is EqualEffect eq)
            {
                var type1Field = typeof(EqualEffect).GetField("type1",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var type2Field = typeof(EqualEffect).GetField("type2",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (type1Field != null && type2Field != null)
                {
                    CardData.CardType t1 = (CardData.CardType)type1Field.GetValue(eq);
                    CardData.CardType t2 = (CardData.CardType)type2Field.GetValue(eq);
                    cm.dem.EqualStats[(t1, t2)] = true;
                    if (cm.dem.DisStats[CardData.CardType.Damage] > 0 &&
                        t1 == CardData.CardType.Damage)
                        cm.dem.AttackNum++;
                    // Apply equal damage immediately
                    int equalBonus = 0;
                    if (t2 == CardData.CardType.Defense)
                        equalBonus = Mathf.Max(0, cm.tempDefense);
                    else if (t2 == CardData.CardType.Strength)
                        equalBonus = Mathf.Max(0, cm.strength);
                    else if (t2 == CardData.CardType.Poison)
                        equalBonus = Mathf.Max(0, cm.poison);

                    // EqualEffect
                    if (t1 == CardData.CardType.Damage && equalBonus > 0)
                        ApplyDamage(equalBonus + cm.strength, sourceCard);
                    else if (t1 == CardData.CardType.Defense && equalBonus > 0)
                    {
                        cm.tempDefense += equalBonus;
                        
                        CombatAnimator.Instance.PlayDefenseEffect(sourceCard, cm.tempDefense);
                    }
                }
            }
            else if (data.effect is CopyEffect)
            {
                // Find the cardView for this data in the hand to get its index
                var allCards = cm.Hand.handCardViews;
                int cardIndex = allCards.FindIndex(cv => cv?.data?.InstanceID == data.InstanceID);
                if (cardIndex <= 0) return;

                CardData prev = null;
                for (int i = cardIndex - 1; i >= 0; i--)
                {
                    CardData candidate = allCards[i]?.data;
                    if (candidate == null) continue;
                    if (cm.jackpot) candidate = candidate.getJackpot();
                    if (candidate.effect is CopyEffect) continue;
                    if (candidate.cardType.Contains(CardData.CardType.Bomb)) return;
                    prev = candidate;
                    break;
                }

                if (prev == null) return;

                // Apply the copied card's special effect to real state
                ApplySpecialToReal(prev, sourceCard);

                // Apply the copied card's base stats to real state
                if (prev.damage != 0)
                {
                    int cardDamage = cm.dem.DisStats[CardData.CardType.Damage] * prev.damage;
                    if (cardDamage > 0)
                        ApplyDamage(cardDamage + cm.strength, sourceCard);
                }
                if (prev.defense != 0)
                {
                    cm.tempDefense += prev.defense;
                    cm.tempDefense = Mathf.Max(0, cm.tempDefense);
                    
                    CombatAnimator.Instance.PlayDefenseEffect(sourceCard, cm.tempDefense);
                }
                if (prev.strength != 0)
                {
                    cm.strength += prev.strength;
                    cm.strength = Mathf.Max(0, cm.strength);
                    
                    CombatAnimator.Instance.PlayStrengthEffect(sourceCard, cm.strength);
                }
                if (prev.poison != 0)
                {
                    cm.poison += prev.poison;
                    cm.poison = Mathf.Max(0, cm.poison);
                    
                    CombatAnimator.Instance.PlayPoisonEffect(sourceCard, cm.poison);
                }
            }
        }

        private void ApplyDamage(int damage, CardView sourceCard = null)
        {
            if (damage <= 0) return;
            if (cm.enemyDefense >= damage)
            {
                cm.enemyDefense -= damage;
                
            }
            else
            {
                damage -= cm.enemyDefense;
                cm.enemyDefense = 0;
                cm.enemyCurrentHealth -= damage;
                
            }
            cm.enemyCurrentHealth = Mathf.Max(0, cm.enemyCurrentHealth);
            EnemyHealthBar.Instance.AnimateToCurrentHealth();

            // Fire projectile toward enemy portrait world position
            if (sourceCard != null)
            {
                Vector3 enemyWorldPos = GetPortraitWorldPos(false);
                CombatAnimator.Instance.PlayDamageEffect(sourceCard, enemyWorldPos, cm.enemyCurrentHealth);
            }

            Debug.Log("Enemy health: " + cm.enemyCurrentHealth);
        }

        private Vector3 GetPortraitWorldPos(bool isAlice)
        {
            return isAlice ? new Vector3(-20f, 5f, 0f) : new Vector3(4f, 5f, 0f);
        }
        

        private void UpdateDisplayFromSim(
            Dictionary<CardData.CardType, int> simPending,
            Dictionary<CardData.CardType, int> simMult,
            Dictionary<CardData.CardType, int> simDis,
            Dictionary<CardData.CardType, int> simPer,
            Dictionary<(CardData.CardType, CardData.CardType), bool> simEqual,
            int simAttackBonus,
            int handCount)
        {
            int eqDmg = GetSimEqualBonus(CardData.CardType.Damage, simEqual, simPending);
            int eqDef = GetSimEqualBonus(CardData.CardType.Defense, simEqual, simPending);
            int eqStr = GetSimEqualBonus(CardData.CardType.Strength, simEqual, simPending);
            int eqPoi = GetSimEqualBonus(CardData.CardType.Poison, simEqual, simPending);

            cm.dem.UpdateDamageUI(((simPending[CardData.CardType.Damage] + eqDmg
                + simPer[CardData.CardType.Damage] * handCount)
                * simMult[CardData.CardType.Damage]) + simAttackBonus);

            cm.dem.UpdateDefenseUI((simPending[CardData.CardType.Defense] + eqDef
                + simPer[CardData.CardType.Defense] * handCount)
                * simMult[CardData.CardType.Defense]);

            cm.dem.UpdateStrengthUI((simPending[CardData.CardType.Strength] + eqStr
                + simPer[CardData.CardType.Strength] * handCount)
                * simMult[CardData.CardType.Strength]);

            cm.dem.UpdatePoisonUI((simPending[CardData.CardType.Poison] + eqPoi
                + simPer[CardData.CardType.Poison] * handCount)
                * simMult[CardData.CardType.Poison]);
        }

        private void ApplySpecialToSim(
            CardData data,
            List<CardView> cards,
            int cardIndex,
            Dictionary<CardData.CardType, int> simPending,
            Dictionary<CardData.CardType, int> simMult,
            Dictionary<CardData.CardType, int> simDis,
            Dictionary<CardData.CardType, int> simPer,
            Dictionary<(CardData.CardType, CardData.CardType), bool> simEqual,
            ref int simAttackNum)
        {
            if (data.effect == null) return;

            if (data.effect is MultiplierEffect mult)
            {
                var multField = typeof(MultiplierEffect).GetField("multiplier",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var typeField = typeof(MultiplierEffect).GetField("cardtype",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (multField != null && typeField != null)
                {
                    int multiplier = (int)multField.GetValue(mult);
                    CardData.CardType cardtype = (CardData.CardType)typeField.GetValue(mult);
                    simMult[cardtype] *= multiplier;
                    if (simDis[CardData.CardType.Damage] > 0 &&
                        cardtype == CardData.CardType.Damage)
                        simAttackNum++;
                }
            }
            else if (data.effect is PerEffect per)
            {
                var multField = typeof(PerEffect).GetField("percard_multiplier",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var typeField = typeof(PerEffect).GetField("cardtype",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (multField != null && typeField != null)
                {
                    int perMult = (int)multField.GetValue(per);
                    CardData.CardType cardtype = (CardData.CardType)typeField.GetValue(per);
                    simPer[cardtype] += perMult * simDis[cardtype];
                    if (simDis[CardData.CardType.Damage] > 0 &&
                        cardtype == CardData.CardType.Damage)
                        simAttackNum++;
                }
            }
            else if (data.effect is DisableEffect dis)
            {
                var typeField = typeof(DisableEffect).GetField("cardType",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (typeField != null)
                {
                    CardData.CardType cardtype = (CardData.CardType)typeField.GetValue(dis);
                    simDis[cardtype] = 0;
                    simPending[cardtype] = 0;
                }
            }
            else if (data.effect is EqualEffect eq)
            {
                var type1Field = typeof(EqualEffect).GetField("type1",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var type2Field = typeof(EqualEffect).GetField("type2",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (type1Field != null && type2Field != null)
                {
                    CardData.CardType t1 = (CardData.CardType)type1Field.GetValue(eq);
                    CardData.CardType t2 = (CardData.CardType)type2Field.GetValue(eq);
                    simEqual[(t1, t2)] = true;
                    if (simDis[CardData.CardType.Damage] > 0 &&
                        t1 == CardData.CardType.Damage)
                        simAttackNum++;
                }
            }
            else if (data.effect is CopyEffect)
            {
                if (cardIndex > 0 && cards[cardIndex - 1]?.data != null)
                {
                    CardData prev = ResolveCopy(cards, cardIndex);
                    if (prev != null && !prev.cardType.Contains(CardData.CardType.Bomb))
                    {
                        ApplySpecialToSim(prev, cards, cardIndex - 1,
                            simPending, simMult, simDis, simPer, simEqual, ref simAttackNum);
                        simPending[CardData.CardType.Damage] +=
                            simDis[CardData.CardType.Damage] * prev.damage;
                        if (simDis[CardData.CardType.Damage] * prev.damage > 0)
                            simAttackNum++;
                        simPending[CardData.CardType.Defense] +=
                            simDis[CardData.CardType.Defense] * prev.defense;
                        simPending[CardData.CardType.Defense] =
                            Mathf.Max(0, simPending[CardData.CardType.Defense]);
                        simPending[CardData.CardType.Poison] +=
                            simDis[CardData.CardType.Poison] * prev.poison;
                    }
                }
            }
        }

        private int GetCardPriority(CardView cardView, List<CardView> allCards)
        {
            if (cardView?.data == null) return 6;
            CardData data = cm.jackpot ? cardView.data.getJackpot() : cardView.data;

            if (data.effect is CopyEffect)
            {
                int index = allCards.IndexOf(cardView);
                CardData resolved = ResolveCopy(allCards, index);
                if (resolved != null)
                    return GetPriorityFromData(resolved, allCards, index - 1);
                return 6;
            }

            return GetPriorityFromData(data, allCards, allCards.IndexOf(cardView));
        }

        private int GetPriorityFromData(CardData data, List<CardView> allCards, int index)
        {
            // EqualEffect must always go last, never let stat priority override it
            if (data.effect is EqualEffect) return 8;
    
            int effectPriority = GetEffectPriority(data);
            int statPriority = GetStatPriority(data);
            return Mathf.Min(effectPriority, statPriority);
        }

        private int GetEffectPriority(CardData data)
        {
            if (data.effect == null) return 99;
            if (data.effect is MultiplierEffect mult)
            {
                var typeField = typeof(MultiplierEffect).GetField("cardtype",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (typeField != null)
                {
                    CardData.CardType cardtype = (CardData.CardType)typeField.GetValue(mult);
                    return cardtype == CardData.CardType.Strength ? 1 : 4;
                }
            }
            if (data.effect is DisableEffect) return 2;
            if (data.effect is PerEffect) return 5;
            if (data.effect is EqualEffect) return 8; // after bombs
            return 99;
        }

        private int GetStatPriority(CardData data)
        {
            if (data.cardType.Contains(CardData.CardType.Bomb)) return 7; // before EqualEffect
            if (data.strength > 0) return 3;
            return 6;
        }

        private CardData ResolveCopy(List<CardView> allCards, int copyIndex)
        {
            for (int i = copyIndex - 1; i >= 0; i--)
            {
                CardData prev = allCards[i]?.data;
                if (prev == null) continue;
                if (cm.jackpot) prev = prev.getJackpot();
                if (prev.effect is CopyEffect) continue;
                if (prev.cardType.Contains(CardData.CardType.Bomb)) return null;
                return prev;
            }
            return null;
        }

        private int GetSimEqualBonus(
            CardData.CardType type1,
            Dictionary<(CardData.CardType, CardData.CardType), bool> simEqual,
            Dictionary<CardData.CardType, int> simPending)
        {
            int bonus = 0;
            foreach (var kvp in simEqual)
                if (kvp.Key.Item1 == type1)
                    bonus += Mathf.Max(0, simPending[kvp.Key.Item2]);
            return bonus;
        }
    }
}