    using UnityEngine;
    using System.Collections.Generic;

    public class InteractableObject : MonoBehaviour
    {
        [Header("Настройки взаимодействия")]
        public string objectName = "Объект";
        public string promptText = "Нажмите [E] для взаимодействия";
        public float interactionRange = 3f;
        
        [Header("Визуал")]
        public SpriteRenderer spriteRenderer;
        public Sprite defaultSprite; // ✅ Спрайт по умолчанию
        
        [Header("Действия")]
        public List<InteractionChoice> choices = new List<InteractionChoice>();
        
        [Header("События")]
        public UnityEngine.Events.UnityEvent onInteractStart;
        public UnityEngine.Events.UnityEvent onInteractEnd;

        private bool isPlayerNear = false;
        private Sprite originalSprite; // ✅ Запоминаем оригинальный спрайт

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            
            // Сохраняем исходный спрайт для возможного возврата
            if (spriteRenderer != null)
            {
                originalSprite = spriteRenderer.sprite;
                if (defaultSprite != null)
                    spriteRenderer.sprite = defaultSprite;
            }
        }

        private void Update()
        {
            if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
            {
                OpenInteractionMenu();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNear = true;
                GameUIManager.Instance?.ShowPrompt(promptText);
                onInteractStart?.Invoke();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNear = false;
                GameUIManager.Instance?.HidePrompt();
                GameUIManager.Instance?.CloseInteractionMenu();
                onInteractEnd?.Invoke();
            }
        }

        public void OpenInteractionMenu()
        {
            if (GameUIManager.Instance == null)
            {
                Debug.LogError("GameUIManager не найден!");
                return;
            }
            GameUIManager.Instance.HidePrompt();
            List<UIButtonData> buttons = new List<UIButtonData>();
            
            foreach (var choice in choices)
            {
                // Создаём локальную копию для замыкания
                InteractionChoice selectedChoice = choice;
                
                string buttonText = selectedChoice.IsItemInInventory 
                    ? $"Использовать {selectedChoice.choiceName}" 
                    : $"Использовать ***";
                
                buttons.Add(new UIButtonData
                {
                    text = buttonText,
                    onClick = () => ExecuteChoice(selectedChoice), // ← Теперь работает!
                    interactable = selectedChoice.IsItemInInventory
                });
            }

            GameUIManager.Instance.ShowInteractionMenu(objectName, buttons);
        }

        private void ExecuteChoice(InteractionChoice choice)
        {
            if (!choice.IsItemInInventory)
            {
                GameUIManager.Instance?.ShowMessage("У вас нет нужного предмета!");
                return;
            }

            if (choice.consumeItem)
            {
                InventoryManager.Instance.RemoveOneItem(choice.requiredItem);
            }

            if (!string.IsNullOrEmpty(choice.successMessage))
            {
                GameUIManager.Instance?.ShowMessage(choice.successMessage);
            }

            if (choice.successSound != null)
                AudioSource.PlayClipAtPoint(choice.successSound, transform.position);
            
            if (choice.successEffect != null)
                choice.successEffect.Play();

            // ✅ Меняем спрайт, если указано
            if (choice.newSprite != null)
            {
                ChangeSprite(choice.newSprite);
            }

            if (choice.destroyObject)
            {
                Destroy(gameObject, 0.3f);
            }
            
            if (choice.triggerShadow && choice.shadowEvent != null)
            {
                choice.shadowEvent.TriggerShadowEvent();
            }

            GameUIManager.Instance?.CloseInteractionMenu();
        }

        // ✅ Новый метод для смены спрайта
        public void ChangeSprite(Sprite newSprite)
        {
            if (spriteRenderer != null && newSprite != null)
            {
                spriteRenderer.sprite = newSprite;
            }
        }

        // ✅ Метод для возврата к исходному спрайту (опционально)
        public void ResetSprite()
        {
            if (spriteRenderer != null && originalSprite != null)
            {
                spriteRenderer.sprite = originalSprite;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }