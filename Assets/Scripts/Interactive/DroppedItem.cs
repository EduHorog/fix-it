using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public ItemData itemData;
    public float pickupRange = 1.5f;
    public float floatSpeed = 1f;
    public float floatAmount = 0.1f;
    
    private Vector3 startPosition;
    private bool isPlayerNear = false;
    private string promptText = "Нажмите [E] чтобы подобрать";

    private void Start()
    {
        startPosition = transform.position;
        UpdatePrompt();
    }

    private void Update()
    {
        // Парящее движение
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        
        // Подбор
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            Pickup();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            UpdatePrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            UpdatePrompt();
        }
    }

    private void Pickup()
    {
        if (itemData == null)
        {
            Destroy(gameObject);
            return;
        }

        // Пытаемся добавить в инвентарь
        if (InventoryManager.Instance != null)
        {
            if (InventoryManager.Instance.AddItem(itemData))
            {
                GameUIManager.Instance?.ShowMessage($"Подобрано: {itemData.itemName}");
                Destroy(gameObject);
            }
            else
            {
                GameUIManager.Instance?.ShowMessage("Инвентарь полон!");
            }
        }
        else
        {
            // Если нет менеджера инвентаря - просто удаляем
            Destroy(gameObject);
        }
    }

    private void UpdatePrompt()
    {
        if (isPlayerNear)
            GameUIManager.Instance?.ShowPrompt(promptText);
        else
            GameUIManager.Instance?.HidePrompt();
    }

    public void Setup(ItemData data)
    {
        itemData = data;
    }
}