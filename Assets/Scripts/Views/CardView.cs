using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Rendering;

public class CardView : MonoBehaviour
{
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
        isHovered = true;

        transform.DOKill();
        transform.DOMove(homePos + Vector3.up * 0.8f, 0.15f).SetEase(Ease.OutBack);
        transform.DORotate(Vector3.zero, 0.15f);
        
        // Scale relative to its original size (120%)
        transform.DOScale(originalScale * 1.2f, 0.15f); 
        
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
        isHovered = false;

        transform.DOKill();
        transform.DOMove(homePos, 0.15f);
        transform.DORotateQuaternion(homeRot, 0.15f);
        transform.DOScale(originalScale, 0.15f);
    
        // Get the SortingGroup and put it back to its original depth
        if (TryGetComponent<SortingGroup>(out var sg))
        {
            // Calculate the index manually if you aren't storing it, 
            // or just pull it from the HandView list.
            sg.sortingOrder = HandView.Instance.handCardViews.IndexOf(this);
        }
    }
    
    // Use this to clear Peeked cards
    public void ClearVisuals()
    {
        data = null;
        description.text = "";
        madness.text = "";
        imageSR.sprite = null;
        
        
    }
    
    
    
}
