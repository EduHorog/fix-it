using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {
    public static UIManager Instance;
    
    [Header("Notifications")]
    public TextMeshProUGUI notificationText;
    public GameObject notificationPanel;
    
    [Header("Choice Panel")]
    public GameObject choicePanel;
    public Text choiceTitle;
    public Text requirementText;
    public Button fixButton;
    public Button hideButton;
    public Text fixButtonText;
    public Text hideButtonText;
    
    private System.Action onFix;
    private System.Action onHide;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        if (fixButton != null) {
            fixButton.onClick.AddListener(() => {
                if (choicePanel != null) choicePanel.SetActive(false);
                onFix?.Invoke();
            });
        }
        
        if (hideButton != null) {
            hideButton.onClick.AddListener(() => {
                if (choicePanel != null) choicePanel.SetActive(false);
                onHide?.Invoke();
            });
        }
    }

    public void ShowNotification(string text) {
        if (notificationText != null) {
            notificationText.text = text;
            if (notificationPanel != null) {
                notificationPanel.SetActive(true);
                CancelInvoke(nameof(HideNotification));
                Invoke(nameof(HideNotification), 2f);
            }
        }
        Debug.Log($"[UI] {text}");
    }

    void HideNotification() {
        if (notificationPanel != null) notificationPanel.SetActive(false);
    }

    // Метод для игрушки (Pickup режим)
    public void ShowChoicePanel(string title, string option1, string option2, 
                                System.Action onOption1, System.Action onOption2) {
        if (choicePanel == null) {
            Debug.LogError("Choice Panel not assigned!");
            return;
        }
        
        if (choiceTitle != null) choiceTitle.text = title;
        if (requirementText != null) {
            requirementText.text = "Выберите действие:";
            requirementText.color = Color.white;
        }
        
        if (fixButtonText != null) fixButtonText.text = option1;
        if (hideButtonText != null) hideButtonText.text = option2;
        
        onFix = onOption1;
        onHide = onOption2;
        choicePanel.SetActive(true);
    }
    
    public void ShowEnding(string type) {
        Debug.Log($"КОНЦОВКА: {type}");
        Time.timeScale = 0f;
        ShowNotification(type == "good" ? "ХОРОШАЯ КОНЦОВКА" : "ПЛОХАЯ КОНЦОВКА");
    }
}