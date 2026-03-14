using UnityEngine;
using UnityEngine.Splines; 
using DG.Tweening;         
using System.Collections;          
using System.Collections.Generic;
using System.Linq;
using Systems;
using UnityEngine.Rendering;

// This script is responsible for the visual organization of the cards in the player's hand and contains the data for the cards in hand.
// It also takes care of the Shatter sequence and deletes/animates the cards getting deleted
public class HandView : Singleton<HandView>
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] public Transform drawDeckTransformPosition; // Slide the deck visual here
    
    public readonly List<CardView> handCardViews = new();
    public bool isShattering = false;

    public IEnumerator AnimateCardToHand(CardView cardView)
    {
        // 1. Force position and rotation at the deck
        Vector3 spawnPos = drawDeckTransformPosition != null ? drawDeckTransformPosition.position : Vector3.zero;
        cardView.transform.position = spawnPos;
        cardView.transform.rotation = Quaternion.Euler(0, 0, 90f);
        
        // 2. Add to list but DON'T rearrange the whole hand yet
        handCardViews.Add(cardView);

        // 3. Calculate where THIS specific card needs to go
        // (This is a simplified version of your layout math)
        float idealSpacing = 1.8f;
        float maxTotalWidth = 10.0f;
        int count = handCardViews.Count;
        float currentSpacing = Mathf.Min(idealSpacing, maxTotalWidth / Mathf.Max(1, count - 1));
        float totalWidth = (count - 1) * currentSpacing;
        float xPos = (-totalWidth / 2f) + ((count - 1) * currentSpacing);
        
        Vector3 targetPos = transform.position + new Vector3(xPos, 0, -0.01f * (count - 1));

        // 4. PERFORM THE FLIGHT: This is the actual animation
        if (AudioManager.instance != null) AudioManager.instance.PlayCardPlayed();
        
        // Fly the card from deck to its new slot
        cardView.transform.DOMove(targetPos, 0.4f).SetEase(Ease.OutBack);
        yield return cardView.transform.DORotate(Vector3.zero, 0.4f).SetEase(Ease.OutBack).WaitForCompletion();

        // 5. Now update everyone else's position to accommodate the new card
        yield return UpdateCardPositions(0.2f);
    }
    
    
    private IEnumerator UpdateCardPositions(float duration)
    {
        handCardViews.RemoveAll(c => !c);
        int count = handCardViews.Count;
        if (count == 0) yield break;

        // Define your spacing where cards don't touch
        float idealSpacing = 1.8f;  // Individual card spacing
        float maxTotalWidth = 10.0f;     // When should the cards start overlapping

        // Calculate the width if we used ideal spacing
        float intendedWidth = (count - 1) * idealSpacing;

        // Dynamic Spacing: Use ideal spacing UNLESS it exceeds max width
        float currentSpacing = (intendedWidth <= maxTotalWidth) 
            ? idealSpacing 
            : maxTotalWidth / Mathf.Max(1, count - 1);

        float totalWidth = (count - 1) * currentSpacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            CardView card = handCardViews[i];
            if (!card) continue;

            float xPos = startX + (i * currentSpacing);
            float normalizedIndex = (count > 1) ? (i - (count - 1) / 2f) : 0f;
            float baseTilt = -normalizedIndex * 3f;
            
            if (card.TryGetComponent<SortingGroup>(out var sg))
            {
                sg.sortingOrder = i;
            }

            
        
            // Update the Home position with the new dynamic spacing
            card.homePos = transform.position + new Vector3(xPos, 0, -0.01f * i);
            card.homeRot = Quaternion.Euler(0, 0, baseTilt); 

            // Only animate if not currently hovered to prevent jitter
            if (card.gameObject.activeInHierarchy)
            {
                card.transform.DOMove(card.homePos, duration).SetEase(Ease.OutBack);
                card.transform.DORotateQuaternion(card.homeRot, duration).SetEase(Ease.OutBack);
            }
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
