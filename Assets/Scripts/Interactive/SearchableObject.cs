using UnityEngine;
using System.Collections.Generic;

public class SearchableObject : MonoBehaviour
{
    [Header("Настройки обыска")]
    public string objectName = "Объект";
    public float searchRange = 3f;
    public bool hasBeenSearched = false;
    
    [Header("Лут")]
    public List<LootSlot> lootTable = new List<LootSlot>();
    public Transform lootSpawnPoint; // Где спавнить предметы (опционально)
    
    [Header("Сообщения")]
    public string emptyMessage = "Здесь ничего нет";
    public string foundMessage = "Вы нашли: ";
    public string alreadySearchedMessage = "Вы уже обыскали это";
    
    [Header("Визуал")]
    public SpriteRenderer spriteRenderer;
    public Sprite searchedSprite; // Спрайт после обыска
    public Color searchedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    
    [Header("Звуки")]
    public AudioClip searchSound;
    public AudioClip emptySound;
    public AudioClip foundSound;
    
    private bool isPlayerNear = false;
    private string promptText = "Нажмите [E] для обыска";

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
            
        if (lootSpawnPoint == null)
            lootSpawnPoint = transform;
    }

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            Search();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            GameUIManager.Instance?.ShowPrompt(promptText);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            GameUIManager.Instance?.HidePrompt();
        }
    }

    public void Search()
    {
        // Воспроизводим звук обыска
        if (searchSound != null)
            AudioSource.PlayClipAtPoint(searchSound, transform.position);

        // Если уже обыскан
        if (hasBeenSearched)
        {
            if (!string.IsNullOrEmpty(alreadySearchedMessage))
                GameUIManager.Instance?.ShowMessage(alreadySearchedMessage);
            return;
        }

        // Помечаем как обысканный
        hasBeenSearched = true;
        UpdateVisuals();

        // Собираем найденные предметы
        List<ItemData> foundItems = new List<ItemData>();
        
        foreach (var lootSlot in lootTable)
        {
            if (lootSlot.item != null && ShouldDropItem(lootSlot))
            {
                int count = Random.Range(lootSlot.minCount, lootSlot.maxCount + 1);
                
                for (int i = 0; i < count; i++)
                {
                    foundItems.Add(lootSlot.item);
                    SpawnItemOnFloor(lootSlot.item);
                }
            }
        }

        // Показываем результат
        if (foundItems.Count == 0)
        {
            if (emptySound != null)
                AudioSource.PlayClipAtPoint(emptySound, transform.position);
                
            if (!string.IsNullOrEmpty(emptyMessage))
                GameUIManager.Instance?.ShowMessage(emptyMessage);
        }
        else
        {
            if (foundSound != null)
                AudioSource.PlayClipAtPoint(foundSound, transform.position);
                
            // Показываем что найдено
            string message = foundMessage;
            HashSet<ItemData> uniqueItems = new HashSet<ItemData>(foundItems);
            bool first = true;
            
            foreach (var item in uniqueItems)
            {
                if (!first) message += ", ";
                message += item.itemName;
                first = false;
            }
            
            GameUIManager.Instance?.ShowMessage(message);
        }

        // Скрываем подсказку
        GameUIManager.Instance?.HidePrompt();
        isPlayerNear = false;
    }

    private bool ShouldDropItem(LootSlot slot)
    {
        if (slot.dropChance >= 100f) return true;
        return Random.Range(0f, 100f) <= slot.dropChance;
    }

    private void SpawnItemOnFloor(ItemData item)
    {
        if (item == null || item.itemPrefab == null)
        {
            Debug.LogWarning($"У предмета {item?.itemName} нет префаба!");
            return;
        }

        // Определяем позицию спавна
        Vector3 spawnPosition = GetSpawnPosition();
        
        // Спавним предмет
        GameObject itemObject = Instantiate(item.itemPrefab, spawnPosition, Quaternion.identity);
        
        // Добавляем компонент для подбора (если нужен)
        DroppedItem droppedItem = itemObject.GetComponent<DroppedItem>();
        if (droppedItem == null)
            droppedItem = itemObject.AddComponent<DroppedItem>();
            
        droppedItem.Setup(item);
        
        // Добавляем физику (если ещё нет)
        Rigidbody2D rb = itemObject.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = itemObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 1f;
            rb.mass = 0.5f;
        }
        
        // Добавляем коллайдер (если ещё нет)
        Collider2D col = itemObject.GetComponent<Collider2D>();
        if (col == null)
        {
            // Пытаемся получить размер из спрайта
            SpriteRenderer sr = itemObject.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                BoxCollider2D box = itemObject.AddComponent<BoxCollider2D>();
                box.size = sr.sprite.bounds.size;
            }
            else
            {
                itemObject.AddComponent<CircleCollider2D>();
            }
        }
        
        // Небольшой случайный разброс
        float randomOffset = Random.Range(-0.5f, 0.5f);
        itemObject.transform.position += new Vector3(randomOffset, 0, 0);
    }

    private Vector3 GetSpawnPosition()
    {
        // Если есть точка спавна - используем её
        if (lootSpawnPoint != null)
            return lootSpawnPoint.position;
            
        // Иначе спавним рядом с объектом
        return transform.position + Vector3.down * 0.5f;
    }

    private void UpdateVisuals()
    {
        if (spriteRenderer == null) return;
        
        if (searchedSprite != null)
            spriteRenderer.sprite = searchedSprite;
        else
            spriteRenderer.color = searchedColor;
    }

    // Для отладки - можно вызвать из инспектора
    [ContextMenu("Reset Search")]
    public void ResetSearch()
    {
        hasBeenSearched = false;
        UpdateVisuals();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, searchRange);
        
        if (lootSpawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(lootSpawnPoint.position, 0.2f);
        }
    }
}

[System.Serializable]
public class LootSlot
{
    public ItemData item;
    [Range(0f, 100f)] public float dropChance = 100f; // Шанс выпадения в %
    public int minCount = 1;
    public int maxCount = 1;
}