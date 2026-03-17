using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PortraitAnimator : MonoBehaviour
{
    public static PortraitAnimator Instance;

    [SerializeField] private RectTransform alicePortrait;
    [SerializeField] private RectTransform enemyPortrait;

    private Vector2 _aliceOrigin;
    private Vector2 _enemyOrigin;

    private void Awake()
    {
        Instance = this;
        _aliceOrigin = alicePortrait.anchoredPosition;
        _enemyOrigin = enemyPortrait.anchoredPosition;
    }

    public void PlayAliceAttack()
    {
        alicePortrait.DOAnchorPos(_aliceOrigin + new Vector2(80f, 0f), 0.15f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
                alicePortrait.DOAnchorPos(_aliceOrigin, 0.25f).SetEase(Ease.InQuad));
    }

    public void PlayEnemyAttack()
    {
        enemyPortrait.DOAnchorPos(_enemyOrigin + new Vector2(-80f, 0f), 0.15f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
                enemyPortrait.DOAnchorPos(_enemyOrigin, 0.25f).SetEase(Ease.InQuad));
    }
    
    public void PlayAliceHit()
    {
        alicePortrait.DOShakeAnchorPos(0.3f, strength: 15f, vibrato: 20);
    }

    public void PlayEnemyHit()
    {
        enemyPortrait.DOShakeAnchorPos(0.3f, strength: 15f, vibrato: 20);
    }
}