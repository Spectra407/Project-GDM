using UnityEngine;
using UnityEngine.InputSystem;

public class TestSystem : MonoBehaviour
{
    [SerializeField] private HandView handView;
    
    // Temporary card data that will be used until we setup the deck manager properly
    [SerializeField] public CardData temporaryCardData;

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame) // Draw a card into your hand using spacebar.
        {
            // CardData data = data of a card in the deck
            
            CardView cardView = CardViewCreator.Instance.CreateCardView(temporaryCardData, transform.position, Quaternion.identity);
            StartCoroutine(handView.AddCard(cardView));
        }
    }
}
