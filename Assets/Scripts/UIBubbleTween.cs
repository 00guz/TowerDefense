using UnityEngine;
using DG.Tweening; // DOTween kütüphanesi

public class UIBubbleTween : MonoBehaviour
{
    [Header("Animasyon Ayarları")]
    public float duration = 0.4f; // Açılma süresi
    public Ease openEase = Ease.OutBack; // Açılırken "Baloncuk" efekti veren yaylanma
    public Ease closeEase = Ease.InBack; // Kapanırken içine çekilme efekti


    

    // Paneli "Baloncuk" efektiyle aç
    public void Open()
    {
        // 🔹 panelTransform YERİNE DİREKT 'transform' KULLANIN
        transform.localScale = Vector3.zero; 
        
        gameObject.SetActive(true);
        transform.DOKill();

        transform.DOScale(Vector3.one, duration).SetEase(openEase).SetUpdate(true);
    }

    public void Close()
    {
        transform.DOKill();

        transform.DOScale(Vector3.zero, duration)
            .SetEase(closeEase)
            .SetUpdate(true)
            .OnComplete(() => 
            {
                gameObject.SetActive(false);
            });
    }
}