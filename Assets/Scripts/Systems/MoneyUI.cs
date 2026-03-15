using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private AliceData alice;
    [SerializeField] private TMP_Text goldText;

    void Update()
    {
        goldText.text = $"{alice.gold}";
    }
}