using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Sound Clips")]
    public AudioClip backgroundMusic;
    public AudioClip pulseSound;
    public AudioClip touchSound;
    public AudioClip messageOpenSound;
    public AudioClip messageCloseSound;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.35f;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;

    public static AudioManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Defensive check — make sure sources are not the same object
        if (musicSource == sfxSource)
        {
            Debug.LogError("AudioManager: musicSource and sfxSource are the same! " +
                           "Assign them to separate GameObjects.");
        }
    }

    void Start()
    {
        if (musicSource == null || sfxSource == null)
        {
            Debug.LogError("AudioManager: Audio sources not assigned.");
            return;
        }

        // Make absolutely sure the SFX source has no clip assigned
        // so it can never accidentally play or interrupt music
        sfxSource.clip = null;
        sfxSource.loop = false;

        // Start music
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.ignoreListenerPause = true; // Never pause with game pause
            musicSource.Play();
        }
    }

    public void PlayPulse()
    {
        PlaySFX(pulseSound);
    }

    public void PlayTouch()
    {
        PlaySFX(touchSound);
    }

    public void PlayMessageOpen()
    {
        PlaySFX(messageOpenSound);
    }

    public void PlayMessageClose()
    {
        PlaySFX(messageCloseSound);
    }

    void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        // PlayOneShot on the dedicated SFX source only
        // This never touches musicSource
        sfxSource.PlayOneShot(clip, sfxVolume);
    }
}