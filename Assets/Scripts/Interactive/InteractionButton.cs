using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public Button button;

    public void Setup(UIButtonData data)
    {
        if (buttonText != null)
            buttonText.text = data.text;
        
        if (button != null)
        {
            button.interactable = data.interactable;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => 
            {
                if (data.interactable)
                    data.onClick?.Invoke();
            });
        }
    }
}