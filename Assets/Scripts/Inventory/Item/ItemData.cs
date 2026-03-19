using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Информация")]
    public string itemName;
    [TextArea] public string description;
    
    [Header("🎨 Спрайты")]
    [Tooltip("Спрайт для хотбара и инвентаря (мелкий, стилизованный)")]
    public Sprite inventoryIcon;
    
    [Tooltip("Спрайт для отображения в мире (на полу, в руке)")]
    public Sprite worldSprite;
    
    [Header("⚙️ Настройки")]
    public int maxStackSize = 1;
    public bool isUsable = true;
}