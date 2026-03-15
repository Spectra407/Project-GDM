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
        Debug.Log("ShufflingHandState entered");
        HandView.Instance.SetHandInteractable(false);
        _cm.shuffleYesButton.SetActive(true);
        _cm.shuffleNoButton.SetActive(true);
        Debug.Log($"Yes active: {_cm.shuffleYesButton.activeSelf}, No active: {_cm.shuffleNoButton.activeSelf}");
    }
    
    

    public void HandleInput(string inputID)
    {
        Debug.Log($"ShufflingHandState received input: {inputID}");
        if (inputID == "YesButton") ConfirmShuffle();
        else if (inputID == "NoButton") CancelShuffle();
    }

    private void ConfirmShuffle()
    {
        _cm.StartCoroutine(ConfirmShuffleAnimated());
    }

    private IEnumerator ConfirmShuffleAnimated()
    {
        _cm.dem.ResetForJackpot();
        _cm.madness = _cm.alice.startingMadness;
        _cm.jackpot = false;

        float totalDuration = 0.4f + (HandView.Instance.handCardViews.Count * 0.05f);
        HandView.Instance.ClearHand();
        yield return new WaitForSeconds(totalDuration);

        _cm.MoveToNewState("ChoosingAction");
    }

    private void CancelShuffle()
    {
        // Hand stays as is, just go back to ChoosingAction
        // ShuffleHand card remains in hand
        _cm.MoveToNewState("ChoosingAction");
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Camera cam = Camera.main;
            RectTransform yesRect = _cm.shuffleYesButton.GetComponent<RectTransform>();
            RectTransform noRect = _cm.shuffleNoButton.GetComponent<RectTransform>();

            Debug.Log($"Mouse pos: {Input.mousePosition}");
            Debug.Log($"Yes contains: {RectTransformUtility.RectangleContainsScreenPoint(yesRect, Input.mousePosition, cam)}");
            Debug.Log($"No contains: {RectTransformUtility.RectangleContainsScreenPoint(noRect, Input.mousePosition, cam)}");

            if (RectTransformUtility.RectangleContainsScreenPoint(yesRect, Input.mousePosition, cam))
                ConfirmShuffle();
            else if (RectTransformUtility.RectangleContainsScreenPoint(noRect, Input.mousePosition, cam))
                CancelShuffle();
        }
    }
    
    public void Exit()
    {
        HandView.Instance.SetHandInteractable(true);
        // TURN OFF Yes/No UI buttons here
        _cm.shuffleYesButton.SetActive(false);
        _cm.shuffleNoButton.SetActive(false);
    }
}