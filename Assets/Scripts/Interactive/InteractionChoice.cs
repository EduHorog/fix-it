using System;
using UnityEngine;

[Serializable]
public class InteractionChoice
{
    [Header("Требования")]
    public string choiceName;
    public ItemData requiredItem;
    public bool consumeItem = true;

    [Header("Результаты")]
    public Sprite newSprite;
    public string successMessage;
    public bool destroyObject = false;

    [Header("Звуки/Эффекты")]
    public AudioClip successSound;
    public ParticleSystem successEffect;

    [Header("Спец. события")]
    public MirrorShadowEvent shadowEvent;
    public bool triggerShadow = false;

    // ✅ НОВОЕ: Система кармы (опционально)
    [Header("Карма")]
    public int karmaChange = 0; // Например: +5, -10, 0
    public bool isCorrectChoice = false;

    // ✅ Безопасная проверка инвентаря
    public bool IsItemInInventory => 
        InventoryManager.Instance != null && 
        (requiredItem == null || InventoryManager.Instance.HasItem(requiredItem));
}