using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;

    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _musicSlider;

    public const string MIXER_MASTER = "MasterVolume";
    public const string MIXER_SFX = "SFXVolume";
    public const string MIXER_MUSIC = "MusicVolume";

    private float _defaultMasterVolume = 1f;
    private float _defaultSFXVolume = 0.8f;
    private float _defaultMusicVolume = 0.8f;

    private void Awake()
    {
        _masterSlider.onValueChanged.AddListener(SetMasterVolume);
        _sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        _musicSlider.onValueChanged.AddListener(SetMusicVolume);
        InitAudio();
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(AudioManager.MASTER_KEY, _masterSlider.value);
        PlayerPrefs.SetFloat(AudioManager.MUSIC_KEY, _musicSlider.value);
        PlayerPrefs.SetFloat(AudioManager.SFX_KEY, _sfxSlider.value);
    }

    
    public void InitAudio()
    {
        _masterSlider.value = PlayerPrefs.GetFloat(AudioManager.MASTER_KEY, _defaultMasterVolume);
        _sfxSlider.value = PlayerPrefs.GetFloat(AudioManager.SFX_KEY, _defaultSFXVolume);
        _musicSlider.value = PlayerPrefs.GetFloat(AudioManager.MUSIC_KEY, _defaultMusicVolume);
    }

    private void SetMasterVolume(float value)
    {
        float volume = Mathf.Lerp(-80f, 0f, value);
        _audioMixer.SetFloat(MIXER_MASTER, volume);
        PlayerPrefs.SetFloat(AudioManager.MASTER_KEY, _masterSlider.value);
    }

    private void SetSFXVolume(float value)
    {
        float volume = Mathf.Lerp(-80f, 0f, value);
        _audioMixer.SetFloat(MIXER_SFX, volume);
        PlayerPrefs.SetFloat(AudioManager.SFX_KEY, _sfxSlider.value);
    }

    private void SetMusicVolume(float value)
    {
        float volume = Mathf.Lerp(-80f, 0f, value);
        _audioMixer.SetFloat(MIXER_MUSIC, volume);
        PlayerPrefs.SetFloat(AudioManager.MUSIC_KEY, _musicSlider.value);
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }
}
