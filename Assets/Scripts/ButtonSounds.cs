using UnityEngine;
using UnityEngine.UI; // Button bileşenine erişmek için

[RequireComponent(typeof(Button))] // Bu script sadece Butonlarda çalışır
public class ButtonSound : MonoBehaviour
{
    [Header("Çalınacak Ses")]
    // AudioManager'da "TusSesi" adında bir ses tanımladığını varsayıyorum.
    // Eğer farklıysa Inspector'dan değiştirebilirsin.
    public int soundIndex = 0;
    private Button btn;

    private void Start()
    {
        btn = GetComponent<Button>();

        // Butonun tıklama olayına (onClick) kendi ses fonksiyonumuzu ekliyoruz
        btn.onClick.AddListener(PlaySound);
    }

    private void PlaySound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxClip[soundIndex]);
        }
    }
}