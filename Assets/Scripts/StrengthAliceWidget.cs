using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class StrengthAliceWidget : MonoBehaviour
{
    [SerializeField] private CombatManager cm;
    [SerializeField] private TMP_Text strengthAliceText;
    private Image _image;
    private bool _isAnimating = false;
    private int _displayValue;

    void Awake()
    {
        _image = GetComponent<Image>();
        _displayValue = cm.strength;
    }

    void Update()
    {
        if (!_isAnimating)
        {
            _displayValue = cm.strength;
            strengthAliceText.text = $"{_displayValue}";
        }
    }

    public void Pulse(int newValue, Color flashColor)
    {
        _isAnimating = true;
        int oldValue = _displayValue;

        // Punch scale on the widget image
        transform.DOKill();
        transform.DOPunchScale(Vector3.one * 0.5f, 0.3f, 8, 0.5f);

        // Flash the image color
        _image.DOKill();
        _image.DOColor(flashColor, 0.1f).OnComplete(() =>
            _image.DOColor(Color.white, 0.3f));

        // Count up the number
        DOTween.To(() => _displayValue, x =>
        {
            _displayValue = x;
            strengthAliceText.text = $"{_displayValue}";
        }, newValue, 0.4f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            _displayValue = newValue;
            _isAnimating = false;
        });
    }
}