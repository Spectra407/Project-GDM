using UnityEngine;
using UnityEngine.Splines; 
using DG.Tweening;         
using System.Collections;          
using System.Collections.Generic;
using System.Linq;
using Systems;
using UnityEngine.Rendering;

public class HandView : Singleton<HandView>
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] public Transform drawDeckTransformPosition;
    
    public readonly List<CardView> handCardViews = new();
    public bool isShattering = false;

    public IEnumerator AnimateCardToHand(CardView cardView)
    {
        Vector3 spawnPos = drawDeckTransformPosition != null ? drawDeckTransformPosition.position : Vector3.zero;
        cardView.transform.position = spawnPos;
        cardView.transform.rotation = Quaternion.Euler(0, 0, 90f);

        cardView.homePos = Vector3.zero;

        handCardViews.Add(cardView);

        if (AudioManager.instance != null) AudioManager.instance.PlayCardPlayed();

        handCardViews.RemoveAll(c => !c);
        int count = handCardViews.Count;

        float idealSpacing = 1.8f;
        float maxTotalWidth = 10.0f;
        float intendedWidth = (count - 1) * idealSpacing;
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
                sg.sortingOrder = i;

            Vector3 targetPos = transform.position + new Vector3(xPos, 0, -0.01f * i);
            Quaternion targetRot = Quaternion.Euler(0, 0, baseTilt);

            if (!card.gameObject.activeInHierarchy) continue;

            if (card == cardView)
            {
                card.transform.DOMove(targetPos, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    card.homePos = targetPos;
                    card.homeRot = targetRot;
                });
                card.transform.DORotateQuaternion(targetRot, 0.5f).SetEase(Ease.OutBack);
            }
            else
            {
                card.isAnimating = true;
                card.homeRot = targetRot;
                card.transform.DOMove(targetPos, 0.6f).SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    card.homePos = targetPos;
                    card.isAnimating = false;
                });
                card.transform.DORotateQuaternion(card.homeRot, 0.6f).SetEase(Ease.OutCubic);
            }
        }

        yield return new WaitForSeconds(0.6f);
    }
    
    private IEnumerator UpdateCardPositions(float duration)
    {
        handCardViews.RemoveAll(c => !c);
        int count = handCardViews.Count;
        if (count == 0) yield break;

        float idealSpacing = 1.8f;
        float maxTotalWidth = 10.0f;
        float intendedWidth = (count - 1) * idealSpacing;
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

            card.homePos = transform.position + new Vector3(xPos, 0, -0.01f * i);
            card.homeRot = Quaternion.Euler(0, 0, baseTilt); 

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
        return handCardViews.Select(cardView => cardView.data).ToList();
    }

    public IEnumerator ShatterSequence(CardView survivor)
    {
        List<CardView> cardsToBurn = handCardViews.Where(c => c != survivor).ToList();
        
        handCardViews.Clear();
        handCardViews.Add(survivor);
        Debug.Log("Finished shatter preload: Hand count is now " + handCardViews.Count);

        foreach (var card in cardsToBurn)
        {
            if (card.TryGetComponent<Dissolve>(out Dissolve dissolveEffect))
            {
                dissolveEffect.StartVanish();
            }
        }
        
        StartCoroutine(UpdateCardPositions(1.5f));
        
        yield return new WaitForSeconds(2.1f);

        Debug.Log("Finished burn animation");
        
        foreach (var card in cardsToBurn)
        {
            if (card == null) continue;
            card.transform.DOKill();
            DeckManager.Instance.RecycleToDrawPile(card.data);
            Destroy(card.gameObject);
        }

        isShattering = false;
    }
    
    public void ClearHand()
    {
        CardViewHoverSystem.Instance.Hide();
        StartCoroutine(ClearHandAnimated());
    }

    private IEnumerator ClearHandAnimated()
    {
        Vector3 deckPos = drawDeckTransformPosition != null ? drawDeckTransformPosition.position : Vector3.zero;

        List<CardView> cardsToClear = new List<CardView>(handCardViews);
        handCardViews.Clear();

        for (int i = 0; i < cardsToClear.Count; i++)
        {
            CardView card = cardsToClear[i];
            if (card == null) continue;

            card.isAnimating = true;

            // Stagger each card slightly so they don't all leave at once
            float delay = i * 0.05f;

            card.transform.DOMove(deckPos, 0.4f).SetEase(Ease.InBack).SetDelay(delay);
            card.transform.DORotate(new Vector3(0, 0, 90f), 0.4f).SetEase(Ease.InBack).SetDelay(delay);
            card.transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack).SetDelay(delay).OnComplete(() =>
            {
                if (card != null)
                {
                    if (card.data != null)
                        DeckManager.Instance.RecycleToDrawPile(card.data);
                    Destroy(card.gameObject);
                }
            });
        }

        // Wait for the last card to finish
        float totalDuration = 0.4f + (cardsToClear.Count * 0.05f);
        yield return new WaitForSeconds(totalDuration);

        DeckManager.Instance.ShuffleAll(DeckManager.Instance.drawPile);
        DeckManager.Instance.OnShuffle.Invoke();

        Debug.Log("Hand cleared and data recycled to deck.");
    }
    
    public IEnumerator AnimateCardToDeck(CardView cardView)
    {
        Vector3 deckPos = drawDeckTransformPosition != null ? drawDeckTransformPosition.position : Vector3.zero;

        handCardViews.Remove(cardView);
        cardView.isAnimating = true;

        cardView.transform.DOMove(deckPos, 0.4f).SetEase(Ease.InBack);
        cardView.transform.DORotate(new Vector3(0, 0, 90f), 0.4f).SetEase(Ease.InBack);
        cardView.transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack).OnComplete(() =>
        {
            if (cardView != null)
            {
                if (cardView.data != null)
                    DeckManager.Instance.RecycleToDrawPile(cardView.data);
                Destroy(cardView.gameObject);
            }
        });

        yield return new WaitForSeconds(0.4f);

        // Reposition remaining cards to close the gap
        yield return UpdateCardPositions(0.3f);
    }
    
    public void SetHandInteractable(bool isInteractable)
    {
        foreach (var card in handCardViews)
        {
            if (card != null)
            {
                BoxCollider collider = card.GetComponent<BoxCollider>();
                if (collider != null)
                {
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