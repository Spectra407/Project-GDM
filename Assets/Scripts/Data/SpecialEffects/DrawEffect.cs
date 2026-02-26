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
        for (int i = 0; i < drawNum; i++)
        {
            Debug.Log("Card drawn.");
            /* commented out for now
            CardData drawnData = cm.Deck.DrawCard();
            if (drawnData != null)
            {
                //not robust yet
                //no view in hand
                //what if draw card drawn? several cards drawn? 
                // yield return new WaitForSeconds(0.8f); //maybe add a little wait beforehand
                cm.lastDrawnCard = drawnData;
                CardView cardView = CardViewCreator.Instance.CreateCardView(drawnData, cm.transform.position, Quaternion.identity);
                cm.StartCoroutine(HandView.Instance.AnimateCardToHand(cardView));
                cm.dem.ResolveOnDraw(cm.lastDrawnCard);
            }
            cm.lastDrawnCard = null;
            //need to implement this
            */
        }
        return card;
    }
}
