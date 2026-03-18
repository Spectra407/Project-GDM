using UnityEngine;
using System.Collections;

public class DiceManager : MonoBehaviour
{
    [Header("Dice Faces")]
    public int diceSize=6; // How many attacks in total the enemy has: 6

    public IEnumerator RollDice(System.Action<int> onResult)
    {
        int result = Random.Range(0, diceSize);
        onResult?.Invoke(result);
        yield break;
    }
}