using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Systems;

public class ShopManager : MonoBehaviour
{
    [Header("Healing")]
    [SerializeField] private int healCost = 25;
    [SerializeField] private int healAmount = 20;
    
    [Header("Card stuff")]
    [SerializeField] private List<ShopCardSlot> cardSlots; // Card slots in UI, 8 for now?
    [SerializeField] private int minPrice = 25;
    [SerializeField] private int maxPrice = 45;
    [SerializeField] private string nextSceneName; // scene to load after the shop
    [SerializeField] public AliceData alice;
    
    [Header("Music")]
    public AudioClip backgroundMusic;   // Current battle's soundtrack
    public float loopStartTime; // At what second does the track loop

    private bool canInteract = false;

    public FadeScript fade;

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
        fade.FadeIn();
        AudioManager.instance.PlayFightMusic(backgroundMusic, loopStartTime);
    }

    

    public void TryBuyCard(ShopCardSlot slot)
    {
        if (!canInteract) return;
        if (alice.SpendGold(slot.price))
        {
            DeckManager.Instance.currentDeck.Add(slot.card);
            Debug.Log($"Bought {slot.card.cardName} for {slot.price} gold.");
            slot.MarkAsSold();
        }
        else
        {
            Debug.Log("Not enough gold!");
            // SHAME THE PLAYER FOR BEING POOR :)
        }
    }
    
    public void TryBuyHeal()
    {
        if (!canInteract) return;
    
        if (alice.currentHealth >= alice.maxHealth)
        {
            Debug.Log("Already at full health!");
            return;
        }
    
        if (alice.SpendGold(healCost))
        {
            alice.currentHealth = Mathf.Min(alice.currentHealth + healAmount, alice.maxHealth);
            Debug.Log($"Healed for {healAmount} HP. Current health: {alice.currentHealth}");
        }
        else
        {
            Debug.Log("Not enough gold!");
        }
    }

    public void LeaveShop()
    {
        fade.FadeOut();
        Invoke("DelayedLeaveShop",3.0f);
    }

    void DelayedLeaveShop()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }

}