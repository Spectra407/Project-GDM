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
    public float bgmVolume = 0.65f;
    public float fxVolume = 1f;

    // Allows the FadeScript to fade the entire BGM system
    private float _fadeMultiplier = 1f; 
	private float lastCardSoundTime = 0f;
	private float cardSoundDelay = 0.15f;

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

    // Call this from FadeScript to fade music in/out globally
    public void SetFadeMultiplier(float value)
    {
        _fadeMultiplier = value;
        if (bgmA.isPlaying) bgmA.volume = bgmVolume * _fadeMultiplier;
        if (bgmB.isPlaying) bgmB.volume = bgmVolume * _fadeMultiplier;
    }

    public void SubscribeToScene()
    {
        CombatManager cm = FindAnyObjectByType<CombatManager>();
        if (cm == null) return;

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

        // Apply the fade multiplier to the starting volume
        _current.volume = bgmVolume * _fadeMultiplier;
        _next.volume = 0f;

        double dspStartTime = AudioSettings.dspTime + 0.1;

        // Play intro
        _current.clip = intro;
        _current.loop = false;
        _current.PlayScheduled(dspStartTime);

        // Start loop slightly BEFORE intro ends
        double overlap = 0.02; 
        double loopStartTime = dspStartTime + (double)intro.length - overlap;

        // Schedule loop
        _next.clip = loop;
        _next.loop = true;
        _next.volume = 0f;
        _next.PlayScheduled(loopStartTime);

        // Fade IN loop
        StartCoroutine(FadeInAtDSPTime(_next, (float)overlap, loopStartTime));

        // Fade OUT intro
        StartCoroutine(FadeOutAtDSPTime(_current, (float)overlap, loopStartTime));
    }
    
    IEnumerator FadeInAtDSPTime(AudioSource source, float duration, double dspStartTime)
    {
        while (AudioSettings.dspTime < dspStartTime) yield return null;

        float t = 0f;
        source.volume = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            source.volume = (Mathf.Clamp01(t / duration) * bgmVolume) * _fadeMultiplier;
            yield return null;
        }

        source.volume = bgmVolume * _fadeMultiplier;
    }

    IEnumerator FadeOutAtDSPTime(AudioSource source, float duration, double dspFadeStartTime)
    {
        while (AudioSettings.dspTime < dspFadeStartTime) yield return null;

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

    
    public void PlayBuyCard() => PlaySound(buySFX, 2f);
    public void PlayHover() => PlaySound(hover);
    public void PlayClick() => PlaySound(click);
    public void PlayShuffle()
	{
    	if (Time.time - lastCardSoundTime < cardSoundDelay) return;
    	lastCardSoundTime = Time.time;
    	PlaySound(shuffle);
	}

	public void PlayCardPlayed() 
	{
    	if (Time.time - lastCardSoundTime < cardSoundDelay) return;
    	lastCardSoundTime = Time.time;
    	PlaySound(cardPlayed);
	}
    public void PlayHoverCard() => PlaySound(hoverCard, 0.4f);
    public void PlayTakeDamage() => PlaySound(takeDamage, 2f);
    public void PlayBluntDamage() => PlaySound(bluntTakeDamage);

    public void PlayMirrorCracks()
    {
        CombatManager cm = FindAnyObjectByType<CombatManager>();
        if (cm == null) return;

        int index = Mathf.Clamp(cm.madness - 1, 0, mirrorCracks.Count - 1);
        PlaySound(mirrorCracks[index]);
    }

    public void PlayGainShield() => PlaySound(gainShield, 0.8f);
    public void PlayGainStrength() => PlaySound(gainStrength, 0.8f);
    public void PlayPoisonDamage() => PlaySound(poisonDamage, 1.8f);

    private void PlaySound(AudioClip sound, float volumeMultiplier = 1f)
    {
        if (sound == null) return;
        effects.pitch = Random.Range(0.95f, 1.05f);
        effects.PlayOneShot(sound, fxVolume * volumeMultiplier * masterVolume);
    }
}