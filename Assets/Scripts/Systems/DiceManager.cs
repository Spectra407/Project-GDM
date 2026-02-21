using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public int RollDice()
    {
        int rolled = Random.Range(1,7);
        Debug.Log("Rolled a"+rolled);
        return rolled;
    }
}
