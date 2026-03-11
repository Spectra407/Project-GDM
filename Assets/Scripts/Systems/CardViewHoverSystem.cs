using UnityEngine;
using UnityEngine.Events;

public class CardViewHoverSystem : Singleton<CardViewHoverSystem>
{
    [SerializeField] private CardView cardViewHover;
    public UnityEvent OnCardHover;

    public void Show(CardData card, Vector3 position)
    {
        if (cardViewHover == null) return;
        OnCardHover.Invoke();
        cardViewHover.gameObject.SetActive(true);
        cardViewHover.Setup(card);
        cardViewHover.transform.position = position;
    }

    public void Hide()
    {
        cardViewHover.gameObject.SetActive(false);
    }
}
