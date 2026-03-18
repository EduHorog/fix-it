using System;
using UnityEngine;

[Serializable]
public class InteractionChoice
{
    [Header("Требования")]
    public string choiceName;           // Название в меню: "Разбить", "Починить"
    public ItemData requiredItem;       // Какой предмет нужен
    public bool consumeItem = true;     // Расходуется ли предмет

    [Header("Результаты")]
    public Sprite newSprite;            // ✅ Новый спрайт объекта (вместо Texture2D)
    public string successMessage;       // Сообщение при успехе
    public bool destroyObject = false;  // Уничтожить объект после действия?

    [Header("Звуки/Эффекты")]
    public AudioClip successSound;
    public ParticleSystem successEffect;

    // ✅ Безопасная проверка инвентаря
    public bool IsItemInInventory => 
        InventoryManager.Instance != null && 
        (requiredItem == null || InventoryManager.Instance.HasItem(requiredItem));
}