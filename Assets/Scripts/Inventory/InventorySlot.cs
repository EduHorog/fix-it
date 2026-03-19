using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public Image icon;
    public Button button;
    private Item current_item;
    
    private Canvas canvas;
    private RectTransform rectTransform;
    private Transform originalParent;
    private int originalIndex;
    
    void Start() {
        if (button == null) button = GetComponent<Button>();
        if (button != null) button.onClick.AddListener(OnSlotClick);
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetItem(Item item) {
        current_item = item;
        if (item != null) {
            icon.sprite = item.icon;
            icon.color = Color.white;
            GetComponent<Image>().raycastTarget = true;
        } else {
            icon.sprite = null;
            icon.color = Color.clear;
            GetComponent<Image>().raycastTarget = false;
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (current_item == null) return;
        
        originalParent = transform.parent;
        originalIndex = transform.GetSiblingIndex();
        
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
        
        icon.color = new Color(1, 1, 1, 0.5f);
    }

    public void OnDrag(PointerEventData eventData) {
        if (current_item == null) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (current_item == null) return;
        
        icon.color = Color.white;
        
        // Проверяем, над каким объектом отпустили
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = eventData.position;
        
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        
        bool droppedOnObject = false;
        
        foreach (var result in results) {
            WorldObject worldObj = result.gameObject.GetComponent<WorldObject>();
            if (worldObj != null && worldObj.enabled) {
                if (worldObj.TryUseItem(current_item)) {
                    InventoryManager.Instance.RemoveItem(current_item);
                    droppedOnObject = true;
                    break;
                }
            }
        }
        
        if (!droppedOnObject) {
            ReturnToOriginalPosition();
        }
    }
    
    void ReturnToOriginalPosition() {
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalIndex);
        icon.color = Color.white;
    }

    void OnSlotClick() {
        if (current_item != null) {
            UIManager.Instance.ShowNotification(current_item.description);
        }
    }

    public Item GetItem() => current_item;
}