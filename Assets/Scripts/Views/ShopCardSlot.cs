using UnityEngine;
using TMPro;

public class ShopCardSlot : MonoBehaviour
{
    [SerializeField] private CardView cardView;
    [SerializeField] private TMP_Text priceText;

    public CardData card { get; private set; }
    public int price { get; private set; }
    private ShopManager _shop;

    public void Setup(CardData cardData, int cardPrice, ShopManager shop)
    {
        card = cardData;
        price = cardPrice;
        _shop = shop;

        cardView.Setup(card);
        cardView.homePos = cardView.transform.position;
        cardView.homeRot = cardView.transform.rotation;
        
        priceText.text = $"{price}g";
    }

    public void OnClickBuy()  // Use with button OnClick effect
    {
        _shop.TryBuyCard(this);
    }

    public void MarkAsSold()
    {
        if (_shop == null) return;
        priceText.text = "SOLD OUT";
    }
}