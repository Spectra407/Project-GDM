using System.Collections;
using Systems;
using UnityEngine;

public class ShufflingHandState : ITurnState
{
    private CombatManager _cm;

    public ShufflingHandState(CombatManager cm)
    {
        _cm = cm;
    }

    public void Enter()
    {
        Debug.Log("ShufflingHandState entered —> waiting for Yes/No input.");
        HandView.Instance.SetHandInteractable(false);
        // TURN ON Yes/No UI buttons here
        _cm.shuffleYesButton.SetActive(true);
        _cm.shuffleNoButton.SetActive(true);
    }

    public void HandleInput(string inputID)
    {
        Debug.Log($"ShufflingHandState received input: {inputID}");
        if (inputID == "YesButton") ConfirmShuffle();
        else if (inputID == "NoButton") CancelShuffle();
    }

    private void ConfirmShuffle()
    {
        // Recycle all cards including the ShuffleHand card back into deck
        foreach (var cv in HandView.Instance.handCardViews)
        {
            if (cv == null) continue;
            DeckManager.Instance.RecycleToDrawPile(cv.data);
            GameObject.Destroy(cv.gameObject);
        }
        HandView.Instance.handCardViews.Clear();

        // Reset all stats and madness since hand is now empty
        _cm.dem.ResetForJackpot();
        _cm.madness = _cm.alice.startingMadness;
        _cm.jackpot = false;

        _cm.MoveToNewState("ChoosingAction");
    }

    private void CancelShuffle()
    {
        // Hand stays as is, just go back to ChoosingAction
        // ShuffleHand card remains in hand
        _cm.MoveToNewState("ChoosingAction");
    }

    public void Update() { }
    public void Exit()
    {
        HandView.Instance.SetHandInteractable(true);
        // TURN OFF Yes/No UI buttons here
        _cm.shuffleYesButton.SetActive(false);
        _cm.shuffleNoButton.SetActive(false);
    }
}