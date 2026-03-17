using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DiceManager : MonoBehaviour
{
    [Header("Dice Sprites")]
    public Sprite[] diceFaces;

    public Image uiImage;

    public float shuffleDuration = 0.05f;
    public float shuffleSpeed = 0.05f;

    public IEnumerator RollDice(System.Action<int> onResult)
    {
        float timer = 0f;

        //Shuffle the dice
        while (timer < shuffleDuration)
        {
            int randomIndex = Random.Range(0, diceFaces.Length);
            SetSprite(randomIndex);
            timer += shuffleSpeed;
            yield return new WaitForSeconds(shuffleSpeed);

            
        }
        
        //Stop the suffle and set the index
        int finalIndex = Random.Range(0, diceFaces.Length);
        SetSprite(finalIndex);

        onResult?.Invoke(finalIndex);
    }

    //Set the picture
    private void SetSprite(int index)
    {
        uiImage.sprite = diceFaces[index];
    }
}
