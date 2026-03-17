using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class WorldObject : MonoBehaviour {
    public string objectName;
    public bool isResolved = false;
    
    public enum ObjectMode { Pickup, Place, UseItem }
    public ObjectMode mode = ObjectMode.UseItem;
    
    [Header("Items")]
    public Item itemToGive;      // Какой предмет дать (Pickup)
    public Item itemToTake;      // Какой предмет забрать (Place)
    public Item requiredItem;    // Нужен ли предмет для авто-крафта (Pickup)
    
    [Header("Use Item Mode")]
    public List<ItemAction> itemActions = new List<ItemAction>();
    
    [System.Serializable]
    public class ItemAction {
        public Item requiredItem;
        public int echoChange;
        public Sprite resultSprite;
        public UnityEvent onAction;
        [TextArea] public string successMessage;
        [TextArea] public string failMessage;
    }
    
    [Header("Echo")]
    public int echoOnPickup = 0;
    public int echoOnHide = -5;
    public int echoOnPlace = 8;
    
    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Sprite filledSprite;

    // Вызывается при клике мышкой по объекту
    void OnMouseDown() {
        if (mode == ObjectMode.Pickup) {
            HandlePickup();
        } else if (mode == ObjectMode.Place) {
            HandlePlace();
        } else if (mode == ObjectMode.UseItem) {
            ShowItemSelection();
        }
    }

    // Вызывается при перетаскивании предмета из инвентаря
    public bool TryUseItem(Item item) {
        if (mode != ObjectMode.UseItem) return false;
        
        foreach (var action in itemActions) {
            if (action.requiredItem == item) {
                ExecuteAction(action);
                return true;
            }
        }
        
        UIManager.Instance.ShowNotification("Этот предмет не подойдет");
        return false;
    }

    void ExecuteAction(ItemAction action) {
        if (isResolved) {
            UIManager.Instance.ShowNotification("Уже обработано");
            return;
        }
        
        isResolved = true;
        
        if (spriteRenderer != null && action.resultSprite != null) {
            spriteRenderer.sprite = action.resultSprite;
        }
        
        EchoManager.Instance.AddEcho(action.echoChange);
        action.onAction?.Invoke();
        
        string message = action.echoChange > 0 ? 
            $"{action.successMessage} (+Эхо)" : 
            $"{action.failMessage} (-Эхо)";
        UIManager.Instance.ShowNotification(message);
    }

    void ShowItemSelection() {
        string msg = "Нужен предмет:\n";
        foreach (var action in itemActions) {
            bool hasItem = InventoryManager.Instance.HasItem(action.requiredItem.name);
            msg += hasItem ? "✅ " : "❌ ";
            msg += action.requiredItem.itemName + "\n";
        }
        UIManager.Instance.ShowNotification(msg);
    }

    void HandlePickup() {
        if (isResolved) {
            UIManager.Instance.ShowNotification("Здесь пусто.");
            return;
        }

        // Проверяем, есть ли предмет для крафта (например, иголка)
        bool hasRequiredItem = requiredItem != null && 
                               InventoryManager.Instance.HasItem(requiredItem.name);
        
        if (hasRequiredItem) {
            // ЕСТЬ иголка → даем починенную игрушку
            if (itemToGive != null) {
                // Ищем предмет для выдачи (починенный)
                Item giveItem = Resources.Load<Item>("Items/" + itemToGive.name);
                if (giveItem == null) giveItem = itemToGive;

                if (InventoryManager.Instance.AddItem(giveItem)) {
                    isResolved = true;
                    if (spriteRenderer != null) spriteRenderer.enabled = false;
                    EchoManager.Instance.AddEcho(echoOnPickup);
                    UIManager.Instance.ShowNotification($"Вы забрали: {giveItem.itemName}");
                }
            }
        } else {
            // НЕТ иголки → просто прячем (автоматически, без выбора)
            isResolved = true;
            if (spriteRenderer != null) spriteRenderer.enabled = false;
            EchoManager.Instance.AddEcho(echoOnHide);
            UIManager.Instance.ShowNotification("Вы спрятали игрушку в шкаф (-Эхо)");
        }
    }

    void HandlePlace() {
        if (isResolved) {
            UIManager.Instance.ShowNotification("Уже занято.");
            return;
        }

        if (itemToTake != null) {
            if (InventoryManager.Instance.HasItem(itemToTake.name)) {
                InventoryManager.Instance.RemoveItemByName(itemToTake.name);
                isResolved = true;
                if (spriteRenderer != null && filledSprite != null)
                    spriteRenderer.sprite = filledSprite;
                EchoManager.Instance.AddEcho(echoOnPlace);
                UIManager.Instance.ShowNotification("Вы поставили на место (+Эхо)");
            } else {
                UIManager.Instance.ShowNotification("У вас нет подходящего предмета");
            }
        }
    }
}