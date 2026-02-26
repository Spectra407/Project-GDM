using UnityEngine;
using UnityEngine.Splines; 
using DG.Tweening;         
using System.Collections;          
using System.Collections.Generic;
using System.Linq;
using Systems;

// This script is responsible for the visual organization of the cards in the player's hand and contains the data for the cards in hand.
// It also takes care of the Shatter sequence and deletes/animates the cards getting deleted
public class HandView : Singleton<HandView>
{
    [SerializeField] private SplineContainer splineContainer;
    public readonly List<CardView> handCardViews = new();
    public bool isShattering = false;

    public IEnumerator AnimateCardToHand(CardView cardView)
    {
        handCardViews.Add(cardView);
        Debug.Log(handCardViews.Count);
        yield return UpdateCardPositions(0.15f);
    }

    private IEnumerator UpdateCardPositions(float duration)
    {
        if (handCardViews.Count == 0) yield break;  // Break when there's no cards
        float cardSpacing = 1f / 15f;       // Adjust according to how many max cards we are expecting, here its at 15
        float firstCardPosition = 0.5f - (handCardViews.Count - 1) * cardSpacing / 2;  
        Spline spline = splineContainer.Spline;
        for (int i = 0; i < handCardViews.Count; i++)   // Adjust the position of each card on the spline when a new card is added.
        {
            float p = firstCardPosition + (i * cardSpacing);
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(-up, Vector3.Cross(-up, forward).normalized);

            Vector3 targetPos = splinePosition + transform.position + 0.01f * i * Vector3.back;
            
            // Utilize DOTween to make cards move smoothly and not teleport around
            handCardViews[i].transform.DOMove(targetPos, duration);
            handCardViews[i].transform.DORotate(rotation.eulerAngles, duration);
        }
        yield return new WaitForSeconds(duration);
    }

    public List<CardData> GetHandData()
    {
        // Take every CardView in the hand and return its CardData in a list
        return handCardViews.Select(cardView => cardView.data).ToList();
    }

    public IEnumerator ShatterSequence(CardView survivor)
    {
        // BURN ANIMATION LOGIC
        // Create a list of cards to burn
        List<CardView> cardsToBurn = new List<CardView>(handCardViews);
        cardsToBurn.Remove(survivor);

        foreach (var card in cardsToBurn)
        {
            // TRIGGER BURN ANIMATION HERE
            
            // card.GetComponent<Animator>().SetTrigger("Burn");
        }
        
        // Wait for animation to play
        yield return new WaitForSeconds(0.8f);
        
        
        // SHATTER LOGIC
        List<CardData> recycledData = new List<CardData>();
        
        // Get the data of all the cards other than the survivor
        foreach (var card in cardsToBurn)
        {
            recycledData.Add(card.data);
            Destroy(card.gameObject);
        }
        
        // Put back all the cards into your draw pile
        foreach (CardData recycledCard in recycledData)
        {
            DeckManager.Instance.RecycleToDrawPile(recycledCard);
        }
        
        // Reset the information with only the survivor
        handCardViews.Clear();  // Remove null objects since we Destroyed the gameobjects.
        handCardViews.Add(survivor);
        
        // Update the cards in your hand
        yield return StartCoroutine(UpdateCardPositions(0.5f));

        // Reset state so Alice can play cards again, adjust later
        isShattering = false;
    }
    
    public void ClearHand()
    {
        // Stop the bug where if you clear the hand while hovering over the card, the hover persists.
        CardViewHoverSystem.Instance.Hide();
        
        // Loop through all active cards in the hand
        foreach (var card in handCardViews)
        {
            if (card != null)
            {
                // Recycle the data back to the DeckManager Singleton
                if (card.data != null)
                {
                    DeckManager.Instance.RecycleToDrawPile(card.data); 
                }

                // Physically remove the card from the scene
                Destroy(card.gameObject); 
            }
        }

        // Clear the list
        handCardViews.Clear(); 

        DeckManager.Instance.OnShuffle.Invoke();
        Debug.Log("Hand cleared and data recycled to deck.");
    }
    
    
    public void SetHandInteractable(bool isInteractable)
    {
        foreach (var card in handCardViews)
        {
            if (card != null)
            {
                // Get the 3D collider
                BoxCollider collider = card.GetComponent<BoxCollider>();
            
                if (collider != null)
                {
                    // Toggle the collider off so it doesn't intercept Raycasts when we Peek
                    collider.enabled = isInteractable;
                }
            }
        }
    }
}
