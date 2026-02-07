using Systems;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestSystem : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame) // Draw a card into your hand using spacebar.
        {
            CardData data = DeckManagerKenny.Instance.DrawCard();

            if (data != null)
            {
                CardView cardView = CardViewCreator.Instance.CreateCardView(data, transform.position, Quaternion.identity);
                StartCoroutine(HandView.Instance.AnimateCardToHand(cardView));
            }
            
        }

        if (Keyboard.current.pKey.wasPressedThisFrame) // Test peek 3 using P.
        {
            PeekManager.Instance.ShowPeek(3);
        }
    }
}
