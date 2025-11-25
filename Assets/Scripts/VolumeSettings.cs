using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [Header("UI Slider Referansları")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // 1. AudioManager'ın hazır olduğundan emin ol
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("Sahnede AudioManager yok!");
            return;
        }

        // 2. Slider'ları mevcut ses düzeyine göre ayarla
        // (Böylece menü açıldığında slider 0'da değil, kayıtlı değerde durur)
        musicSlider.value = AudioManager.Instance.GetMusicVolume();
        sfxSlider.value = AudioManager.Instance.GetSFXVolume();

        // 3. Slider'lara dinleyici ekle
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    // Slider her hareket ettiğinde bu çalışır
    public void SetMusicVolume(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    // Slider her hareket ettiğinde bu çalışır
    public void SetSFXVolume(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }
}