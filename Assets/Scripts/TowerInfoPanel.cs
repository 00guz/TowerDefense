using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerInfoPanel : MonoBehaviour
{
    [Header("UI Referansları")]
    public TextMeshProUGUI infoNameText;
    public TextMeshProUGUI infoDamageText;
    public TextMeshProUGUI infoRangeText;
    public TextMeshProUGUI infoFireRateText;
    public TextMeshProUGUI infoSoruText;
    public TextMeshProUGUI infoCostText;
    public Button closeButton;

    

    // Dışarıdan TowerData alıp paneli dolduran ve açan fonksiyon
    public void ShowInfo(TowerData tower)
    {
        if (tower == null) return;

        infoNameText.text = tower.towerName;
        infoDamageText.text = $"Hasar: {tower.damage}";
        infoRangeText.text = $"Menzil: {tower.range}";
        infoFireRateText.text = $"Hız: {tower.fireRate}";
        infoSoruText.text = $"Soru: {tower.soruSayisi}";
        infoCostText.text = $"Altın: {tower.cost}";
        
        // Paneli göster
        gameObject.SetActive(true);
    }

    // Paneli gizleyen fonksiyon
    public void Hide()
    {
        gameObject.SetActive(false);

        if (TowerMenu.Instance != null && TowerMenu.Instance.toggleButton != null)
        {
            TowerMenu.Instance.toggleButton.interactable = true;
        }
    }
}