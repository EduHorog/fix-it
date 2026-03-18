using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Tooltip UI")]
    public GameObject tooltipPanel;
    public Text nameText;
    public Text descText;

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
        // Проверяем что pointerEnter не null
        if (eventData.pointerEnter == null)
            return;

        // Ищем HotbarSlotUI в родителях
        var slotUI = eventData.pointerEnter.GetComponentInParent<HotbarSlotUI>();
        if (slotUI == null)
            return;

        // Проверяем менеджер
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager.Instance is null!");
            return;
        }

        int slotIndex = slotUI.slotIndex;
        
        // Проверяем границы массива
        if (slotIndex < 0 || slotIndex >= InventoryManager.Instance.slots.Length)
            return;

        var item = InventoryManager.Instance.slots[slotIndex].item;

        if (item != null)
        {
            // Проверяем тексты
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
        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            Vector2 mousePos = Input.mousePosition;
            
            if (canvas != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.GetComponent<RectTransform>(),
                    mousePos,
                    canvas.worldCamera,
                    out Vector2 localPoint
                );

                if (tooltipRect != null)
                    tooltipRect.anchoredPosition = localPoint + new Vector2(20, -30);
            }
        }
    }
}