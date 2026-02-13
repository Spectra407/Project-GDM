using UnityEngine;
//lets player draw [drawNum] cards
public class MadnessEffect : SpecialEffect
{
    private int drawNum;
    public void DrawEffect(int num)
    {
        drawNum = num;
    }
    public abstract void Execute(CombatManager cm, CardData card)
    {
        for (int i = 0; i < drawNum; i++)
        {
            Debug.Log("Card drawn.");
            //need to implement this
        }
    }
}
