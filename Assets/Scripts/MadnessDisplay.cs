using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class MadnessDisplay : MonoBehaviour
{
    public static MadnessDisplay Instance;

    public AliceData alice;
    public CombatManager combatManager;
    public TextMeshProUGUI text;
    public Sprite[] sprites;
    private Image image;
    private RectTransform _rect;
    private Vector2 _origin;

    void Start()
    {
        Instance = this;
        image = GetComponent<Image>();
        _rect = GetComponent<RectTransform>();
        _origin = _rect.anchoredPosition;
    }

    void Update()
    {
        text.text = string.Format("{0}\n/\n{1}", combatManager.madness, alice.maxMadness);
        int sprite = combatManager.madness;
        if (sprite > 7) sprite = 8;
        image.sprite = sprites[sprite];
    }

    public void PulseMadness()
    {
        // Violent shake
        _rect.DOKill();
        _rect.DOShakeAnchorPos(0.6f, strength: 40f, vibrato: 25, randomness: 90);

        // Large punch scale
        transform.DOKill();
        transform.DOPunchScale(Vector3.one * 0.4f, 0.5f, 8, 0.5f);

        // Flash red then back
        image.DOKill();
        image.DOColor(new Color(1f, 0.15f, 0.15f), 0.1f).OnComplete(() =>
            image.DOColor(Color.white, 0.5f));
    }
}