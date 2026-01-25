using UnityEngine;
using UnityEngine.Splines; 
using DG.Tweening;         
using System.Collections;          
using System.Collections.Generic;
using System.Linq; 

// This script is responsible for the visual organization of the cards in the player's hand.
public class HandView : Singleton<HandView>
{
    [SerializeField] private SplineContainer splineContainer;
    private readonly List<CardView> handCardViews = new();

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
            
            // Utilize DOTween to make cards move smoothly and not teleport around
            handCardViews[i].transform.DOMove(splinePosition + transform.position + 0.01f * i * Vector3.back, duration);
            handCardViews[i].transform.DORotate(rotation.eulerAngles, duration);
        }
        yield return new WaitForSeconds(duration);
    }

    public List<CardData> GetHandData()
    {
        // Take every CardView in the hand and return its CardData in a list
        return handCardViews.Select(cardView => cardView.data).ToList();
    }
}
