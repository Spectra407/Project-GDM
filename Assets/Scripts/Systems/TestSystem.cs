using System.Collections.Generic;
using Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class TestSystem : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // This handles clicking cards when you're in the shatter state to preserve 1 card.
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleCardClick();
        }
        
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

        ForceShatter();    // Press "o" key to force the shatter state to test shatter
    }

    public void ForceShatter()
    {
        if (Keyboard.current.oKey.wasPressedThisFrame)      // Change this to if (currentMadness > 7) or smtn later on
        {
            HandView.Instance.isShattering = true;
            Debug.Log("Shattered! Choose 1 card to preserve.");
        }
    }
    
    private void HandleCardClick()      // For now this is only used when isShattering = true and allows you to preserve one card while reshuffling the rest.
    {
        // NOTE: I TRIED USING ON MOUSE CLICK ACTIONS TO TRIGGER THIS STUFF BUT IT WASN'T REGISTERING/WORKING 
        // SO WE'RE USING RAYCASTING SHENANIGANS INSTEAD 
        
        // Set up the Pointer Data for the Raycast
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Mouse.current.position.ReadValue();

        // Raycast against all objects in the scene
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // Check if we hit anything
        if (results.Count > 0)
        {
            // Pick ONLY the first result (the one visually on top)
            GameObject topObject = results[0].gameObject;
            
            // Find the CardView component on this object or its parents
            CardView topCard = topObject.GetComponentInParent<CardView>();

            // Trigger Shatter only if a card was hit and we are in the Shatter state
            if (topCard != null && HandView.Instance.isShattering)
            {
                Debug.Log("Shatter Survivor Selected: " + topCard.name);
                
                // We MUST use StartCoroutine because ShatterSequence is an IEnumerator
                HandView.Instance.StartCoroutine(HandView.Instance.ShatterSequence(topCard));
            }
        }
    }
}
