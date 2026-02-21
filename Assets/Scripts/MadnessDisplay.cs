using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MadnessDisplay : MonoBehaviour
{
    public AliceData alice;
    public CombatManager combatManager;
    public TextMeshProUGUI text;
    public Sprite[] sprites;
    private Image image;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = string.Format("{0}\n/\n{1}", combatManager.madness, alice.maxMadness);

        int sprite = combatManager.madness;
        if (sprite > 7) sprite = 8;
        image.sprite = sprites[sprite];
    }
}
