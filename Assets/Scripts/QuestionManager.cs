using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    public GameObject questionPanel;
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;

    private System.Action<bool> callback; // Doğru/yanlış sonucunu TowerPlacementManager'a bildirir

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
                    questionPanel.SetActive(false);
                });
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }
}
