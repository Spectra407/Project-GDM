using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardView : MonoBehaviour
{
    // boolean used to manipulate peeking cards
    public bool isPeek = false;
    
    [Header("Card UI References")]
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text madness;
    [SerializeField] private SpriteRenderer imageSR;
    [SerializeField] private GameObject wrapper;
    
    
    [Header("Card Data, do not drag anything into here manually")]
    public CardData data;

    public void Setup(CardData newCardData)
    {
        data = newCardData;
        
        // Update the visuals according to the newCardData
        description.text = data.description;
        madness.text = data.madness.ToString();
        imageSR.sprite = data.art;
        
        

    }

    void OnMouseEnter()
    {
        if (!isPeek)
        {
            wrapper.SetActive(false);
            Vector3 pos = new(transform.position.x, 0, 0);
            CardViewHoverSystem.Instance.Show(data, pos);
        }
        

    }

    void OnMouseExit()
    {
        if (!isPeek)
        {
            CardViewHoverSystem.Instance.Hide();
            wrapper.SetActive(true);
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
