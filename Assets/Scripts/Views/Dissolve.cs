using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class Dissolve : MonoBehaviour
{
    [SerializeField] private float _dissolveTime = 0.75f;
    
    private SpriteRenderer[] _spriteRenderers;
    private Material[] _materials;
    
    private int _dissolveAmount = Shader.PropertyToID("_DissolveAmount");
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        
        _materials = new Material[_spriteRenderers.Length];
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            _materials[i] = _spriteRenderers[i].material;
            
        }
    }

    private IEnumerator Vanish()
    {
        float elapsedTime = 0f;
        while (elapsedTime < _dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            
            float lerpedDissolve = Mathf.Lerp(0, 1.1f, (elapsedTime / _dissolveTime));

            for (int i = 0; i < _materials.Length; i++)
            {
                _materials[i].SetFloat(_dissolveAmount, lerpedDissolve);
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
            
            float lerpedDissolve = Mathf.Lerp(1.1f, 0f, (elapsedTime / _dissolveTime));

            for (int i = 0; i < _materials.Length; i++)
            {
                _materials[i].SetFloat(_dissolveAmount, lerpedDissolve);
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
