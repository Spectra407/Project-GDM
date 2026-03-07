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
        handCardViews.RemoveAll(c => !c);
        if (handCardViews.Count == 0) yield break;  // Break when there's no cards
        
        float cardSpacing = 1f / 12f;       // Adjust according to how many max cards we are expecting, here its at 12
        float firstCardPosition = 0.5f - (handCardViews.Count - 1) * cardSpacing / 2;  
        Spline spline = splineContainer.Spline;
        for (int i = 0; i < handCardViews.Count; i++)   // Adjust the position of each card on the spline when a new card is added.
        {
            CardView card = handCardViews[i];
            // If the card was destroyed mid-animation, skip
            if (!card) continue;
            
            Transform t = card.transform;
            
            float p = firstCardPosition + (i * cardSpacing);
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(-up, Vector3.Cross(-up, forward).normalized);

            Vector3 targetPos = splinePosition + transform.position + 0.01f * i * Vector3.back;
            
            // Utilize DOTween to make cards move smoothly and not teleport around
            
            t.DOMove(targetPos, duration);
            t.DORotate(rotation.eulerAngles, duration);
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
        List<CardView> cardsToBurn = handCardViews.Where(c => c != survivor).ToList();
        
        handCardViews.Clear();
        handCardViews.Add(survivor);
        Debug.Log("Finished shatter preload: Hand count is now " + handCardViews.Count);

        foreach (var card in cardsToBurn)
        {
            // TRIGGER BURN ANIMATION HERE
            if (card.TryGetComponent<Dissolve>(out Dissolve dissolveEffect))
            {
                dissolveEffect.StartVanish();
            }
            
        }
        
        // Update the cards in your hand
        StartCoroutine(UpdateCardPositions(1.5f));
        
        // Wait for animation to play
        yield return new WaitForSeconds(2.1f);
        
        

        Debug.Log("Finished burn animation");
        
        // SHATTER LOGIC
        List<CardData> recycledData = new List<CardData>();
        
        // Get the data of all the cards other than the survivor
        foreach (var card in cardsToBurn)
        {
            if (card == null) continue;
            card.transform.DOKill();
            DeckManager.Instance.RecycleToDrawPile(card.data);
            Destroy(card.gameObject);
        }
        

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
        
        // Shuffle deck again
        DeckManager.Instance.ShuffleAll(DeckManager.Instance.drawPile);
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
    
    public void RefreshHandPositions(float duration = 0.15f)
    {
        StartCoroutine(UpdateCardPositions(duration));
    }
}
