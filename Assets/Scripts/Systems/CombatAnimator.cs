using UnityEngine;
using DG.Tweening;
using System.Collections;

public class CombatAnimator : MonoBehaviour
{
    public static CombatAnimator Instance;

    [Header("Widget References")]
    [SerializeField] public StrengthAliceWidget strengthAliceWidget;
    [SerializeField] public BlockAliceWidget blockAliceWidget;
    [SerializeField] public StrengthEnemyWidget strengthEnemyWidget;
    [SerializeField] public BlockEnemyWidget blockEnemyWidget;
    [SerializeField] public PoisonEnemyWidget poisonEnemyWidget;

    [Header("Colors")]
    public Color damageColor = new Color(1f, 0.2f, 0.2f);
    public Color shieldColor = new Color(0.3f, 0.6f, 1f);
    public Color strengthColor = new Color(1f, 0.7f, 0.1f);
    public Color poisonColor = new Color(0.3f, 0.9f, 0.3f);

    void Awake()
    {
        Instance = this;
    }

    // Fires a ghost card projectile from a CardView toward a world-space target position
    public void FireCardProjectile(CardView sourceCard, Vector3 targetWorldPos, System.Action onArrival = null)
    {
        StartCoroutine(FireProjectileCoroutine(sourceCard, targetWorldPos, onArrival));
    }

    private IEnumerator FireProjectileCoroutine(CardView sourceCard, Vector3 targetWorldPos, System.Action onArrival)
    {
        targetWorldPos.z = sourceCard.transform.position.z;

        GameObject ghost = sourceCard.CreateGhost();
        ghost.transform.localScale = Vector3.one * 0.02f;

        float duration = 2f;
        ghost.transform.DOMove(targetWorldPos, duration).SetEase(Ease.InQuad);

        foreach (var sr in ghost.GetComponentsInChildren<SpriteRenderer>())
            sr.DOFade(0f, duration).SetEase(Ease.InQuad);

        yield return new WaitForSeconds(duration);

        onArrival?.Invoke();
        Destroy(ghost);
    }

    // Damage card: flies toward enemy, enemy flashes red
    public void PlayDamageEffect(CardView sourceCard, Vector3 enemyWorldPos, int newEnemyHealth)
    {
        FireCardProjectile(sourceCard, enemyWorldPos, () =>
        {
            PortraitAnimator.Instance.PlayEnemyHit();
            PortraitAnimator.Instance.FlashPortrait(false, damageColor);
        });
    }

    // Defense card — Alice flashes blue, block widget pulses
    public void PlayDefenseEffect(CardView sourceCard, int newAliceBlock)
    {
        PortraitAnimator.Instance.FlashPortrait(true, shieldColor);
        blockAliceWidget.Pulse(newAliceBlock, shieldColor);
    }

    // Strength card — Alice flashes orange, strength widget pulses
    public void PlayStrengthEffect(CardView sourceCard, int newStrength)
    {
        PortraitAnimator.Instance.FlashPortrait(true, strengthColor);
        strengthAliceWidget.Pulse(newStrength, strengthColor);
    }

    // Poison card — enemy flashes green, poison widget pulses
    public void PlayPoisonEffect(CardView sourceCard, int newPoison)
    {
        PortraitAnimator.Instance.FlashPortrait(false, poisonColor);
        poisonEnemyWidget.Pulse(newPoison, poisonColor);
    }

    // Enemy block widget pulses when enemy gains block
    public void PlayEnemyBlockEffect(int newEnemyBlock)
    {
        blockEnemyWidget.Pulse(newEnemyBlock, shieldColor);
    }

    // Enemy strength widget pulses when bomb gives enemy strength
    public void PlayEnemyStrengthEffect(int newEnemyStrength)
    {
        strengthEnemyWidget.Pulse(newEnemyStrength, strengthColor);
        PortraitAnimator.Instance.FlashPortrait(false, strengthColor);
    }
}