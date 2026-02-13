using UnityEngine;
//lets player draw [drawNum] cards
public class DrawEffect : SpecialEffect
{
    private int drawNum;
    public DrawEffect(int num)
    {
        drawNum = num;
    }
    public void Execute(CombatManager cm, CardData card)
    {
        for (int i = 0; i < drawNum; i++)
        {
            Debug.Log("Card drawn.");
            //need to implement this
        }
    }
}
