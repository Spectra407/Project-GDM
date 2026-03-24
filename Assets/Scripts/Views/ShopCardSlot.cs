using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening;

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

        // Hide hover overlay in case this card was being hovered when bought
        CardViewHoverSystem.Instance.Hide();
        // Re-enable wrapper in case it was hidden by hover
        cardView.GetComponent<CardView>()?.gameObject.SetActive(true);

        AudioManager.instance.PlayBuyCard();

        BoxCollider col = cardView.GetComponent<BoxCollider>();
        if (col != null) col.enabled = false;

        StartCoroutine(SoldSequence());
    }
    
    private IEnumerator SoldSequence()
    {
        yield return StartCoroutine(cardView.ShakeAndHighlight(false));

        foreach (var sr in cardView.GetComponentsInChildren<SpriteRenderer>())
            sr.DOColor(new Color(0.3f, 0.3f, 0.3f, 1f), 0.3f);

        foreach (var tmp in cardView.GetComponentsInChildren<TMPro.TMP_Text>())
            tmp.DOColor(new Color(0.3f, 0.3f, 0.3f, 1f), 0.3f);
    }
}