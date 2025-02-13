using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public const string MASTER_KEY = "MasterVolume";
    public const string MUSIC_KEY = "MusicVolume";
    public const string SFX_KEY = "SFXVolume";


    [SerializeField] private AudioClip[] _audioClip;
    [SerializeField] private AudioClip[] _musicClip;  
    [SerializeField] private AudioSource _musicSource;  
    [SerializeField] private AudioSource[] _sfxSources;

    private int currentSFXSourceIndex = 0;


    private void Start()
    {
        LoadAudioSettings();
    }

    /// <summary>
    /// Function to call for playing a Music
    /// </summary>
    /// <param name="index"></param>
    public void PlayMusic(int index)
    {
        if (index >= 0 && index < _musicClip.Length) 
        {
            _musicSource.clip = _musicClip[index];
            _musicSource.loop = true;
            _musicSource.Play();
        }
    }

    /// <summary>
    /// Function to call to play a SFX clip
    /// </summary>
    /// <param name="index"></param>
    public void PlaySFX(int index)
    {
        if (index >= 0 && index < _audioClip.Length)
        {
            AudioSource availableSource = _sfxSources[currentSFXSourceIndex];
            availableSource.PlayOneShot(_audioClip[index]);
            currentSFXSourceIndex = (currentSFXSourceIndex + 1) % _sfxSources.Length;
        }
    }

    /// <summary>
    /// Function to call to stop the music
    /// </summary>
    public void StopMusic()
    {
        _musicSource?.Stop();
    }

    /// <summary>
    /// Function to load the audio settings from parameters
    /// </summary>
    public void LoadAudioSettings()
    {
        float masterVolume = PlayerPrefs.GetFloat(MASTER_KEY);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_KEY);
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_KEY);

        _musicSource.volume = musicVolume;
        foreach (var source in _sfxSources)
        {
            source.volume = sfxVolume;
        }
    }
}
