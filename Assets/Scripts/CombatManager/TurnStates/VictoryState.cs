using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Systems;
using UnityEngine.SceneManagement;

public class VictoryState : ITurnState
{
    private CombatManager _cm;
    private int _picksRemaining = 2;
    private bool _canSelect = false;

    public VictoryState(CombatManager cm)
    {
        _cm = cm;
    }

    public void Enter()
    {
        Debug.Log("Victory! Choose your rewards.");
        _picksRemaining = 2;
        _cm.StartCoroutine(OfferRewards());
    }

    private IEnumerator OfferRewards()
    {
        yield return new WaitForSeconds(0.3f);
        
        List<CardData> offers = DeckManager.Instance.GetRewardOffers(3);
        RewardManager.Instance.ShowRewards(offers);
        _canSelect = true;
    }

    public void Update()
    {
        if (!_canSelect) return;
        if (Input.GetMouseButtonDown(0))
            DetectRewardClick();
    }

    private void DetectRewardClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            CardView clickedCard = hit.collider.GetComponentInParent<CardView>();
            if (clickedCard != null)
            {
                int index = RewardManager.Instance.GetIndexOfCard(clickedCard);
                if (index != -1)
                {
                    _canSelect = false;
                    CardData chosen = RewardManager.Instance.GetOffer(index);
                    SelectReward(chosen);
                }
            }
        }
    }

    private void SelectReward(CardData chosen)
    {
        // Remove chosen reward card so it's not offered again next round
        DeckManager.Instance.rewardDeck.Remove(chosen);
        
        // Add to current deck permanently
        DeckManager.Instance.currentDeck.Add(chosen);
        Debug.Log($"Added {chosen.cardName} to deck. currentDeck now has {DeckManager.Instance.currentDeck.Count} cards.");

        Debug.Log($"Added {chosen.cardName} to deck.");

        _picksRemaining--;
        RewardManager.Instance.HideRewards();
        
        
        if (_picksRemaining > 0)
        {
            // Offer another set of cards
            _cm.StartCoroutine(OfferRewards());
        }
        else
        {
            CardViewHoverSystem.Instance.Hide();    // Hide it to avoid some bugs when initializing the shop scene
            
            // Gain money according to the enemy
            _cm.alice.AddGold(_cm.enemy.goldReward);
            Debug.Log($"Earned {_cm.enemy.goldReward} gold.");
            Debug.Log("Rewards selected. Proceeding...");
            // MOVE TO NEXT SCENE HERE
            _cm.StartCoroutine(DelayedSceneLoad(_cm.enemy.nextSceneName)); // Either next fight or "ShopScene"
            // Maybe use SceneManager.LoadScene(_cm.enemy.nextSceneName); so we can put into the enemydata what the next scene is
        }
    }

    private IEnumerator DelayedSceneLoad(string sceneName)
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
    }

    public void HandleInput(string inputID) { }
    public void Exit() { }
}