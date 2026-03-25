using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // ← Обязательно для TMP

public class TooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Tooltip UI")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI nameText;  // ← Изменено на TextMeshProUGUI
    public TextMeshProUGUI descText;  // ← Изменено на TextMeshProUGUI

    [Header("Tooltip Offset")]
    public Vector2 offset = new Vector2(0, 40); // ← Сдвиг: X=вправо/влево, Y=вверх/вниз от курсора

    private RectTransform tooltipRect;
    private Canvas canvas;

    private void Start()
    {
        if (tooltipPanel == null)
        {
            Debug.LogError("Tooltip Panel не назначен!");
            return;
        }
        
        tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        
        if (canvas == null)
        {
            Debug.LogError("Canvas не найден!");
        }
        
        tooltipPanel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerEnter == null)
            return;

        var slotUI = eventData.pointerEnter.GetComponentInParent<HotbarSlotUI>();
        if (slotUI == null)
            return;

        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager.Instance is null!");
            return;
        }

        int slotIndex = slotUI.slotIndex;
        
        if (slotIndex < 0 || slotIndex >= InventoryManager.Instance.slots.Length)
            return;

        var item = InventoryManager.Instance.slots[slotIndex].item;

        if (item != null)
        {
            if (nameText != null)
                nameText.text = item.itemName;
            
            if (descText != null)
                descText.text = item.description;
                
            if (tooltipPanel != null)
                tooltipPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }

    private void LateUpdate()
    {
        if (tooltipPanel != null && tooltipPanel.activeSelf && tooltipRect != null && canvas != null)
        {
            Vector2 mousePos = Input.mousePosition;
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                mousePos,
                canvas.worldCamera,
                out Vector2 localPoint
            );

            // ← Позиционируем тултип НАД курсором (положительный Y = вверх)
            tooltipRect.anchoredPosition = localPoint + offset;
            
            // Опционально: предотвращаем выход за границы экрана
            ClampTooltipToScreen();
        }
    }

    // Опциональный метод: чтобы тултип не уходил за край экрана
    private void ClampTooltipToScreen()
    {
        if (canvas == null || tooltipRect == null) return;

        Vector2 canvasSize = (canvas.GetComponent<RectTransform>() as RectTransform).sizeDelta;
        Vector2 tooltipSize = tooltipRect.sizeDelta;
        Vector2 pos = tooltipRect.anchoredPosition;

        // Ограничиваем по горизонтали
        float halfWidth = tooltipSize.x / 2;
        pos.x = Mathf.Clamp(pos.x, -canvasSize.x / 2 + halfWidth, canvasSize.x / 2 - halfWidth);

        // Ограничиваем по вертикали (особенно важно сверху)
        float halfHeight = tooltipSize.y / 2;
        pos.y = Mathf.Clamp(pos.y, -canvasSize.y / 2 + halfHeight, canvasSize.y / 2 - halfHeight);

        tooltipRect.anchoredPosition = pos;
    }
}