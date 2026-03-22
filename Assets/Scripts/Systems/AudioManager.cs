using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Systems;

public class AudioManager : MonoBehaviour
{
    
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource effects;
    public AudioSource bgm;
    
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
        transform.SetParent(null);  // Detach from the parent so that DontDestroyOnLoad can work even when we put it under a parent for cleanliness.
        
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
    
    // Call this from CombatManager.Start() instead of using Start()
    public void SubscribeToScene()
    {
        CombatManager cm = FindAnyObjectByType<CombatManager>();
    
        // Clear old sound first to avoid double conflicts
        cm.OnTakeDamage.RemoveListener(PlayTakeDamage);
        cm.OnMirrorCrack.RemoveListener(PlayMirrorCracks);
        DeckManager.Instance.OnShuffle.RemoveListener(PlayShuffle);
        DeckManager.Instance.OnDraw.RemoveListener(PlayCardPlayed);
        CardViewHoverSystem.Instance.OnCardHover.RemoveListener(PlayHoverCard);

        // Find and attach sound with new scene's objects
        cm.OnTakeDamage.AddListener(PlayTakeDamage);
        cm.OnMirrorCrack.AddListener(PlayMirrorCracks);
        DeckManager.Instance.OnShuffle.AddListener(PlayShuffle);
        DeckManager.Instance.OnDraw.AddListener(PlayCardPlayed);
        CardViewHoverSystem.Instance.OnCardHover.AddListener(PlayHoverCard);
    }
    
    public void PlayFightMusic(AudioClip clip, float loopStartTime = 0f)
    {
        if (clip == null) return;
        bgm.clip = clip;
        bgm.loop = false;   // Handle looping manually bc some tracks are weirder
        bgm.Play();
        
        StartCoroutine(LoopTrack(loopStartTime));
    }
    
    private IEnumerator LoopTrack(float loopStartTime)
    {
        // Wait for the track to finish its intro (or full length if loopStartTime is 0)
        yield return new WaitForSeconds(bgm.clip.length - loopStartTime > 0 
            ? bgm.clip.length 
            : bgm.clip.length);
    
        while (true)
        {
            bgm.time = loopStartTime;
            bgm.Play();
            yield return new WaitForSeconds(bgm.clip.length - loopStartTime);
        }
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
        //ASSUMED MADNESS OF 7
        CombatManager cm = (CombatManager) FindAnyObjectByType(typeof(CombatManager));

        if (cm.madness <= cm.alice.maxMadness) {
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
        PlaySound(gainShield, 0.3f);
    }
    public void PlayGainStrength()
    {
        PlaySound(gainStrength, 0.6f);
    }
    public void PlayPoisonDamage()
    {
        PlaySound(poisonDamage, 1.5f);
    }
    
    
    private void PlaySound(AudioClip sound, float volumeMultiplier = 1f)
    {
        if (sound == null) return;
        float finalVolume = fxVolume * volumeMultiplier;
        
        // Add a tiny bit of pitch variance so every sound feels unique
        effects.pitch = Random.Range(0.95f, 1.05f);
        effects.PlayOneShot(sound, finalVolume);
    }
}

