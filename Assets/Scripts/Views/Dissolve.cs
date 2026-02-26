using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class Dissolve : MonoBehaviour
{
    [SerializeField] private float _dissolveTime = 0.75f;
    
    private SpriteRenderer[] _spriteRenderers;
    private Material[] _materials;
    private TextMeshPro[] _texts;
    
    private int _dissolveAmount = Shader.PropertyToID("_DissolveAmount");
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        _texts = GetComponentsInChildren<TextMeshPro>(); //
        
        _materials = new Material[_spriteRenderers.Length];
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            _materials[i] = _spriteRenderers[i].material;
            
        }
    }

    public void StartVanish()
    {
        StartCoroutine(Vanish());
    }

    public void StartAppear()
    {
        StartCoroutine(Appear());
    }

    private IEnumerator Vanish()
    {
        float elapsedTime = 0f;
        while (elapsedTime < _dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _dissolveTime;
            
            float lerpedDissolve = Mathf.Lerp(0, 1.1f, t);

            for (int i = 0; i < _materials.Length; i++)
            {
                _materials[i].SetFloat(_dissolveAmount, lerpedDissolve);
            }
            
            
            // TMP Fade logic (1 to 0 alpha)
            float lerpedAlpha = Mathf.Lerp(1, 0, t);
            foreach (var text in _texts)
            {
                text.alpha = lerpedAlpha; 
            }
            yield return null;
        }
    }
    
    private IEnumerator Appear()
    {
        float elapsedTime = 0f;
        while (elapsedTime < _dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _dissolveTime;
            
            float lerpedDissolve = Mathf.Lerp(1.1f, 0f, t);

            for (int i = 0; i < _materials.Length; i++)
            {
                _materials[i].SetFloat(_dissolveAmount, lerpedDissolve);
            }
            // TMP Fade logic (0 to 1 alpha)
            float lerpedAlpha = Mathf.Lerp(0, 1, t);
            foreach (var text in _texts)
            {
                text.alpha = lerpedAlpha;
            }
            
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartCoroutine(Vanish());
        }

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            StartCoroutine(Appear());
        }
    }
}
