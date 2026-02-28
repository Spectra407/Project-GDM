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

    [Header("Background Music")]
    public AudioClip fightMusic;
    [Header("Sound Effects")]
    public AudioClip hover;
    public AudioClip click;
    public AudioClip shuffle;
    public AudioClip cardPlayed;
    public AudioClip takeDamage;
    public AudioClip hoverCard;
    public List<AudioClip> mirrorCracks;
    [Header("Volume Settings")]
    public float masterVolume = 1f;
    public float bgmVolume = 1f;
    public float fxVolume = 1f;

    void Awake()
    {
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
    
    private void Start()
    {
        CombatManager cm = (CombatManager) FindAnyObjectByType(typeof(CombatManager));
        cm.OnTakeDamage.AddListener(PlayTakeDamage);
        cm.OnMirrorCrack.AddListener(PlayMirrorCracks);
        DeckManager.Instance.OnShuffle.AddListener(PlayShuffle);
        DeckManager.Instance.OnDraw.AddListener(PlayCardPlayed);
        CardViewHoverSystem.Instance.OnCardHover.AddListener(PlayHoverCard);
    }
    public void PlayFightMusic()
    {
        PlaySound(fightMusic);
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
        PlaySound(hoverCard);
    }
    public void PlayTakeDamage()
    {
        PlaySound(takeDamage);
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
    private void PlaySound(AudioClip sound)
    {
        effects.PlayOneShot(sound);
    }
}

