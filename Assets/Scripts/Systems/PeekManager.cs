using System.Collections.Generic;
using Systems;
using UnityEngine;

public class PeekManager : Singleton<PeekManager>
{
    [SerializeField] private GameObject peekPanel;

    [SerializeField] private List<CardView> peekCardVisuals;
    private List<CardData> peekCardData;

    public void ShowPeek(int count)
    {
        peekCardData = DeckManager.Instance.PeekCards(count);
        for (int i = 0; i < peekCardVisuals.Count; i++)
        {
            // Make the card show up
            if (i < peekCardData.Count)
            {
                peekCardVisuals[i].gameObject.SetActive(true);
                peekCardVisuals[i].Setup(peekCardData[i]);
                peekCardVisuals[i].transform.localScale = new Vector3(100, 100, 1);
            }
            // Make the ghost card disappear if we're not peeking as many cards
            else
            {
                peekCardVisuals[i].gameObject.SetActive(false);
            }
        }
        peekPanel.SetActive(true);
    }

    public void OnCardSelected(int index)
    {
        CardData chosenCard = peekCardData[index];
        // Remove the card from the deck
        DeckManager.Instance.RemoveCardFromDrawPile(chosenCard);
        
        // Turn off peek UI when you finish peeking
        ClosePeek();
        
        // Make it show up in your hand
        CardView cardView = CardViewCreator.Instance.CreateCardView(chosenCard, transform.position, Quaternion.identity);
        StartCoroutine(HandView.Instance.AnimateCardToHand(cardView));
        
        // 1. Update the CombatManager's active card
        CombatManager cm = Object.FindAnyObjectByType<CombatManager>();
        cm.lastDrawnCard = chosenCard; 

        // 2. Return to the HandlingCardState to finish the turn
        cm.ReturnToLastState();
    }

    public void ClosePeek()
    {
        // Clear peek data so it doesn't carry over
        foreach (CardView slot in peekCardVisuals)
        {
            slot.ClearVisuals();
        }

        
        peekPanel.SetActive(false);
    }
    
    public void OnPassClicked()
    {
        ClosePeek();
        
        // We don't change cm.lastDrawnCard
        // We simply tell the manager to go back
        CombatManager cm = Object.FindAnyObjectByType<CombatManager>();
        cm.ReturnToLastState(); 
    }
}
