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
            //need to implement this
        }
        return card;
    }
}
