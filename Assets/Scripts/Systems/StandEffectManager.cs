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

            // Sort all cards by priority
            List<CardView> pass1Cards = cards
                .Where(c => GetCardPriority(c, cards) < 6)
                .OrderBy(c => GetCardPriority(c, cards))
                .ToList();

            List<CardView> pass2Cards = cards
                .Where(c => GetCardPriority(c, cards) >= 6)
                .OrderBy(c => GetCardPriority(c, cards)) // 6 = normal, 7 = EqualEffect, 8 = bombs
                .ToList();

            // Build isolated sim dictionaries
            var simPending = new Dictionary<CardData.CardType, int>
            {
                { CardData.CardType.Damage, 0 },
                { CardData.CardType.Defense, 0 },
                { CardData.CardType.Strength, 0 },
                { CardData.CardType.Poison, 0 }
            };
            var simMult = new Dictionary<CardData.CardType, int>
            {
                { CardData.CardType.Damage, 1 },
                { CardData.CardType.Defense, 1 },
                { CardData.CardType.Strength, 1 },
                { CardData.CardType.Poison, 1 }
            };
            var simDis = new Dictionary<CardData.CardType, int>
            {
                { CardData.CardType.Damage, 1 },
                { CardData.CardType.Defense, 1 },
                { CardData.CardType.Strength, 1 },
                { CardData.CardType.Poison, 1 }
            };
            var simPer = new Dictionary<CardData.CardType, int>
            {
                { CardData.CardType.Damage, 0 },
                { CardData.CardType.Defense, 0 },
                { CardData.CardType.Strength, 0 },
                { CardData.CardType.Poison, 0 }
            };
            var simEqual = new Dictionary<(CardData.CardType, CardData.CardType), bool>();
            int simAttackNum = 0;
            int simAttackBonus = 0;

            // --- PASS 1: Modifier cards animate first ---
            foreach (CardView cardView in pass1Cards)
            {
                if (cardView?.data == null) continue;
                CardData data = cm.jackpot ? cardView.data.getJackpot() : cardView.data;
                bool isBomb = data.cardType.Contains(CardData.CardType.Bomb);

                yield return cm.StartCoroutine(cardView.ShakeAndHighlight(isBomb));

                // Apply base stats first
                if (!isBomb)
                {
                    simPending[CardData.CardType.Damage] += simDis[CardData.CardType.Damage] * data.damage;
                    if (simDis[CardData.CardType.Damage] * data.damage > 0) simAttackNum++;
                    simPending[CardData.CardType.Defense] += simDis[CardData.CardType.Defense] * data.defense;
                    simPending[CardData.CardType.Strength] += simDis[CardData.CardType.Strength] * data.strength;
                    simPending[CardData.CardType.Poison] += simDis[CardData.CardType.Poison] * data.poison;
                }
                else
                {
                    simPending[CardData.CardType.Defense] += simDis[CardData.CardType.Defense] * data.defense;
                    simPending[CardData.CardType.Defense] = Mathf.Max(0, simPending[CardData.CardType.Defense]);
                }

                // Then apply the modifier effect
                ApplySpecialToSim(data, cards, cards.IndexOf(cardView),
                    simPending, simMult, simDis, simPer, simEqual, ref simAttackNum);

                simAttackBonus = simAttackNum * (cm.strength + simPending[CardData.CardType.Strength]);

                UpdateDisplayFromSim(simPending, simMult, simDis, simPer, simEqual,
                    simAttackBonus, handCount);

                yield return new WaitForSeconds(0.15f);
            }

            // --- PASS 2: Stat cards animate with modifiers already active ---
            foreach (CardView cardView in pass2Cards)
            {
                if (cardView?.data == null) continue;
                CardData data = cm.jackpot ? cardView.data.getJackpot() : cardView.data;
                bool isBomb = data.cardType.Contains(CardData.CardType.Bomb);

                yield return cm.StartCoroutine(cardView.ShakeAndHighlight(isBomb));

                // Apply special effect if any (EqualEffect, CopyEffect etc.)
                ApplySpecialToSim(data, cards, cards.IndexOf(cardView),
                    simPending, simMult, simDis, simPer, simEqual, ref simAttackNum);

                // Apply base stats
                if (!isBomb)
                {
                    simPending[CardData.CardType.Damage] +=
                        simDis[CardData.CardType.Damage] * data.damage;
                    if (simDis[CardData.CardType.Damage] * data.damage > 0) simAttackNum++;
                    simPending[CardData.CardType.Defense] +=
                        simDis[CardData.CardType.Defense] * data.defense;
                    simPending[CardData.CardType.Poison] +=
                        simDis[CardData.CardType.Poison] * data.poison;
                }
                else
                {
                    simPending[CardData.CardType.Defense] += simDis[CardData.CardType.Defense] * data.defense;
                    simPending[CardData.CardType.Defense] = Mathf.Max(0, simPending[CardData.CardType.Defense]);
                }

                simAttackBonus = simAttackNum *
                    (cm.strength + simPending[CardData.CardType.Strength]);

                // Update display after this card
                UpdateDisplayFromSim(simPending, simMult, simDis, simPer, simEqual,
                    simAttackBonus, handCount);

                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.4f);

            // --- PHASE 3: Apply real final values (original logic untouched) ---
            int equalDamage = cm.dem.GetEqualBonus(CardData.CardType.Damage);
            int equalDefense = cm.dem.GetEqualBonus(CardData.CardType.Defense);
            int equalStrength = cm.dem.GetEqualBonus(CardData.CardType.Strength);
            int equalPoison = cm.dem.GetEqualBonus(CardData.CardType.Poison);

            int finalStrength = (cm.dem.PendingStats[CardData.CardType.Strength] + equalStrength
                                 + cm.dem.PerStats[CardData.CardType.Strength] * handCount)
                                * cm.dem.MultStats[CardData.CardType.Strength];

            int finalPoison = (cm.dem.PendingStats[CardData.CardType.Poison] + equalPoison
                               + cm.dem.PerStats[CardData.CardType.Poison] * handCount)
                              * cm.dem.MultStats[CardData.CardType.Poison];

            int finalDefense = (cm.dem.PendingStats[CardData.CardType.Defense] + equalDefense
                                + cm.dem.PerStats[CardData.CardType.Defense] * handCount)
                               * cm.dem.MultStats[CardData.CardType.Defense];

            int finalDamage = ((cm.dem.PendingStats[CardData.CardType.Damage] + equalDamage
                                + cm.dem.PerStats[CardData.CardType.Damage] * handCount)
                               * cm.dem.MultStats[CardData.CardType.Damage]
                               + cm.dem.AttackBonus);

            int bombStrength = 0;
            foreach (var cardView in cards)
            {
                if (cardView?.data != null &&
                    cardView.data.cardType.Contains(CardData.CardType.Bomb))
                    bombStrength += cardView.data.strength;
            }
            if (bombStrength > 0)
            {
                cm.enemyStrength += bombStrength;
                Debug.Log($"Bomb cards gave {bombStrength} strength to the enemy!");
            }

            UpdateStrength(finalStrength);
            UpdatePoison(finalPoison);
            UpdateDefense(Mathf.Max(0, finalDefense));

            if (cm.poison > 0)
            {
                cm.enemyCurrentHealth -= cm.poison;
                cm.enemyCurrentHealth = Mathf.Max(0, cm.enemyCurrentHealth);
                Debug.Log($"Poison dealt {cm.poison} damage ignoring block.");
            }

            UpdateDamage(finalDamage);

            PortraitAnimator.Instance.PlayAliceAttack();
            PortraitAnimator.Instance.PlayEnemyHit();

            Debug.Log($"Gained {cm.tempDefense} defense, {cm.strength} strength. Dealt {finalDamage} damage.");

            DecayPoison();
            cm.dem.ResetForStand();
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
                    CardData prev = cm.jackpot
                        ? cards[cardIndex - 1].data.getJackpot()
                        : cards[cardIndex - 1].data;
                    if (!prev.cardType.Contains(CardData.CardType.Bomb))
                    {
                        ApplySpecialToSim(prev, cards, cardIndex - 1,
                            simPending, simMult, simDis, simPer, simEqual, ref simAttackNum);
                        simPending[CardData.CardType.Damage] +=
                            simDis[CardData.CardType.Damage] * prev.damage;
                        if (simDis[CardData.CardType.Damage] * prev.damage > 0)
                            simAttackNum++;
                        simPending[CardData.CardType.Defense] +=
                            simDis[CardData.CardType.Defense] * prev.defense;
                        simPending[CardData.CardType.Poison] +=
                            simDis[CardData.CardType.Poison] * prev.poison;
                    }
                }
            }
        }

        private int GetSimEqualBonus(
            CardData.CardType type1,
            Dictionary<(CardData.CardType, CardData.CardType), bool> simEqual,
            Dictionary<CardData.CardType, int> simPending)
        {
            int bonus = 0;
            foreach (var kvp in simEqual)
                if (kvp.Key.Item1 == type1)
                    bonus += Mathf.Max(0, simPending[kvp.Key.Item2]); // clamp to 0
            return bonus;
        }

        private void UpdateStrength(int strength)
        {
            cm.strength += strength;
            cm.strength = Mathf.Max(0, cm.strength);
            if (cm.strength > 0) AudioManager.instance.PlayGainStrength();
        }

        private void UpdatePoison(int poison)
        {
            cm.poison += poison;
            cm.poison = Mathf.Max(0, cm.poison);
        }

        private void UpdateDefense(int defense)
        {
            cm.tempDefense += defense;
            cm.tempDefense = Mathf.Max(0, cm.tempDefense);
            if (cm.tempDefense > 0) AudioManager.instance.PlayGainShield();
        }

        private void UpdateDamage(int damage)
        {
            if (cm.enemyDefense >= damage)
            {
                cm.enemyDefense -= damage;
                AudioManager.instance.PlayBluntDamage();
            }
            else
            {
                damage -= cm.enemyDefense;
                cm.enemyDefense = 0;
                cm.enemyCurrentHealth -= damage;
                cm.OnTakeDamage.Invoke();
            }
            cm.enemyCurrentHealth = Mathf.Max(0, cm.enemyCurrentHealth);
            Debug.Log("Enemy health: " + cm.enemyCurrentHealth);
        }

        private void DecayPoison()
        {
            if (cm.poison > 0) AudioManager.instance.PlayPoisonDamage();
            cm.poison = (int)Math.Floor(cm.poison / 2.0);
        }
        
        private int GetCardPriority(CardView cardView, List<CardView> allCards)
        {
            if (cardView?.data == null) return 6;
            CardData data = cm.jackpot ? cardView.data.getJackpot() : cardView.data;

            // Resolve CopyEffect by chaining back to find the effective card
            if (data.effect is CopyEffect)
            {
                int index = allCards.IndexOf(cardView);
                CardData resolved = ResolveCopy(allCards, index);
                if (resolved != null)
                    return GetPriorityFromData(resolved, allCards, allCards.IndexOf(cardView) - 1);
                return 6; // fallback to normal card
            }

            return GetPriorityFromData(data, allCards, allCards.IndexOf(cardView));
        }

        private int GetPriorityFromData(CardData data, List<CardView> allCards, int index)
        {
            // Effect-based priority — only if effect is HIGHER priority than base stat priority
            int effectPriority = GetEffectPriority(data);
            int statPriority = GetStatPriority(data);

            // Card goes to whichever is higher priority (lower number)
            return Mathf.Min(effectPriority, statPriority);
        }

        private int GetEffectPriority(CardData data)
        {
            if (data.cardType.Contains(CardData.CardType.Bomb)) return 8;
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
            if (data.effect is EqualEffect) return 7;
            return 99;
        }

        private int GetStatPriority(CardData data)
        {
            // Trigger bombs at the end, priority 8
            if (data.cardType.Contains(CardData.CardType.Bomb)) return 8;   
            // If card has strength, it belongs at priority 3
            if (data.strength > 0) return 3;
            // Everything else is a normal card
            return 6;
        }

        private CardData ResolveCopy(List<CardView> allCards, int copyIndex)
        {
            for (int i = copyIndex - 1; i >= 0; i--)
            {
                CardData prev = allCards[i]?.data;
                if (prev == null) continue;
                if (cm.jackpot) prev = prev.getJackpot();
                // Chain through copies
                if (prev.effect is CopyEffect) continue;
                if (prev.cardType.Contains(CardData.CardType.Bomb)) return null;
                return prev;
            }
            return null;
        }
    }
}