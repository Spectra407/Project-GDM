using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AliceHealthBar : MonoBehaviour
{
    public static AliceHealthBar Instance;

    public AliceData alice;
    public TextMeshProUGUI text;
    public GameObject bar;

    private float width;
    private float baseX;
    private bool _isAnimating = false;

    void Start()
    {
        Instance = this;
        width = bar.GetComponent<RectTransform>().rect.width / bar.GetComponent<Image>().sprite.pixelsPerUnit;
        baseX = bar.transform.position.x;
    }

    void Update()
    {
        text.text = string.Format("{0:D2}/{1}", alice.currentHealth, alice.maxHealth);

        if (!_isAnimating)
        {
            float adjustedX = baseX - width * (1 - (float)alice.currentHealth / (float)alice.maxHealth);
            bar.transform.position = new Vector3(adjustedX, bar.transform.position.y, bar.transform.position.z);
        }
    }

    public void AnimateToCurrentHealth()
    {
        _isAnimating = true;
        float targetX = baseX - width * (1 - (float)alice.currentHealth / (float)alice.maxHealth);
        bar.transform.DOMoveX(targetX, 0.4f).SetEase(Ease.OutQuad).OnComplete(() =>
            _isAnimating = false);
    }
}