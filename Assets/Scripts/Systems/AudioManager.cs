using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Systems;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource effects;
    public AudioSource bgmA;
    public AudioSource bgmB;

    private AudioSource _current;
    private AudioSource _next;

    [Header("Sound Effects")]
    public AudioClip hover;
    public AudioClip click;
    public AudioClip shuffle;
    public AudioClip cardPlayed;
    public AudioClip takeDamage;
    public AudioClip bluntTakeDamage;
    public AudioClip hoverCard;
    public List<AudioClip> mirrorCracks;
    public AudioClip gainShield;
    public AudioClip gainStrength;
    public AudioClip poisonDamage;
    public AudioClip buySFX;

    [Header("Volume Settings")]
    public float masterVolume = 1f;
    public float bgmVolume = 1f;
    public float fxVolume = 1f;

    void Awake()
    {
        transform.SetParent(null);

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    

    public void SubscribeToScene()
    {
        CombatManager cm = FindAnyObjectByType<CombatManager>();

        cm.OnTakeDamage.RemoveListener(PlayTakeDamage);
        cm.OnMirrorCrack.RemoveListener(PlayMirrorCracks);
        DeckManager.Instance.OnShuffle.RemoveListener(PlayShuffle);
        DeckManager.Instance.OnDraw.RemoveListener(PlayCardPlayed);
        CardViewHoverSystem.Instance.OnCardHover.RemoveListener(PlayHoverCard);

        cm.OnTakeDamage.AddListener(PlayTakeDamage);
        cm.OnMirrorCrack.AddListener(PlayMirrorCracks);
        DeckManager.Instance.OnShuffle.AddListener(PlayShuffle);
        DeckManager.Instance.OnDraw.AddListener(PlayCardPlayed);
        CardViewHoverSystem.Instance.OnCardHover.AddListener(PlayHoverCard);
    }

    public void PlayFightMusic(AudioClip intro, AudioClip loop)
    {
        if (intro == null || loop == null) return;

        StopAllCoroutines();

        bgmA.Stop();
        bgmB.Stop();

        _current = bgmA;
        _next = bgmB;

        _current.volume = bgmVolume;
        _next.volume = 0f;

        double dspStartTime = AudioSettings.dspTime + 0.1;

        
        // Play intro
        _current.clip = intro;
        _current.loop = false;
        _current.PlayScheduled(dspStartTime);

        // Start loop slightly BEFORE intro ends
        double overlap = 0.02; // 20 ms, 10ms has a crackle :(

        double loopStartTime = dspStartTime + intro.length - overlap;

        // Schedule loop
        _next.clip = loop;
        _next.loop = true;
        _next.volume = 0f;
        _next.PlayScheduled(loopStartTime);

        // Fade IN loop immediately (it starts at loopStartTime anyway)
        StartCoroutine(FadeInAtDSPTime(
            _next,
            (float)overlap,
            loopStartTime
        ));

        // Fade OUT intro at the correct DSP time
        StartCoroutine(FadeOutAtDSPTime(
            _current,
            (float)overlap,
            loopStartTime
        ));
    }
    
    IEnumerator FadeInAtDSPTime(AudioSource source, float duration, double dspStartTime)
    {
        // Wait until the audio actually starts
        while (AudioSettings.dspTime < dspStartTime)
        {
            yield return null;
        }

        float t = 0f;
        source.volume = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Clamp01(t / duration) * bgmVolume;
            yield return null;
        }

        source.volume = bgmVolume;
    }

    IEnumerator FadeOutAtDSPTime(AudioSource source, float duration, double dspFadeStartTime)
    {
        // Wait until DSP time reaches fade start
        while (AudioSettings.dspTime < dspFadeStartTime)
        {
            yield return null;
        }

        float startVol = source.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVol, 0f, t / duration);
            yield return null;
        }

        source.volume = 0f;
        source.Stop();
    }

    public void PlayBuyCard()
    {
        PlaySound(buySFX, 2f);
    }

    public void PlayHover()
    {
        PlaySound(hover);
    }

    public void PlayClick()
    {
        PlaySound(click);
    }

    public void PlayShuffle()
    {
        PlaySound(shuffle);
    }

    public void PlayCardPlayed()
    {
        PlaySound(cardPlayed);
    }

    public void PlayHoverCard()
    {
        PlaySound(hoverCard, 0.4f);
    }

    public void PlayTakeDamage()
    {
        PlaySound(takeDamage, 2f);
    }

    public void PlayBluntDamage()
    {
        PlaySound(bluntTakeDamage);
    }

    public void PlayMirrorCracks()
    {
        CombatManager cm = (CombatManager)FindAnyObjectByType(typeof(CombatManager));

        if (cm.madness <= cm.alice.maxMadness)
        {
            Debug.Log("Playing mirror crack sound number " + (cm.madness - 1));
            PlaySound(mirrorCracks[cm.madness - 1]);
        }
        else
        {
            PlaySound(mirrorCracks[cm.alice.maxMadness]);
        }
    }

    public void PlayGainShield()
    {
        PlaySound(gainShield, 0.8f);
    }

    public void PlayGainStrength()
    {
        PlaySound(gainStrength, 0.8f);
    }

    public void PlayPoisonDamage()
    {
        PlaySound(poisonDamage, 1.8f);
    }

    private void PlaySound(AudioClip sound, float volumeMultiplier = 1f)
    {
        if (sound == null) return;
        float finalVolume = fxVolume * volumeMultiplier;
        effects.pitch = Random.Range(0.95f, 1.05f);
        effects.PlayOneShot(sound, finalVolume);
    }
}