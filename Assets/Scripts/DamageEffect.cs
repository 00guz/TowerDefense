using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // DOTween kullanıyoruz

public class DamageEffect : MonoBehaviour
{
    public static DamageEffect Instance;

    [Header("Ayarlar")]
    [SerializeField] private Image flashImage; // Kırmızı panelimiz
    [SerializeField] private float flashDuration = 0.5f; // Ne kadar sürede kaybolsun
    [SerializeField] private float maxAlpha = 0.5f; // Kırmızılık ne kadar koyu olsun (0-1 arası)

    // Kırmızı renk (Alpha hariç)
    private Color defaultColor = Color.red;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (flashImage != null)
        {
            // Başlangıçta rengi ayarla ve tamamen şeffaf yap
            flashImage.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
            flashImage.raycastTarget = false; // Tıklamayı engellemesin (kodla da garanti edelim)
        }
    }

    public void TriggerFlash()
    {
        if (flashImage == null) return;

        // 1. Önceki animasyon varsa durdur (üst üste binerse bozulmasın)
        flashImage.DOKill();

        // 2. Rengi anında maksimum görünürlüğe getir
        flashImage.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, maxAlpha);

        // 3. DOTween ile yavaşça şeffaflaştır (Fade Out)
        flashImage.DOFade(0f, flashDuration).SetEase(Ease.OutQuad);
    }
}