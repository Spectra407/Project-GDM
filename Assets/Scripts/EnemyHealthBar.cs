using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public EnemyData enemy;
    public CombatManager combatManager;
    public TextMeshProUGUI text;
    public GameObject bar;

    private float width;
    private float baseX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        width = bar.GetComponent<RectTransform>().rect.width / bar.GetComponent<Image>().sprite.pixelsPerUnit;
        baseX = bar.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        int currentHealth = combatManager.enemyCurrentHealth;
        int maxHealth = enemy.maxHealth;

        // Update text
        text.text = string.Format("{0:D2}/{1}", currentHealth, maxHealth);

        // Update bar position
        float adjustedX = baseX - width * (1 - (float) currentHealth / (float) maxHealth);
        bar.transform.position = new Vector3(adjustedX, bar.transform.position.y, bar.transform.position.z);
    }
}