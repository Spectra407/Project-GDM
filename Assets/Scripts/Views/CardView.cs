using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Rendering;

public class CardView : MonoBehaviour
{
    private static CardView _currentlyHovered;
    
    // boolean used to manipulate peeking cards
    public bool isPeek = false;
    
    
    [Header("Card UI References")]
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text madness;
    [SerializeField] private SpriteRenderer imageSR;
    [SerializeField] private GameObject wrapper;
    
    [Header("Card Background")]
    [SerializeField] private SpriteRenderer cardBackgroundSR;
    [SerializeField] private Sprite defaultCardSprite;
    [SerializeField] private Sprite bombCardSprite;
    [SerializeField] private Color defaultTextColor = new Color(0.541f, 0f, 0f, 1f);
    [SerializeField] private Color bombTextColor = Color.white;
    
    [Header("Card Hover stuff")]
    [HideInInspector] public Vector3 homePos;
    [HideInInspector] public Quaternion homeRot;
    private bool isHovered = false;
    public bool isShopCard = false;
    private Vector3 originalScale;
    
    [Header("Balatro Idle Animation")]
    [SerializeField] private float bounceSpeed = 2f;
    [SerializeField] private float bounceAmount = 0.05f;
    [SerializeField] private float tiltSpeed = 1.5f;
    [SerializeField] private float tiltAmount = 2f;
    private float randomOffset;
    [HideInInspector] public bool isAnimating = false;
    
    [Header("Card Data, do not drag anything into here manually")]
    public CardData data;
    
    void Start()
    {
        // Offset so every card doesn't move in perfect unison
        randomOffset = Random.Range(0f, 10f);
    }
    
    void Update()
    {
        // Keep collider anchored to homePos regardless of visual movement
        // Skip for shop cards (they don't have homePos logic) and animating cards
        if (!isShopCard && !isAnimating && homePos != Vector3.zero)
        {
            BoxCollider col = GetComponent<BoxCollider>();
            if (col != null)
            {
                Vector3 offset = homePos - transform.position;
                col.center = offset;
            }
        }
        else if (isShopCard)
        {
            // Reset collider center for shop cards so hover works normally
            BoxCollider col = GetComponent<BoxCollider>();
            if (col != null) col.center = Vector3.zero;
        }
        
        // Only animate passively if it's in the hand and NOT being hovered
        if (isHovered || isShopCard || isPeek || isAnimating) return;
        
        if (homePos == Vector3.zero) return;

        // Passive Vertical animation
        float yOffset = Mathf.Sin(Time.time * bounceSpeed + randomOffset) * bounceAmount;
    
        // Passive wobble
        float zWobble = Mathf.Cos(Time.time * tiltSpeed + randomOffset) * tiltAmount;

        // Apply relative to the home position set by HandView
        transform.position = homePos + new Vector3(0, yOffset, 0);
        transform.rotation = homeRot * Quaternion.Euler(0, 0, zWobble);
    }
    
    public void Setup(CardData newCardData)
    {
        data = newCardData;
       
        float s = CardViewCreator.Instance.scale;
        originalScale = new Vector3(s, s, s);
        
        // Update the visuals according to the newCardData
        description.text = data.description;
        madness.text = data.madness.ToString();
        imageSR.sprite = data.art;
        
        // Swap card background and text color based on bombs
        if (cardBackgroundSR != null)
            cardBackgroundSR.sprite = data.cardType.Contains(CardData.CardType.Bomb) ? bombCardSprite : defaultCardSprite;
        bool isBomb = data.cardType.Contains(CardData.CardType.Bomb);
        Color textColor = isBomb ? bombTextColor : defaultTextColor;
        description.color = textColor;
        madness.color = textColor;
    }

    void OnMouseEnter()
    {
        if (isAnimating) return;
        
        // Play hover sound
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayHoverCard();
        }
        
        // If this is a shop card, we use the Overlay System instead of moving it
        if (isShopCard)
        {
            // Calculate the position for the hover card (slightly to the side or centered)
            Vector3 hoverPos = transform.position; 
            CardViewHoverSystem.Instance.Show(data, hoverPos);
        
            // Optionally dim the current card or turn off the wrapper
            wrapper.SetActive(false); 
            return;
        }
        
        if (isPeek || HandView.Instance.isShattering || isHovered) return;
        // Force exit any previously hovered card that didn't get its OnMouseExit
        if (_currentlyHovered != null && _currentlyHovered != this)
            _currentlyHovered.ForceExit();

        _currentlyHovered = this;
        
