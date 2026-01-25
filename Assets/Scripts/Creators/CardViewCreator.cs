using DG.Tweening;
using UnityEngine;

public class CardViewCreator : Singleton<CardViewCreator>
{
    [SerializeField] private CardView cardViewPrefab;

    public CardView CreateCardView(CardData data, Vector3 position, Quaternion rotation)
    {
        // Create a new physical card
        CardView cardView = Instantiate(cardViewPrefab, position, rotation);    
        
        // Put the CardData into the card view
        cardView.Setup(data);
        
        // Adjust the size and position
        cardView.transform.localScale = Vector3.zero;   
        cardView.transform.DOScale(Vector3.one, 0.15f);     
        return cardView;
    }
}
