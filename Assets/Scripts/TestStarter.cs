using UnityEngine;

public class TestStarter : MonoBehaviour {
    void Start() {
        // Даём игроку иголку при старте
        Item needle = Resources.Load<Item>("Items/Item_Needle");
        
        if (needle != null) {
            InventoryManager.Instance.AddItem(needle);
            Debug.Log("✅ Иголка добавлена в инвентарь!");
        } else {
            Debug.LogError("❌ Не найдена Item_Needle!");
        }
    }
}