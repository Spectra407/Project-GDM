using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Systems;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private List<ShopCardSlot> cardSlots; // Card slots in UI, 8 for now?
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private int minPrice = 20;
    [SerializeField] private int maxPrice = 40;
    [SerializeField] private string nextSceneName; // scene to load after the shop
    [SerializeField] public AliceData alice;

    private bool canInteract = false;

    private void Start()
    {
        canInteract = true;
    
        List<CardData> offers = DeckManager.Instance.GetRewardOffers(8);
        for (int i = 0; i < cardSlots.Count; i++)
        {
            if (i < offers.Count)
            {
                int price = Random.Range(minPrice, maxPrice + 1);
                cardSlots[i].Setup(offers[i], price, this);
                cardSlots[i].gameObject.SetActive(true);
            }
            else
            {
                cardSlots[i].gameObject.SetActive(false);
            }
        }
        UpdateGoldUI();
    }

    

    public void TryBuyCard(ShopCardSlot slot)
    {
        if (!canInteract) return;
        if (alice.SpendGold(slot.price))
        {
            DeckManager.Instance.currentDeck.Add(slot.card);
            Debug.Log($"Bought {slot.card.cardName} for {slot.price} gold.");
            slot.MarkAsSold();
            UpdateGoldUI();
        }
        else
        {
            Debug.Log("Not enough gold!");
            // SHAME THE PLAYER FOR BEING POOR :)
        }
    }

    public void LeaveShop()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }

    private void UpdateGoldUI()
    {
        goldText.text = $"{alice.gold} gold";
    }
}