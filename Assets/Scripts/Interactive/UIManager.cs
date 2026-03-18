using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("Prompt")]
    public GameObject promptPanel;
    public TextMeshProUGUI promptText;

    [Header("Interaction Menu")]
    public GameObject interactionMenuPanel;
    public TextMeshProUGUI menuTitle;
    public Transform buttonsContainer;
    public GameObject buttonPrefab; // Префаб кнопки с компонентом InteractionButton

    [Header("Message")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;
    public float messageDuration = 2f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    #region Prompt
    public void ShowPrompt(string text)
    {
        if (promptPanel != null) promptPanel.SetActive(true);
        if (promptText != null) promptText.text = text;
    }

    public void HidePrompt()
    {
        if (promptPanel != null) promptPanel.SetActive(false);
    }
    #endregion

    #region Interaction Menu
    public void ShowInteractionMenu(string title, List<UIButtonData> buttons)
    {
        if (interactionMenuPanel == null) return;
        
        menuTitle.text = title;
        
        // Очищаем старые кнопки
        foreach (Transform child in buttonsContainer)
            Destroy(child.gameObject);
        
        // Создаём новые
        foreach (var btnData in buttons)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonsContainer);
            var btnComp = btnObj.GetComponent<InteractionButton>();
            
            if (btnComp != null)
                btnComp.Setup(btnData);
        }
        
        interactionMenuPanel.SetActive(true);
        Time.timeScale = 0; // Пауза игры
    }

    public void CloseInteractionMenu()
    {
        if (interactionMenuPanel != null)
            interactionMenuPanel.SetActive(false);
        Time.timeScale = 1;
    }
    #endregion

    #region Messages
    public void ShowMessage(string text)
    {
        if (messagePanel == null || messageText == null) return;
        
        messageText.text = text;
        messagePanel.SetActive(true);
        
        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), messageDuration);
    }

    private void HideMessage()
    {
        if (messagePanel != null)
            messagePanel.SetActive(false);
    }
    #endregion

    public void OnCloseButtonClicked()
    {
        CloseInteractionMenu();
    }

    // Обработка ESC
    private void Update()
    {
        if (interactionMenuPanel != null && interactionMenuPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseInteractionMenu();
            }
        }
    }
}