using UnityEngine;
using UnityEngine.UI;

public class HotbarSlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public Text quantityText;

    [Range(0, 6)]
    public int slotIndex; // ВАЖНО: установите 0 для первого слота, 1 для второго и т.д.

    private void Start()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager не найден!");
            enabled = false;
            return;
        }

        InventoryManager.Instance.OnInventoryChanged += UpdateVisuals;
        InventoryManager.Instance.OnSlotSelected += HighlightSlot;
        
        UpdateVisuals();
        HighlightSlot(InventoryManager.Instance.selectedSlotIndex);
    }

    private void OnDestroy()
    {
        if(InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= UpdateVisuals;
            InventoryManager.Instance.OnSlotSelected -= HighlightSlot;
        }
    }

    public void UpdateVisuals()
    {
        if (InventoryManager.Instance == null) return;
        if (InventoryManager.Instance.slots == null) return;
        if (slotIndex >= InventoryManager.Instance.slots.Length) return;

        var slotData = InventoryManager.Instance.slots[slotIndex];

        if (slotData.item != null)
        {
            if (iconImage != null)
            {
                // ✅ Используем спрайт для интерфейса!
                iconImage.sprite = slotData.item.inventoryIcon;
                iconImage.color = Color.white;
            }
            if (quantityText != null)
                quantityText.text = "1"; 
        }
        else
        {
            if (iconImage != null)
            {
                iconImage.sprite = null;
                iconImage.color = Color.clear;
            }
            if (quantityText != null)
                quantityText.text = "";
        }
    }

    public void HighlightSlot(int index)
    {
        Image bgImage = GetComponent<Image>();
        if (bgImage == null) return;
        
        if (index == slotIndex)
        {
            bgImage.color = new Color(1, 1, 1, 0.5f); // Выделенный слот
        }
        else
        {
            bgImage.color = new Color(0, 0, 0, 0.5f); // Обычный слот
        }
    }
}