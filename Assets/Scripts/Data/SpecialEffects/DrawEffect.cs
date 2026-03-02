using System.Collections;
using Data.SpecialEffects;
using UnityEngine;
//lets player draw [drawNum] cards
public class DrawEffect : SpecialEffect
{
    private int drawNum;
    
    [RuntimeInitializeOnLoadMethod]
    static void Register()
    {
        EffectRegistry.Register("DRAW", args =>
            new DrawEffect(
                int.Parse(args[0])
            )
        );
    }
    
    public DrawEffect(int num)
    {
        drawNum = num;
    }
    public CardData Execute(CombatManager cm, CardData card)
    {
        if (cm.dem.isRecalculating)
        {
            Debug.Log("DrawEffect suppressed during recalculation.");
            return card; // don't queue during jackpot recalc
        }
        if (cm.CurrentStateName == "Shattering")
        {
            Debug.Log("DrawEffect suppressed during Shattering.");
            return card;
        }
        
        Debug.Log($"Execute called. isDrawing = {cm.isDrawing}, pendingDraws before = {cm.pendingDraws}");
        cm.pendingDraws += drawNum;
        if (!cm.isDrawing)
            DrawNext(cm);
        return card;
    }

    public static void DrawNext(CombatManager cm)
    {
        Debug.Log($"DrawNext called. pendingDraws = {cm.pendingDraws}");
        if (cm.pendingDraws <= 0)
        {
            cm.isDrawing = false;
            return;
        }
        cm.pendingDraws--;
        cm.isDrawing = true;
        cm.StartCoroutine(DrawOne(cm));
    }

    private static IEnumerator DrawOne(CombatManager cm)
    {
        CardData drawnData = cm.Deck.DrawCard();
        if (drawnData == null) { cm.pendingDraws = 0; cm.isDrawing = false; yield break; }

        CardView cardView = CardViewCreator.Instance.CreateCardView(
            drawnData, cm.transform.position, Quaternion.identity);
        yield return cm.StartCoroutine(HandView.Instance.AnimateCardToHand(cardView));

        cm.MoveToNewState("HandlingCard"); // Exit() runs here, wiping lastDrawnCard is now harmless
        cm.lastDrawnCard = drawnData;      // ← set AFTER the transition
    }


}