        isHovered = true;

        transform.DOKill();
        transform.DOMove(homePos + Vector3.up * 0.8f, 0.15f).SetEase(Ease.OutBack);
        transform.DORotate(Vector3.zero, 0.15f);
        
        // Scale relative to its original size (130%)
        transform.DOScale(originalScale * 1.3f, 0.15f); 
        
        GetComponent<SortingGroup>().sortingOrder = 100; 
    }

    void OnMouseExit()
    {
        if (isAnimating) return;

        if (isShopCard)
        {
            CardViewHoverSystem.Instance.Hide();
            wrapper.SetActive(true);
            return;
        }

        if (isPeek || !isHovered) return;

        if (_currentlyHovered == this)
            _currentlyHovered = null;

        ExitHover();
    }
    
    public void ForceExit()
    {
        if (!isHovered) return;
        isHovered = false;
        ExitHover();
    }

    private void ExitHover()
    {
        isHovered = false;
        transform.DOKill();
        transform.DOMove(homePos, 0.15f);
        transform.DORotateQuaternion(homeRot, 0.15f);
        transform.DOScale(originalScale, 0.15f);

        if (TryGetComponent<SortingGroup>(out var sg))
            sg.sortingOrder = HandView.Instance.handCardViews.IndexOf(this);
    }
    
    // Use this to clear Peeked cards
    public void ClearVisuals()
    {
        data = null;
        description.text = "";
        madness.text = "";
        imageSR.sprite = null;
        
        
    }
    
    public IEnumerator ShakeAndHighlight(bool isBomb = false, System.Action onStart = null)
    {
        isAnimating = true;
        
        // Fire the sound callback immediately when the card pops
        onStart?.Invoke();

        Color highlightColor = isBomb ? new Color(1f, 0.3f, 0.3f) : new Color(1f, 0.95f, 0.7f);

        SortingGroup sg = GetComponent<SortingGroup>();
        int originalOrder = sg != null ? sg.sortingOrder : 0;
        if (sg != null) sg.sortingOrder = 200;

        transform.DOKill();
        transform.DOScale(originalScale * 1.35f, 0.12f).SetEase(Ease.OutBack);

        

        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in renderers)
            sr.DOColor(highlightColor, 0.1f);

        yield return new WaitForSeconds(0.12f);

        transform.DOShakePosition(0.4f, new Vector3(0.12f, 0.06f, 0), 18, 90, false, true);
        yield return new WaitForSeconds(0.45f);

        transform.DOScale(originalScale, 0.15f).SetEase(Ease.OutBack);
        foreach (var sr in renderers)
            sr.DOColor(Color.white, 0.15f);

        yield return new WaitForSeconds(0.15f);

        if (sg != null) sg.sortingOrder = originalOrder;
        isAnimating = false;
    }
    
    public GameObject CreateGhost()
    {
        GameObject ghost = new GameObject("CardGhost");
        ghost.transform.position = transform.position;
        ghost.transform.rotation = transform.rotation;
        ghost.transform.localScale = transform.localScale;

        // Copy card background
        if (cardBackgroundSR != null)
        {
            GameObject bgGhost = new GameObject("Background");
            bgGhost.transform.SetParent(ghost.transform, false);
            bgGhost.transform.localPosition = cardBackgroundSR.transform.localPosition;
            bgGhost.transform.localScale = cardBackgroundSR.transform.localScale;
            SpriteRenderer bgSR = bgGhost.AddComponent<SpriteRenderer>();
            bgSR.sprite = cardBackgroundSR.sprite;
            bgSR.color = new Color(1f, 1f, 1f, 0.4f);
            bgSR.sortingLayerName = cardBackgroundSR.sortingLayerName;
            bgSR.sortingOrder = cardBackgroundSR.sortingOrder + 50;
        }

        // Copy card art
        if (imageSR != null && imageSR.sprite != null)
        {
            GameObject artGhost = new GameObject("Art");
            artGhost.transform.SetParent(ghost.transform, false);
            artGhost.transform.localPosition = imageSR.transform.localPosition;
            artGhost.transform.localScale = imageSR.transform.localScale;
            SpriteRenderer artSR = artGhost.AddComponent<SpriteRenderer>();
            artSR.sprite = imageSR.sprite;
            artSR.color = new Color(1f, 1f, 1f, 0.4f);
            artSR.sortingLayerName = imageSR.sortingLayerName;
            artSR.sortingOrder = imageSR.sortingOrder + 50;
        }

        return ghost;
    }
    
}
