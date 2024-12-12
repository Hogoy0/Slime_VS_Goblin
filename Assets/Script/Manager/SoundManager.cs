using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    // BGM Á¾·ùµé
    public enum EBgm { BGM_TITLE, BGM_GAME }
    public enum SlimeESfx { Slime_Death, Slime_Merge, Slime_Launch, Slime_Landing, Slime_Attack }
    public enum GoblinESfx { Goblin_Death, Goblin_Bomb, Goblin_Drill, Goblin_DiggingTools, Goblin_Sheif }
    public enum UIESfx { UI_BasicBtn, UI_CostBtn }
    public enum EtcESfx { SFX_Win, SFX_Lose, SFX_WaveStart }

    [Serializable]
    public struct BgmClip
    {
        public EBgm bgmType;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
    }

    [Serializable]
    public struct SoundClip<T>
    {
        public T soundType;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
    }

    [SerializeField] private List<BgmClip> bgmClips = new List<BgmClip>();
    [SerializeField] private List<SoundClip<SlimeESfx>> slimeSounds = new List<SoundClip<SlimeESfx>>();
    [SerializeField] private List<SoundClip<GoblinESfx>> goblinSounds = new List<SoundClip<GoblinESfx>>();
    [SerializeField] private List<SoundClip<UIESfx>> uiSounds = new List<SoundClip<UIESfx>>();
    [SerializeField] private List<SoundClip<EtcESfx>> etcSounds = new List<SoundClip<EtcESfx>>();

    private Dictionary<EBgm, BgmClip> bgmDict = new Dictionary<EBgm, BgmClip>();
    private Dictionary<SlimeESfx, SoundClip<SlimeESfx>> slimeSoundDict = new Dictionary<SlimeESfx, SoundClip<SlimeESfx>>();
    private Dictionary<GoblinESfx, SoundClip<GoblinESfx>> goblinSoundDict = new Dictionary<GoblinESfx, SoundClip<GoblinESfx>>();
    private Dictionary<UIESfx, SoundClip<UIESfx>> uiSoundDict = new Dictionary<UIESfx, SoundClip<UIESfx>>();
    private Dictionary<EtcESfx, SoundClip<EtcESfx>> etcSoundDict = new Dictionary<EtcESfx, SoundClip<EtcESfx>>();

    [SerializeField] private AudioSource audioBgm;
    [SerializeField] private AudioSource audioSfx;

    private Dictionary<string, EBgm> sceneToBgmMap = new Dictionary<string, EBgm>
    {
        { "TitleScene", EBgm.BGM_TITLE },
        { "MainScene", EBgm.BGM_GAME }
    };

    private EBgm? currentBgm;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDictionaries();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void InitializeDictionaries()
    {
        foreach (var bgm in bgmClips)
        {
            if (!bgmDict.ContainsKey(bgm.bgmType))
            {
                bgmDict.Add(bgm.bgmType, bgm);
            }
        }

        AddToDictionary(slimeSounds, slimeSoundDict);
        AddToDictionary(goblinSounds, goblinSoundDict);
        AddToDictionary(uiSounds, uiSoundDict);
        AddToDictionary(etcSounds, etcSoundDict);
    }

    private void AddToDictionary<T>(List<SoundClip<T>> list, Dictionary<T, SoundClip<T>> dictionary)
    {
        foreach (var sound in list)
        {
            if (!dictionary.ContainsKey(sound.soundType))
            {
                dictionary.Add(sound.soundType, sound);
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (sceneToBgmMap.TryGetValue(scene.name, out EBgm bgm))
        {
            if (currentBgm != bgm || scene.name == "MainScene")
            {
                PlayBGM(bgm);
                currentBgm = bgm;
            }
        }
        else
        {
            Debug.LogWarning($"No BGM mapped for scene: {scene.name}");
        }
    }

    public void PlayBGM(EBgm bgmType)
    {
        if (bgmDict.TryGetValue(bgmType, out BgmClip bgm))
        {
            audioBgm.clip = bgm.clip;
            audioBgm.volume = bgm.volume;
            audioBgm.Play();
        }
        else
        {
            Debug.LogWarning($"BGM {bgmType} not found!");
        }
    }

    public void PlaySlimeESfx(SlimeESfx esfx)
    {
        PlaySound(slimeSoundDict, esfx);
    }

    public void PlayGoblinESfx(GoblinESfx esfx)
    {
        PlaySound(goblinSoundDict, esfx);
    }

    public void PlayUIESfx(UIESfx esfx)
    {
        PlaySound(uiSoundDict, esfx);
    }

    public void PlayEtcESfx(EtcESfx esfx)
    {
        PlaySound(etcSoundDict, esfx);
    }

    private void PlaySound<T>(Dictionary<T, SoundClip<T>> dictionary, T key)
    {
        if (dictionary.TryGetValue(key, out SoundClip<T> sound))
        {
            audioSfx.PlayOneShot(sound.clip, sound.volume);
        }
        else
        {
            Debug.LogWarning($"Sound {key} not found!");
        }
    }

    public void PlayUIESfxByIndex(int index)
    {
        if (Enum.IsDefined(typeof(UIESfx), index))
        {
            UIESfx soundType = (UIESfx)index;
            PlayUIESfx(soundType);
        }
        else
        {
            Debug.LogWarning($"Invalid UIESfx index: {index}");
        }
    }
}
