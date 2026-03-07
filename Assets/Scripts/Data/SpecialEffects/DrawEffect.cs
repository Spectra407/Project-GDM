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
        if (cm.dem.isRecalculating || cm.dem.isreshuffling)
        {
            Debug.Log("DrawEffect suppressed during recalculation.");
            return card;
        }
        if (cm.CurrentStateName == "Shattering")
        {
            Debug.Log("DrawEffect suppressed during Shattering.");
            return card;
        }
    
        Debug.Log($"Execute called. isDrawing = {cm.isDrawing}, pendingDraws = {cm.pendingDraws}, isRecalculating = {cm.dem.isRecalculating}");
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
        
        // PROBLEM: SCRIPTABLE OBJECTS ARE SHARED, SO THE NEW INSTANCEID OVERWRITES THE PREVIOUS DUPLICATE'S BC OF MUTABILITY
        drawnData.InstanceID = System.Guid.NewGuid().ToString();    // New InstanceID when drawn,
                                                                    // doesn't rly work dw bout it I gave up on troubleshooting this lmao
                                                                    // Just make it so that you can't have duplicate Copy card effects

        CardView cardView = CardViewCreator.Instance.CreateCardView(
            drawnData, cm.transform.position, Quaternion.identity);
        yield return cm.StartCoroutine(HandView.Instance.AnimateCardToHand(cardView));

        cm.MoveToNewState("HandlingCard"); 
        cm.lastDrawnCard = drawnData;      
    }


}
