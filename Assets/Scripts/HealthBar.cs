using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public AliceData alice;
    public TextMeshProUGUI text;
    public GameObject bar;

    private float width;
    private float baseX;

    private float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        width = bar.GetComponent<RectTransform>().rect.width / bar.GetComponent<Image>().sprite.pixelsPerUnit;
        baseX = bar.transform.position.x;
        Debug.Log("Width: " + width + ", base X: " + baseX);
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        int currentHealth = alice.currentHealth;
        int maxHealth = alice.maxHealth;

        // Update text
        text.text = string.Format("{0:D2}/{1}", currentHealth, maxHealth);

        // Update bar position
        float adjustedX = baseX - width * (1 - (float) currentHealth / (float) maxHealth);
        bar.transform.position = new Vector3(adjustedX, bar.transform.position.y, bar.transform.position.z);

        timer += Time.deltaTime;
        if (timer > 1) Debug.Log("Set x to " + adjustedX);
        timer %= 1;
    }
}
