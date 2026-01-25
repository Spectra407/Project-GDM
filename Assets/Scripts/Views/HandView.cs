using UnityEngine;
using UnityEngine.Splines; // Fixes: SplineContainer and Spline errors
using DG.Tweening;         // Fixes: .DOMove and .DORotate errors
using System.Collections;           // Fixes: IEnumerator
using System.Collections.Generic;   // Fixes: List<CardView>

public class HandView : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    private readonly List<CardView> cards = new();

    public IEnumerator AddCard(CardView cardView)
    {
        cards.Add(cardView);
        Debug.Log(cards.Count);
        yield return UpdateCardPositions(0.15f);
    }

    private IEnumerator UpdateCardPositions(float duration)
    {
        if (cards.Count == 0) yield break;  // Break when there's no cards
        float cardSpacing = 1f / 15f;       // Adjust according to how many max cards we are expecting, here its at 15
        float firstCardPosition = 0.5f - (cards.Count - 1) * cardSpacing / 2;  
        Spline spline = splineContainer.Spline;
        for (int i = 0; i < cards.Count; i++)   // Adjust the position of each card on the spline when a new card is added.
        {
            float p = firstCardPosition + (i * cardSpacing);
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(-up, Vector3.Cross(-up, forward).normalized);
            
            // Utilize DOTween to make cards move smoothly and not teleport around
            cards[i].transform.DOMove(splinePosition + transform.position + 0.01f * i * Vector3.back, duration);
            cards[i].transform.DORotate(rotation.eulerAngles, duration);
        }
        yield return new WaitForSeconds(duration);
    }
}
