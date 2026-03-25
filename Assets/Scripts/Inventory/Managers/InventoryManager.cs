using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public int hotbarSize = 7;
    public Transform dropPoint; // Точка выброса (рука игрока)
    public GameObject itemPrefab; // Префаб предмета для выброса

    [Serializable]
    public class Slot
    {
        public ItemData item;
        public bool IsEmpty => item == null;
    }

    public Slot[] slots;
    public int selectedSlotIndex = 0;

    public event Action OnInventoryChanged;
    public event Action<int> OnSlotSelected;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        slots = new Slot[hotbarSize];
        for (int i = 0; i < hotbarSize; i++) slots[i] = new Slot();
    }

    public bool AddItem(ItemData item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty)
            {
                slots[i].item = item;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }
        Debug.Log("Инвентарь полон!");
        return false;
    }

    public void UseItem()
    {
        Slot currentSlot = slots[selectedSlotIndex];
        if (currentSlot.IsEmpty) return;

        Debug.Log($"Используем: {currentSlot.item.itemName}");
        RemoveItem(selectedSlotIndex);
    }

   public void DropItem()
    {
        Slot currentSlot = slots[selectedSlotIndex];
        if (currentSlot.IsEmpty) return;

        if (itemPrefab != null && dropPoint != null)
        {
            // Спавн предмета
            GameObject droppedItem = Instantiate(itemPrefab, dropPoint.position, Quaternion.identity);
            PickupItem2D pickup = droppedItem.GetComponent<PickupItem2D>();
            if (pickup != null)
            {
                pickup.itemData = currentSlot.item;
            }
        }
        else
        {
            Debug.LogWarning("Не настроен itemPrefab или dropPoint в InventoryManager!");
        }

        RemoveItem(selectedSlotIndex);
    }

    public void RemoveItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return;
        slots[slotIndex].item = null;
        OnInventoryChanged?.Invoke();
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= hotbarSize) return;
        selectedSlotIndex = index;
        OnSlotSelected?.Invoke(index);
    }

    // Добавь в класс InventoryManager:

    /// <summary>
    /// Проверяет, есть ли у игрока хотя бы один экземпляр предмета
    /// </summary>
    public bool HasItem(ItemData item)
    {
        if (item == null) return false;
        
        foreach (var slot in slots)
        {
            if (slot.item == item) return true;
        }
        return false;
    }

    /// <summary>
    /// Удаляет один экземпляр предмета из инвентаря
    /// </summary>
    public bool RemoveOneItem(ItemData item)
    {
        if (item == null) return false;
        
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                slots[i].item = null;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }
        return false;
    }
}