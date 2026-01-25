using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour
{
    
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text madness;
    [SerializeField] private SpriteRenderer imageSR;
    [SerializeField] private GameObject wrapper;
    [SerializeField] private TMP_Text topValue;
    [SerializeField] private SpriteRenderer topIcon;
    [SerializeField] private TMP_Text bottomValue;
    [SerializeField] private SpriteRenderer bottomIcon;

    [Header("All Icon Sprites")]
    [SerializeField] private Sprite damageSprite;
    [SerializeField] private Sprite defenseSprite;
    [SerializeField] private Sprite peekSprite;
    [SerializeField] private Sprite poisonSprite;
    
    [Header("Card Data, do not drag anything into here manually")]
    public CardData data;

    public void Setup(CardData newCardData)
    {
        data = newCardData;
        
        // Update the visuals according to the newCardData
        description.text = data.description;
        madness.text = data.madness.ToString();
        imageSR.sprite = data.art;
        
        // Update the top and bottom banners
        UpdateBanners();

    }

    private void UpdateBanners()
    {
        // Turn off the banners by default
        topValue.gameObject.transform.parent.gameObject.SetActive(false);
        bottomValue.gameObject.transform.parent.gameObject.SetActive(false);

        for (int i = 0; i < data.cardType.Count; i++)
        {
            if (i == 0)
            {
                topValue.gameObject.transform.parent.gameObject.SetActive(true);

                ChooseBannerType(topValue, topIcon, data.cardType[i]);
            }

            if (i == 1)
            {
                bottomValue.gameObject.transform.parent.gameObject.SetActive(true);
                ChooseBannerType(bottomValue, bottomIcon, data.cardType[i]);
            }
        }
    }

    private void ChooseBannerType(TMP_Text textSlot, SpriteRenderer iconSlot, CardData.CardType type)
    {
        switch (type)
        {
            case CardData.CardType.Damage:
                textSlot.text = data.damage.ToString();
                iconSlot.sprite = damageSprite;
                break;
            case CardData.CardType.Defense:
                textSlot.text = data.defense.ToString();
                iconSlot.sprite = defenseSprite;
                break;
            case CardData.CardType.Peek:
                textSlot.text = data.peek.ToString();
                iconSlot.sprite = peekSprite;
                break; 
            case CardData.CardType.Poison:
                textSlot.text = data.poison.ToString();
                iconSlot.sprite = poisonSprite;
                break;
            default:
                Debug.Log("Unknown card type");
                break;
        }
    }
}
