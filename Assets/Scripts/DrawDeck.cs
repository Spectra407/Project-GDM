using Systems;
using UnityEngine;
using System.Collections.Generic;

public class DrawDeck : MonoBehaviour
{
    public GameObject cardPrefab;

    public Vector3 positionShift;

    private List<GameObject> cards;
    private int myCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cards = new();
    }

    // Update is called once per frame
    void Update()
    {
        if (DeckManager.Instance == null) return;
        
        int count = DeckManager.Instance.drawPile.Count;
        if (count < myCount)
        {
            while (count != myCount)
            {
                Destroy(cards[myCount - 1]);
                cards.RemoveAt(myCount - 1);
                myCount--;
            }
        }
        else if (count > myCount)
        {
            while (count != myCount)
            {
                GameObject card = Instantiate(cardPrefab);
                card.transform.position += gameObject.transform.position + myCount * positionShift;
                cards.Add(card);
                Debug.Log("spawned card at " + card.transform.position);
                myCount++;
            }
        }
    }
}
