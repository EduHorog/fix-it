using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    public static InventoryManager Instance;
    
    public int maxSlots = 7;
    public List<Item> items = new List<Item>();
    public List<GameObject> slotObjects = new List<GameObject>();
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public bool AddItem(Item item) {
        if (items.Count >= maxSlots) {
            UIManager.Instance.ShowNotification("Инвентарь полон!");
            return false;
        }
        
        items.Add(item);
        UpdateUI();
        CheckCombinations();
        return true;
    }

    public void RemoveItem(int index) {
        if (index >= 0 && index < items.Count) {
            items.RemoveAt(index);
            UpdateUI();
        }
    }

    public void RemoveItem(Item itemToRemove) {
        for (int i = 0; i < items.Count; i++) {
            if (items[i] == itemToRemove) {
                items.RemoveAt(i);
                UpdateUI();
                return;
            }
        }
    }

    public void RemoveItemByName(string itemNameContains) {
        for (int i = 0; i < items.Count; i++) {
            if (items[i] != null && items[i].name.Contains(itemNameContains)) {
                items.RemoveAt(i);
                UpdateUI();
                return;
            }
        }
    }

    void CheckCombinations() {
        for (int i = 0; i < items.Count; i++) {
            for (int j = i + 1; j < items.Count; j++) {
                if (CanCombine(items[i], items[j])) {
                    CombineItems(i, j);
                    return;
                }
            }
        }
    }

    bool CanCombine(Item a, Item b) {
        if (a.combineWith != null) {
            foreach (string id in a.combineWith) {
                if (b.name.Contains(id)) return true;
            }
        }
        if (b.combineWith != null) {
            foreach (string id in b.combineWith) {
                if (a.name.Contains(id)) return true;
            }
        }
        return false;
    }

    void CombineItems(int indexA, int indexB) {
        Item itemA = items[indexA];
        Item itemB = items[indexB];
        
        Item result = FindItemByID(itemA.resultItemID);
        if (result == null) result = FindItemByID(itemB.resultItemID);
        
        if (result != null) {
            int maxIndex = Mathf.Max(indexA, indexB);
            int minIndex = Mathf.Min(indexA, indexB);
            
            RemoveItem(maxIndex);
            RemoveItem(minIndex);
            
            AddItem(result);
            
            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlaySound("Craft");
            }
            UIManager.Instance.ShowNotification($"Создано: {result.itemName}!");
        }
    }

    Item FindItemByID(string id) {
        Item[] allItems = Resources.LoadAll<Item>("");
        foreach (Item item in allItems) {
            if (item.name.Contains(id)) return item;
        }
        return null;
    }

    void UpdateUI() {
        for (int i = 0; i < maxSlots; i++) {
            if (i < slotObjects.Count) {
                InventorySlot slot = slotObjects[i].GetComponent<InventorySlot>();
                if (i < items.Count) {
                    slot.SetItem(items[i]);
                } else {
                    slot.SetItem(null);
                }
            }
        }
    }

    public bool HasItem(string itemNameContains) {
        foreach (Item item in items) {
            if (item != null && item.name.Contains(itemNameContains)) return true;
        }
        return false;
    }
    
    public Item GetItem(string itemNameContains) {
        foreach (Item item in items) {
            if (item != null && item.name.Contains(itemNameContains)) return item;
        }
        return null;
    }
}