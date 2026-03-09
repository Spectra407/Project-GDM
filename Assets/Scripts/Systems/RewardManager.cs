using System.Collections.Generic;
using UnityEngine;
using Systems;

public class RewardManager : Singleton<RewardManager>
{
    [SerializeField] private List<CardView> rewardCardVisuals; // 3 card slots in UI, similar to peek
    [SerializeField] private GameObject rewardPanel;
    private List<CardData> currentOffers = new List<CardData>();    // 3 card data
    public void ShowRewards(List<CardData> offers)
    {
        currentOffers = offers;
        for (int i = 0; i < rewardCardVisuals.Count; i++)
        {
            if (i < offers.Count)
            {
                rewardCardVisuals[i].gameObject.SetActive(true);
                rewardCardVisuals[i].Setup(offers[i]);
                rewardCardVisuals[i].transform.localScale = new Vector3(200, 200, 1);
            }
            else
            {
                rewardCardVisuals[i].gameObject.SetActive(false);
            }
        }
        // TURN ON REWARD UI HERE
        rewardPanel.SetActive(true);
    }

    public void HideRewards()
    {
        // Enable hand interaction when done.
        HandView.Instance.SetHandInteractable(true);
        
        foreach (var slot in rewardCardVisuals)
            slot.gameObject.SetActive(false);
        // TURN OFF REWARD UI HERE
        rewardPanel.SetActive(false);
    }

    public int GetIndexOfCard(CardView card)
    {
        return rewardCardVisuals.IndexOf(card);
    }

    public CardData GetOffer(int index)
    {
        return currentOffers[index];
    }
}