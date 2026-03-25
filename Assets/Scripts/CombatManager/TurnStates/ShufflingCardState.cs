using UnityEngine;
using System.Collections;
using Systems;

public class ShufflingCardState : ITurnState
{
    private CombatManager _cm;
    private bool _hasSelected = false;
    private bool _canSelect = false;

    public ShufflingCardState(CombatManager cm)
    {
        _cm = cm;
    }

    public void Enter()
    {
        Debug.Log("Choose a card to shuffle back into your deck.");
        CardViewHoverSystem.Instance.Hide();    // Hide the zoom effect thing so that it doesn't persist
        _hasSelected = false;
        _cm.StartCoroutine(EnableSelectionDelay());
        // TURN ON UI indicator here
    }

    private IEnumerator EnableSelectionDelay()
    {
        yield return new WaitForSeconds(0.3f);
        _canSelect = true;
    }

    public void Update()
    {
        if (!_hasSelected && _canSelect && Input.GetMouseButtonDown(0))
            DetectCardClick();
    }

    private void DetectCardClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            CardView clickedCard = hit.collider.GetComponentInParent<CardView>();
            if (clickedCard != null)
            {
                _hasSelected = true;
                ShuffleCard(clickedCard);
            }
        }
    }

    private void ShuffleCard(CardView cardView)
    {
        _cm.StartCoroutine(ShuffleCardAnimated(cardView));
    }

    private IEnumerator ShuffleCardAnimated(CardView cardView)
    {
        yield return HandView.Instance.AnimateCardToDeck(cardView);

        // STOP PULLING ATTENTION
        HandView.Instance.ClearAttentionCard();

        RecalculateHand();
        _cm.MoveToNewState("ChoosingAction");
    }

    private void RecalculateHand()
    {
        // Reset all pending stats and madness
        _cm.dem.ResetForJackpot();
        _cm.madness = _cm.turnBaseMadness;
        _cm.jackpot = false;

        _cm.dem.isreshuffling = true;

        // Process each card left to right exactly as if drawn fresh
        foreach (var cv in HandView.Instance.handCardViews)
        {
            if (cv == null || cv.data == null) continue;

            // Increment madness exactly like HandlingCardState.ProcessDraw
            _cm.madness += cv.data.madness;
            if (cv.data.madness > 0) _cm.OnMirrorCrack.Invoke();

            // Check jackpot status after each card's madness is applied
            _cm.dem.ProcessJackpot();

            // Resolve effects and stats: MadnessEffect, DisableEffect, etc fire correctly
            _cm.dem.ResolveOnDraw(cv.data);
        }

        _cm.dem.isreshuffling = false;
    }

    public void HandleInput(string inputID) { }

    public void Exit()
    {
        HandView.Instance.ClearAttentionCard();
    }
}