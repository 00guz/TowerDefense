using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource; // Arkaplan müziği için
    [SerializeField] private AudioSource sfxSource;   // Efektler (atış, patlama) için

    public AudioClip[] musicClip;
    public AudioClip[] sfxClip;

    [Header("Varsayılan Ayarlar")]
    public float defaultMusicVolume = 0.5f;
    public float defaultSFXVolume = 0.5f;

    private void Awake()
    {
        // Singleton Yapısı (Sahne geçişlerinde yok olmasın)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Kayıtlı ses ayarlarını yükle (Yoksa varsayılanı al)
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", defaultSFXVolume);

        ApplyMusicVolume(musicVol);
        ApplySFXVolume(sfxVol);
    }

    // --------------------------------------------------------
    // 🎵 MÜZİK FONKSİYONLARI
    // --------------------------------------------------------
    
    // Arkaplan müziğini değiştirir
    public void PlayMusic(AudioClip clip)
    {
        // Eğer zaten aynı müzik çalıyorsa tekrar başlatma
        if (musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    // Slider'dan gelen değeri uygular ve kaydeder
    public void SetMusicVolume(float volume)
    {
        ApplyMusicVolume(volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    private void ApplyMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    // Diğer scriptlerden mevcut ses düzeyini okumak için
    public float GetMusicVolume()
    {
        return musicSource.volume;
    }

    // --------------------------------------------------------
    // 🔊 SFX (SES EFEKTİ) FONKSİYONLARI
    // --------------------------------------------------------

    // Tek seferlik efekt çalar (Atış, Patlama, UI Tık)
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        // PlayOneShot, aynı anda birden fazla sesin üst üste binmesine izin verir
        sfxSource.PlayOneShot(clip);
    }

    // Slider'dan gelen değeri uygular ve kaydeder
    public void SetSFXVolume(float volume)
    {
        ApplySFXVolume(volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    private void ApplySFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    public float GetSFXVolume()
    {
        return sfxSource.volume;
    }
}