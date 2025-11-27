
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    public GameObject questionPanel;
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public GameObject dogruImage;
    public GameObject yanlisImage;

    private System.Action<bool> callback; // Doğru/yanlış sonucunu TowerManager'a bildirir

    [System.Serializable]
    public class Question
    {
        public string question;
        public string[] answers;
        public int correctIndex;
    }

    // Örnek sorular
    public Question[] sampleQuestions = new Question[]
    {
        new Question
        {
            question = "2 + 2 kaçtır?",
            answers = new string[] { "3", "4", "5", "6" },
            correctIndex = 1
        },
        new Question
        {
            question = "Dünyanın uydusu hangisidir?",
            answers = new string[] { "Mars", "Ay", "Venüs", "Jüpiter" },
            correctIndex = 1
        },
        new Question
        {
            question = "Renkler arasında sıcak renk hangisidir?",
            answers = new string[] { "Mavi", "Yeşil", "Kırmızı", "Mor" },
            correctIndex = 2
        },
        new Question
        {
            question = "Hangi hayvan uçabilir?",
            answers = new string[] { "Fil", "Kedi", "Kartal", "Köpek" },
            correctIndex = 2
        },
        new Question
        {
            question = "Su hangi sıcaklıkta donar?",
            answers = new string[] { "0°C", "100°C", "50°C", "-10°C" },
            correctIndex = 0
        }
    };


    public void AskQuestion(System.Action<bool> resultCallback)
    {
        callback = resultCallback;

        // Paneli aç
        UIBubbleTween bubble = questionPanel.GetComponent<UIBubbleTween>();
        if (bubble != null) 
            bubble.Open();
        else 
            questionPanel.SetActive(true);

        // Rastgele soru seç
        Question q = sampleQuestions[Random.Range(0, sampleQuestions.Length)];

        questionText.text = q.question;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < q.answers.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = q.answers[i];

                int index = i; // closure için kopyala
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() =>
                {
                    bool correct = (index == q.correctIndex);
                    callback?.Invoke(correct);
                    
                });
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void ShowResult(bool correct, System.Action onComplete)
    {
        if (correct)
        {
            dogruImage.SetActive(true);
            yanlisImage.SetActive(false);
        }
        else
        {
            dogruImage.SetActive(false);
            yanlisImage.SetActive(true);
        }

        // Coroutine'e bu eylemi gönderiyoruz
        StartCoroutine(HideResultAfterDelay(onComplete));
    }

    private IEnumerator HideResultAfterDelay(System.Action onComplete)
    {
        // Sonucu gösterme süresi (Örn: 1 saniye bekle)
        yield return new WaitForSeconds(0.5f); 

        dogruImage.SetActive(false);
        yanlisImage.SetActive(false);

        
        UIBubbleTween bubble = questionPanel.GetComponent<UIBubbleTween>();
        if (bubble != null) 
            bubble.Close();
        else 
            questionPanel.SetActive(false);

        // 🔹 SÜRE BİTTİ! Şimdi TowerManager'a haber ver
        onComplete?.Invoke(); 
    }
}