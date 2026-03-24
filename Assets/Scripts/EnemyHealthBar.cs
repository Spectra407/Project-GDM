using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyHealthBar : MonoBehaviour
{
    public static EnemyHealthBar Instance;

    public EnemyData enemy;
    public CombatManager combatManager;
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
        text.text = string.Format("{0:D2}/{1}", combatManager.enemyCurrentHealth, enemy.maxHealth);

        if (!_isAnimating)
        {
            float adjustedX = baseX + width * (1 - (float)combatManager.enemyCurrentHealth / (float)enemy.maxHealth);
            bar.transform.position = new Vector3(adjustedX, bar.transform.position.y, bar.transform.position.z);
        }
    }

    public void AnimateToCurrentHealth()
    {
        _isAnimating = true;
        float targetX = baseX + width * (1 - (float)combatManager.enemyCurrentHealth / (float)enemy.maxHealth);
        bar.transform.DOMoveX(targetX, 0.4f).SetEase(Ease.OutQuad).OnComplete(() =>
            _isAnimating = false);
    }
}